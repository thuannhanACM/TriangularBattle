#!/bin/bash

set -eu

#echo "引数 $#"
#exit

PROGNAME=$(basename $0)

usage_exit() {
	echo "Usage: ${PROGNAME} aabpath" 1>&2
	echo "      -h, --help"
	exit 1
}

for OPT in "$@"
do
	case $OPT in
		--help | -h)
			usage_exit
			;;
	esac
done

echo $*

SCRIPT_DIR=$(cd $(dirname $0); pwd)
#source ${SCRIPT_DIR}/unity.env
source ${SCRIPT_DIR}/build_com.env
source ${SCRIPT_DIR}/build_apk.env

# jenkinsから呼ぶならコメントアウト
WORKSPACE=$(cd $(dirname ${SCRIPT_DIR}/../../../); pwd)

# 出力先
AAB_PATH=${1}
PRODUCT_NAME=$(basename ${AAB_PATH})
EXPORT_ROOT_PARENT=${WORKSPACE}/export
EXPORT_ROOT=${EXPORT_ROOT_PARENT}/convert
APK_DIR=${EXPORT_ROOT}/apk
APK_PATH=${APK_DIR}/${PRODUCT_NAME}.apk
APKS_PATH=${APK_DIR}/${PRODUCT_NAME}.apks
APKS_UNZIP_DIR=${APK_DIR}/apks_unzip

mkdir ${EXPORT_ROOT_PARENT} || true
mkdir ${EXPORT_ROOT} || true
mkdir ${APK_DIR} || true
mkdir ${APKS_UNZIP_DIR} || true

#android apk作成用
BUNDLE_TOOL_PATH=$(find $(dirname ${UNITY_APP})/../../../ -name "bundletool-all*")

echo SCRIPT_DIR:$SCRIPT_DIR
echo WORKSPACE:$WORKSPACE
echo EXPORT_ROOT:$EXPORT_ROOT
#echo GRADLE_PROJECT_DIR:$GRADLE_PROJECT_DIR
echo APK_DIR:$APK_DIR
echo BUNDLE_TOOL_PATH:$BUNDLE_TOOL_PATH


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
mv "${APKS_UNZIP_DIR}/universal.apk" "${APK_PATH}"
rm -rf ${APKS_UNZIP_DIR}
echo "apk success"
