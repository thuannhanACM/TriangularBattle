#!/bin/bash

set -eu

#echo "引数 $#"
#exit

PROGNAME=$(basename $0)

usage_exit() {
	echo "Usage: ${PROGNAME} [dev|prod]" 1>&2
	echo "      -h, --help"
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
source ${SCRIPT_DIR}/build_apk.env

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
#GRADLE_PROJECT_DIR=${EXPORT_ROOT}/gradleProject
APK_DIR=${EXPORT_ROOT}/apk
APK_PATH=${APK_DIR}/${PRODUCT_NAME}.apk
APKS_PATH=${APK_DIR}/${PRODUCT_NAME}.apks
APKS_UNZIP_DIR=${APK_DIR}/apks_unzip
AAB_DIR=${EXPORT_ROOT}/aab
AAB_PATH=${AAB_DIR}/${PRODUCT_NAME}.aab

#android apk作成用
BUNDLE_TOOL_PATH=$(find $(dirname ${UNITY_APP})/../../../ -name "bundletool-all*")

echo SCRIPT_DIR:$SCRIPT_DIR
echo WORKSPACE:$WORKSPACE
echo EXPORT_ROOT:$EXPORT_ROOT
#echo GRADLE_PROJECT_DIR:$GRADLE_PROJECT_DIR
echo AAB_DIR:$AAB_DIR
echo APK_DIR:$APK_DIR
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
echo BUNDLE_TOOL_PATH:$BUNDLE_TOOL_PATH

if ! [ "${UNITY_USERNAME:+foo}" ] || ! [ "${UNITY_PASSWORD:+foo}" ]; then
	echo "UNITY_USERNAME($UNITY_USERNAME) と UNITY_PASSWORD($UNITY_PASSWORD) を定義してください"
	exit 1
fi

#exit

mkdir ${EXPORT_ROOT} || true
#rm -rf ${GRADLE_PROJECT_DIR} || true
#mkdir ${GRADLE_PROJECT_DIR} || true
rm -rf ${APK_DIR} || true
mkdir ${APK_DIR} || true
rm -rf ${APKS_UNZIP_DIR} || true
mkdir ${APKS_UNZIP_DIR} || true
mkdir ${AAB_DIR} || true
cp -v ${RSP_FILE} ${WORKSPACE}/Assets/csc.rsp

function force_rebuild(){
	# Force Rebuild (と言いつつ、大抵二回実行しないと切り替わらない)
	rm Library/ScriptAssemblies/* || true
}

function firebase_forceupdatejson(){
	${UNITY_APP} -batchmode -quit -executeMethod CloudBuildExportMethod.ForceUpdateXml \
	-projectPath ${WORKSPACE} \
	-buildTarget android \
	-username ${UNITY_USERNAME} \
	-password ${UNITY_PASSWORD} \
	-logFile -
}

function switch_facebooksdk(){
	# grepに備えて一時的に緩和
	set +e
	grep -e "NO_FACEBOOK" ${SCRIPT_DIR}/dev.rsp
	if [ $? -eq 0 ]; then
		echo found NO_FACEBOOK
		${UNITY_APP} -batchmode -quit -executeMethod CloudBuildExportMethod.DisableFacebookSDK \
		-projectPath ${WORKSPACE} \
		-buildTarget android \
		-username ${UNITY_USERNAME} \
		-password ${UNITY_PASSWORD} \
		-logFile -
	fi
	# 再度厳密に
	set -e
}

function firebase_copy(){
	RETRY=$1
	echo firebase_copy RETRY: ${RETRY}
	force_rebuild
	# FirebaseFiles Copy
	${UNITY_APP} -batchmode -quit -executeMethod CloudBuildExportMethod.CopyFirebaseConfigFiles \
	-projectPath ${WORKSPACE} \
	-buildTarget android \
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
	# prodの場合(このスクリプトはCloudBuildからも呼ばれるため、単純な機構になっている)
	${SCRIPT_DIR}/pre_build_adjust_projectid_prod.sh
else
	# devの場合
	${SCRIPT_DIR}/pre_build_adjust_projectid_dev.sh
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

switch_facebooksdk
#exit

# Unity build(aab)
${UNITY_APP} -batchmode -quit -executeMethod Build.BuildProcess \
-projectPath ${WORKSPACE} \
-buildTarget android \
-username ${UNITY_USERNAME} \
-password ${UNITY_PASSWORD} \
-logFile ${WORKSPACE}/${BUILD_NUMBER}.log \
-productName="${PRODUCT_NAME_FULL}" \
-bundleVersion=${BUNDLE_VERSION} \
-bundleIdentifier=${BUNDLE_IDENTIFIER} \
-developmentBuild=${DEVELOPMENT_BUILD} \
-bundleVersionCode=${BUNDLE_VERSION_INTERNAL} \
-outputPath=${AAB_PATH} || exit 1

# grepに備えて一時的に緩和
set +e

grep -i login ${WORKSPACE}/${BUILD_NUMBER}.log | grep -i fail
if [ $? -ne 1 ]; then
	echo Error!
	exit 1
fi

echo "Unity Build Success(aab)"

# 再度厳密に
set -e

# aabからuniversal設定でapkを吐き出す
java -jar ${BUNDLE_TOOL_PATH} build-apks \
  --bundle="${AAB_PATH}" \
  --output="${APKS_PATH}" \
  --ks="${KEY_STORE_PATH}" \
  --ks-pass="pass:${KEY_STORE_PASS}" \
  --ks-key-alias="${KEY_ALIAS_NAME}" \
  --key-pass="pass:${KEY_ALIAS_PASS}" \
  --mode=universal

unzip "${APKS_PATH}" -d "${APKS_UNZIP_DIR}"
touch "${APKS_UNZIP_DIR}/universal.apk"
mv "${APKS_UNZIP_DIR}/universal.apk" "${APK_PATH}"
rm -rf ${APKS_UNZIP_DIR}
echo "apk success"
