#!/bin/bash
SCRIPT_PATH="${BASH_SOURCE[0]}";
if ([ -h "${SCRIPT_PATH}" ]) then
  while([ -h "${SCRIPT_PATH}" ]) do SCRIPT_PATH=`readlink "${SCRIPT_PATH}"`; done
fi

# A trick to resolve the script directory
pushd . > /dev/null
cd `dirname ${SCRIPT_PATH}` > /dev/null
SCRIPT_PATH=`pwd`;
popd  > /dev/null

pushd . > /dev/null
cd ..

if test "$OS" = "Windows_NT"
then
    $SCRIPT_PATH/nuget.exe restore $SCRIPT_PATH/packages.config -PackagesDirectory $SCRIPT_PATH/../packages

    # If updating FAKE version, also update build.bat and build.fsx
    $SCRIPT_PATH/packages/FAKE.4.63.0/tools/FAKE.exe $@ --fsiargs $SCRIPT_PATH/build.fsx
else
    mozroots --import --sync
    mono --runtime=v4.0 $SCRIPT_PATH/nuget.exe restore $SCRIPT_PATH/packages.config -PackagesDirectory $SCRIPT_PATH/../packages

    # If updating FAKE version, also update build.bat and build.fsx
    mono $SCRIPT_PATH/packages/FAKE.4.63.0/tools/FAKE.exe $@ --fsiargs -d:MONO $SCRIPT_PATH/build.fsx
fi

popd > /dev/null
