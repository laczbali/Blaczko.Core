# read .env file
Get-Content .env | foreach {
    $name, $value = $_.split('=')
    set-content env:\$name $value
}

# get current version number from csproj
$csproj = Get-Content -Path .\$env:NUGET_PACKAGE_ID.csproj
$versionTag = ($csproj | Select-String -Pattern "<VersionPrefix>.*</VersionPrefix>" | Select-Object -First 1).Line
$versionString = ($versionTag -replace "<VersionPrefix>|</VersionPrefix>").Trim()

$majorVersion = [int]$versionString.Split('.')[0]
$minorVersion = [int]$versionString.Split('.')[1]
$patchVersion = [int]$versionString.Split('.')[2]

# increment and update version number in csproj
$patchVersion = $patchVersion + 1
$versionString = "$majorVersion.$minorVersion.$patchVersion"

$csproj = $csproj -replace "<VersionPrefix>.*</VersionPrefix>", "<VersionPrefix>$versionString</VersionPrefix>"
$csproj | Set-Content -Path .\$env:NUGET_PACKAGE_ID.csproj

# build and publish nuget package
dotnet pack -c Release
dotnet nuget push .\bin\Release\$env:NUGET_PACKAGE_ID.$versionString.nupkg -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json
