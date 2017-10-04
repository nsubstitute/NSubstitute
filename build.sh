"build/nuget.exe" restore "build/packages.config" -PackagesDirectory "packages"
"packages/FAKE.4.63.0/tools/Fake.exe" "build/build.fsx" $*
