#!/bin/bash

set -eu

SCRIPT_DIR=$(cd $(dirname $0); pwd)
source ${SCRIPT_DIR}/build_prod.env

WORKSPACE=$(cd $(dirname ${SCRIPT_DIR}/../../../); pwd)

env

echo "Before adjust"
grep -i -e "project" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset

# replace cloud project
sed -i '' -e "s/cloudProjectId: .*$/cloudProjectId: ${UNITY_PROJECT_ID}/g" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset
sed -i '' -e "s/projectName: .*$/projectName: ${UNITY_PROJECT_NAME}/g" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset

echo "After adjust"
grep -i -e "project" ${WORKSPACE}/ProjectSettings/ProjectSettings.asset

# Copy Filebase Config files
ls -l ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info-prod.plist* ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info.plist*
cp -v ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info-prod.plist ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info.plist
cp -v ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info-prod.plist.meta ${WORKSPACE}/Assets/FirebaseConfig/GoogleService-Info.plist.meta
ls -l ${WORKSPACE}/Assets/FirebaseConfig/google-services-prod.json* ${WORKSPACE}/Assets/FirebaseConfig/google-services.json*
cp -v ${WORKSPACE}/Assets/FirebaseConfig/google-services-prod.json ${WORKSPACE}/Assets/FirebaseConfig/google-services.json
cp -v ${WORKSPACE}/Assets/FirebaseConfig/google-services-prod.json.meta ${WORKSPACE}/Assets/FirebaseConfig/google-services.json.meta

# remove desktop(pc) config file
rm -v ${WORKSPACE}/Assets/StreamingAssets/google-services-desktop.json* || true

# convert firebase config json to xml
python  ${WORKSPACE}/Assets/Firebase/Editor/generate_xml_from_google_services_json.py -i ${WORKSPACE}/Assets/FirebaseConfig/google-services.json -o ${WORKSPACE}/Assets/Plugins/Android/FirebaseApp.androidlib/res/values/google-services.xml -p "${BUNDLE_IDENTIFIER}"


