#!/bin/bash

set -eu

#echo "引数 $#"
#exit

PROGNAME=$(basename $0)

usage_exit() {
	echo "Usage: ${PROGNAME} [--ipa] [dev|prod]" 1>&2
	echo "      -h, --help"
	echo "      --ipa: export ipa(Build only by default)"
	echo "      dev: develop build"
	echo "      prod: production build"
	exit 1
}

BUILD_TYPE=dev

for OPT in "$@"
do
	case $OPT in
		--help | -h)
			usage_exit
			;;
		--ipa)
			EXPORT_IPA=1
			;;
		prod)
			BUILD_TYPE=prod
			;;
		dev)
			BUILD_TYPE=dev
			;;
		*)
			usage_exit
			;;
	esac
	shift
done

SCRIPT_DIR=$(cd $(dirname $0); pwd)
#source ${SCRIPT_DIR}/unity.env
source ${SCRIPT_DIR}/build_com.env
if [ ${BUILD_TYPE} == "prod" ]; then
	source ${SCRIPT_DIR}/build_prod.env
else
	source ${SCRIPT_DIR}/build_dev.env
fi
source ${SCRIPT_DIR}/version.env

# jenkinsから呼ぶときの予約
BUILD_NUMBER=777

# jenkinsから呼ぶならコメントアウト
WORKSPACE=$(cd $(dirname ${SCRIPT_DIR}/../../../); pwd)

PRODUCT_NAME=$(cat ${WORKSPACE}/ProjectSettings/ProjectSettings.asset | grep "productName:" | sed -E "s/  productName: //" | sed -E "s/ Dev//g" )

if [ ${BUILD_TYPE} == "prod" ]; then
	# prodの場合
	PRODUCT_NAME_FULL="${PRODUCT_NAME}"
	RSP_FILE=${SCRIPT_DIR}/prod.rsp
else
	# devの場合
	PRODUCT_NAME_FULL="${PRODUCT_NAME} Dev"
	RSP_FILE=${SCRIPT_DIR}/dev.rsp
fi

# 出力先
EXPORT_ROOT=${WORKSPACE}/export
XCODE_PROJECT_DIR=${EXPORT_ROOT}/xcodeProject
IPA_DIR=${EXPORT_ROOT}/ipa

echo BUILD_TYPE:$BUILD_TYPE
if ! [ "${EXPORT_IPA:+foo}" ]; then
	echo EXPORT_IPA:build only
else
	echo "EXPORT_IPA:build and export ipa"
fi
echo SCRIPT_DIR:$SCRIPT_DIR
echo WORKSPACE:$WORKSPACE
echo EXPORT_ROOT:$EXPORT_ROOT
echo XCODE_PROJECT_DIR:$XCODE_PROJECT_DIR
echo PRODUCT_NAME_FULL:$PRODUCT_NAME_FULL
echo RSP_FILE:$RSP_FILE
echo UNITY_APP:$UNITY_APP
echo UNITY_USERNAME:$UNITY_USERNAME
echo BUNDLE_IDENTIFIER:$BUNDLE_IDENTIFIER
echo BUNDLE_VERSION:$BUNDLE_VERSION
echo BUNDLE_VERSION_INTERNAL:$BUNDLE_VERSION_INTERNAL
echo DEVELOPMENT_BUILD:$DEVELOPMENT_BUILD
echo BUILD_NUMBER:$BUILD_NUMBER
echo UNITY_PROJECT_ID:$UNITY_PROJECT_ID
echo UNITY_PROJECT_NAME:$UNITY_PROJECT_NAME
echo logPath:${WORKSPACE}/${BUILD_NUMBER}.log

if ! [ "${UNITY_USERNAME:+foo}" ] || ! [ "${UNITY_PASSWORD:+foo}" ]; then
	echo "UNITY_USERNAME($UNITY_USERNAME) と UNITY_PASSWORD($UNITY_PASSWORD) を定義してください"
	exit 1
fi

#exit

mkdir ${EXPORT_ROOT} || true
rm -rf ${XCODE_PROJECT_DIR} || true
#mv ${XCODE_PROJECT_DIR} ${XCODE_PROJECT_DIR}.$$
mkdir ${XCODE_PROJECT_DIR} || true
mkdir ${IPA_DIR} || true
cp -v ${RSP_FILE} ${WORKSPACE}/Assets/csc.rsp

