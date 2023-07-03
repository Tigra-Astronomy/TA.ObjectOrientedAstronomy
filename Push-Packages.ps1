# Push all packages from release builds to Tigra Astronomy MyGet feed.
# Assumes that the API key for the relevant feeds has been installed in NuGet.
# Searches the current directory and child directories recursively.

param (
    [string]$ApiKey = "231f5f9d-d1ee-4445-a9e0-5d7eafecbc46"
)

$packageFeed = "https://www.myget.org/F/tigra-astronomy/api/v2/package"
$symbolFeed = "https://www.myget.org/F/tigra-astronomy/api/v3/index.json"

$allPackages = Get-ChildItem -Recurse | Where-Object { $_.Name -match '^.*\.nupkg$' }
$releasePackages = $allPackages | Where-Object { $_.DirectoryName -match 'Release' }

if ($releasePackages) {
    $releasePackages | Format-List -Property Name | Write-Output
}
else {
    Write-Host "No release packages found"
    Exit
}

foreach ($package in $releasePackages) {
    NuGet.exe push $package -Source $packageFeed -SymbolSource $symbolFeed -ApiKey $ApiKey -SymbolApiKey $ApiKey -NonInteractive -SkipDuplicate
}
