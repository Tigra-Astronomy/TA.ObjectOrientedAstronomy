# Push all packages from release builds to Tigra Astronomy MyGet feed.
# Assumes that the API key for the relevant feeds has been installed in NuGet.
# Searches the current directory and child directories recursively.

param (
    [string]$ApiKey = $null
)

if ($ApiKey) { $setApiKey = "-ApiKey " + $ApiKey }

$packageFeed = "https://www.myget.org/F/tigra-astronomy/api/v2/package"
$symbolFeed = "https://www.myget.org/F/tigra-astronomy/api/v3/index.json"

$allPackages = Get-ChildItem -Recurse | Where-Object { $_.Name -match '^.*\.s?nupkg$' }
$releasePackages = $allPackages | Where-Object { $_.DirectoryName -match 'Release' }

if ($releasePackages) {
    $releasePackages | Format-Table | Write-Output
}
else {
    Write-Host "No release packages found"
    Exit
}

foreach ($package in $releasePackages) {
    if ($package.Name -like "*.snupkg") {
        NuGet.exe push -Source $symbolFeed $package $setApiKey
    }
    else {
        NuGet.exe push -Source $packageFeed $package $setApiKey
    }
}