function force_rebuild(){
	# Force Rebuild (と言いつつ、大抵二回実行しないと切り替わらない)
	rm Library/ScriptAssemblies/* || true
}

function firebase_forceupdatejson(){
	${UNITY_APP} -batchmode -quit -executeMethod CloudBuildExportMethod.ForceUpdateXml \
	-projectPath ${WORKSPACE} \
	-buildTarget ios \
	-username ${UNITY_USERNAME} \
	-password ${UNITY_PASSWORD} \
	-logFile -
}

function firebase_copy(){
	RETRY=$1
	echo firebase_copy RETRY: ${RETRY}
	force_rebuild
	# FirebaseFiles Copy
	${UNITY_APP} -batchmode -quit -executeMethod CloudBuildExportMethod.CopyFirebaseConfigFiles \
	-projectPath ${WORKSPACE} \
	-buildTarget ios \
	-username ${UNITY_USERNAME} \
	-password ${UNITY_PASSWORD} \
	-logFile ${WORKSPACE}/${BUILD_NUMBER}_fb_${RETRY}.log 

	# grepに備えて一時的に緩和
	set +e
	echo Firebase Config Files Check
	find Assets -name "google-services.json" | xargs grep ${BUNDLE_IDENTIFIER}
	if [ $? -ne 0 ]; then
		echo Error!
		find Assets -name "google-services.json" | xargs grep package_name
		find Assets -name "google-services.json" | xargs grep bundle_id
		if [ ${RETRY} == "1" ]; then
			exit 1
		else
			firebase_copy "1"
		fi
	fi
	# 再度厳密に
	set -e
}

if [ ${BUILD_TYPE} == "prod" ]; then
	# prodの場合
	${SCRIPT_DIR}/pre_build_adjust_projectid_prod.sh
else
	# devの場合
	echo ""
fi

firebase_forceupdatejson

firebase_copy "0"

# grepに備えて一時的に緩和
set +e

grep -i login ${WORKSPACE}/${BUILD_NUMBER}_fb_*.log | grep -i fail
if [ $? -ne 1 ]; then
	echo Error!
	exit 1
fi

echo Firebase Copy Success

# 再度厳密に
set -e

#exit

# replace cloud project
sed -i '' -e "s/cloudProjectId: .*$/cloudProjectId: ${UNITY_PROJECT_ID}/g" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset
sed -i '' -e "s/projectName: .*$/projectName: ${UNITY_PROJECT_NAME}/g" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset

#exit

# Unity build
${UNITY_APP} -batchmode -quit -executeMethod Build.BuildProcess \
-projectPath ${WORKSPACE} \
-buildTarget ios \
-username ${UNITY_USERNAME} \
-password ${UNITY_PASSWORD} \
-logFile ${WORKSPACE}/${BUILD_NUMBER}.log \
-productName="${PRODUCT_NAME_FULL}" \
-bundleVersion=${BUNDLE_VERSION} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} \
-developmentBuild=${DEVELOPMENT_BUILD} \
-buildNumber=${BUNDLE_VERSION_INTERNAL} \
-outputPath=${XCODE_PROJECT_DIR}  || exit 1

# grepに備えて一時的に緩和
set +e

grep -i login ${WORKSPACE}/${BUILD_NUMBER}.log | grep -i fail
if [ $? -ne 1 ]; then
	echo Error!
	exit 1
fi

echo Unity Build Success

# 再度厳密に
set -e

if ! [ "${EXPORT_IPA:+foo}" ]; then
	echo あとは、export/xcodeProject/Unity-iPhone.xcworkspaceをXcode 12で開いて、 Archive → Distribute App を行う
	exit 0
fi

# Xcode archive(build)
xcodebuild -scheme Unity-iPhone archive \
-workspace ${XCODE_PROJECT_DIR}/Unity-iPhone.xcworkspace \
-archivePath ${XCODE_PROJECT_DIR}/Unity-iPhone.xcarchive \
-configuration Release \
PROVISIONING_PROFILE_SPECIFIER="${PROFILE_NAME}" \
DEVELOPMENT_TEAM="${DEV_TEAM}" || exit 1

#PROVISIONING_PROFILE="${PUUID}" \
#CODE_SIGN_IDENTITY="${CODE_SIGN_NAME}" \
#CODE_SIGN_STYLE="Manual" \
#-UseNewBuildSystem=NO \

echo Archive Success

# Xcode : ipaビルド
xcodebuild -exportArchive \
-archivePath ${XCODE_PROJECT_DIR}/Unity-iPhone.xcarchive \
-exportPath ${IPA_DIR} \
-exportOptionsPlist Tools/Build/ExportOptions.plist || exit 1
