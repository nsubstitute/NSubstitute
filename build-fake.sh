"ThirdParty/NuGet/nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "ThirdParty/FAKE" "-ExcludeVersion" "-version" "2.6.0"

"ThirdParty/FAKE/FAKE.Core/tools/Fake.exe" "build.fsx" $*
