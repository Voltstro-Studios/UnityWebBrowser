#This script downloads the CEF binaries, since we can't include them in the repo (They are bigger then your mum surprisingly)
#CefGlue wants the EXACT correct CEF version, so we look at what CEF version CefGlue is targeting and download the right tar.bz2 file off from spotify

#Find what version CefGlue wants
$cefGlueVersionFile = "../src/ThirdParty/CefGlue/CefGlue/Interop/version.g.cs"
$cefGlueVersionfileContent = Get-Content $cefGlueVersionFile
$regularExpression = [regex] 'CEF_VERSION = \"(.*)\"'

$cefVersion = ""
foreach($content in $cefGlueVersionfileContent)
{
    $match = [System.Text.RegularExpressions.Regex]::Match($content, $regularExpression)
    if($match.Success) 
    {
        $cefVersion = $match.groups[1].value
    }
}

#Create a temp directory
New-Item -Path "../src/ThirdParty/Libs/cef/" -Name "temp" -ItemType "directory" -Force

#Some variables that we will use
$cefBinTarBz2FileName = "cef_binary_$($cefVersion)_windows64_minimal.tar.bz2"
$cefBinTarFileName = "cef_binary_$($cefVersion)_windows64_minimal.tar"
$cefTempDirectory = (Resolve-Path -Path ../src/ThirdParty/Libs/cef/temp/).Path
$cefBinTarBz2FileLocation = "$($cefTempDirectory)$($cefBinTarBz2FileName)"

#Now to download it
Write-Output "Downloading CEF version: $($cefVersion) ..."

#We download the CEF builds off from spotify's build server
#The URL look like this:
#   https://cef-builds.spotifycdn.com/cef_binary_[CEF-VERSION]_windows64_minimal.tar.bz2
#   Example: https://cef-builds.spotifycdn.com/cef_binary_85.3.12+g3e94ebf+chromium-85.0.4183.121_windows64_minimal.tar.bz2

#Invoke-WebRequest -Uri "https://cef-builds.spotifycdn.com/$($cefBinTarBz2FileName)" -OutFile $cefBinTarBz2FileLocation

Write-Output "Downloaded CEF to $($cefBinTarBz2FileLocation)"

#Extraction
Write-Output "Extracting files..."

#TODO: Linux has built in tar extraction stuff (I think)
& ../src/devtools/7zip/win-x64/7za.exe x $cefBinTarBz2FileLocation "-o$($cefTempDirectory)" *.* -r -y

$cefBinTarFileLocation = "$($cefTempDirectory)$($cefBinTarFileName)"
& ../src/devtools/7zip/win-x64/7za.exe x $cefBinTarFileLocation "-o$($cefTempDirectory)" *.* -r -y

#Copy files
Write-Output "Copying files..."

$cefBinReleaseLocation = (Resolve-Path -Path "$($cefTempDirectory)/cef_binary_$($cefVersion)_windows64_minimal/Release/").Path
$cefBinResourcesLocation = (Resolve-Path -Path "$($cefTempDirectory)/cef_binary_$($cefVersion)_windows64_minimal/Resources/").Path
$cefBinLocation = (Resolve-Path -Path ../src/ThirdParty/Libs/cef/).Path

Get-ChildItem -Path "$($cefBinReleaseLocation)*.dll" | Copy-Item -Destination $cefBinLocation
Get-ChildItem -Path "$($cefBinReleaseLocation)*.bin" | Copy-Item -Destination $cefBinLocation

Copy-Item -Path $cefBinResourcesLocation -Destination $cefBinLocation -Recurse -Force