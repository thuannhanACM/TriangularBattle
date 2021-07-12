#!/bin/bash

set -eu

mkdir tmp || true
mkdir tmp/firebase_import || true

SCRIPT_DIR=$(cd $(dirname $0); pwd)
source ${SCRIPT_DIR}/firebase.env
source ${SCRIPT_DIR}/../Build/build_com.env
WORKSPACE=$(cd $(dirname ${SCRIPT_DIR}/../../../); pwd)
cp -v ${SCRIPT_DIR}/csc.rsp ${WORKSPACE}/Assets/csc.rsp


curl https://dl.google.com/firebase/sdk/unity/firebase_unity_sdk_${FIREBASE_VER}.zip -o tmp/firebase_import/firebase_unity_sdk_${FIREBASE_VER}.zip

unzip -o tmp/firebase_import/firebase_unity_sdk_${FIREBASE_VER}.zip firebase_unity_sdk/dotnet4/FirebaseAnalytics.unitypackage firebase_unity_sdk/dotnet4/FirebaseRemoteConfig.unitypackage

${UNITY_APP} \
 -batchmode -quit -nographics \
 -projectPath ${WORKSPACE} \
 -importPackage firebase_unity_sdk/dotnet4/FirebaseAnalytics.unitypackage \
 -logFile log1.log || echo "ignore: 'Scripts have compiler errors.' "

${UNITY_APP} \
 -batchmode -quit -nographics \
 -projectPath ${WORKSPACE} \
 -importPackage firebase_unity_sdk/dotnet4/FirebaseRemoteConfig.unitypackage \
 -logFile log3.log 

rm -fv ${WORKSPACE}/Assets/csc.rsp
echo "Done."
