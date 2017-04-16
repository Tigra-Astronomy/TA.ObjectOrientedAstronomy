$rootNamespace = "TA.ObjectOrientedAstronomy."
$output = "NuGet-Packages"
$projects_to_pack = "FundamentalTypes", "OrbitEngines", "FlexibleImageTransportSystem"
$nuget = "nuget.exe"
$apiKey = "a0482288-35f2-49cf-a252-b22b11753f8b"

# Ensure that the output directory exists
If(!(test-path $output))
    {
    New-Item -ItemType Directory -Force -Path $output
    }

# Rebuild the solution Release configuration, Any CPU platform

# Invoke-MSBuild -configuration Release -platform "Any CPU" -targets Rebuild

# Package each project

foreach ($package in $projects_to_pack)
{
    $projectName = $rootNamespace + $package
    $projectFile = $projectName + ".csproj"
    $projectLocation = [System.IO.Path]::Combine($projectName, $projectFile)
    &$nuget Pack $projectLocation -OutputDirectory $output -Symbols -Properties Configuration=Release
}
