#!/bin/bash
SCRIPT_PATH="${BASH_SOURCE[0]}";
if ([ -h "${SCRIPT_PATH}" ]) then
  while([ -h "${SCRIPT_PATH}" ]) do SCRIPT_PATH=`readlink "${SCRIPT_PATH}"`; done
fi
pushd . > /dev/null
cd `dirname ${SCRIPT_PATH}` > /dev/null
SCRIPT_PATH=`pwd`;
popd  > /dev/null

TARGET=${1:-Default}
CONFIGURATION=${2:-Debug}

mozroots --import --sync
mono --runtime=v4.0 $SCRIPT_PATH/nuget.exe restore $SCRIPT_PATH/packages.config -PackagesDirectory $SCRIPT_PATH/../packages

mono $SCRIPT_PATH/../packages/FAKE.4.63.0/tools/FAKE.exe $TARGET --envvar "configuration" $CONFIGURATION --fsiargs -d:MONO $SCRIPT_PATH/build.fsx