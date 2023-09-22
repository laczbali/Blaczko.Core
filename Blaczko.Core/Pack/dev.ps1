Set-Location ..\

# get latest dev package version
$outputDir = ".\bin\Debug"
$latestFile = Get-ChildItem -Path $outputDir -Filter "*dev*.nupkg" | Select-Object -Last 1
if ($null -ne $latestFile) {
    $devNumber = $latestFile.name -replace ".*dev(\d+).*", '$1'
    $devNumber = [int]$devNumber + 1
}
else {
    $devNumber = 1
}

# build nuget package
dotnet pack -c Debug --version-suffix dev$devNumber --output $outputDir