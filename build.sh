"build/nuget.exe" restore "build/packages.config" -PackagesDirectory "packages"
# If changing the FAKE version here, make sure the scripts in build/ are also
# updates (build.bat, build.sh)
"packages/FAKE.4.63.0/tools/Fake.exe" "build/build.fsx" $*
