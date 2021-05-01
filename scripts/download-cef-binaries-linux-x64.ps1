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
$cefLongName = "cef_binary_$($cefVersion)_linux64_client"
$cefBinTarBz2FileName = "$($cefLongName).tar.bz2"
$cefBinTarFileName = "$($cefLongName).tar"
$cefTempDirectory = (Resolve-Path -Path ../src/ThirdParty/Libs/cef/temp/).Path
$cefBinTarBz2FileLocation = "$($cefTempDirectory)$($cefBinTarBz2FileName)"

#Now to download it
Write-Output "Downloading CEF version: $($cefVersion) ..."

#We download the CEF builds off from spotify's build server
#The URL look like this:
#   https://cef-builds.spotifycdn.com/cef_binary_[CEF-VERSION]_linux64_minimal.tar.bz2
#   Example: https://cef-builds.spotifycdn.com/cef_binary_85.3.12+g3e94ebf+chromium-85.0.4183.121_linux64_minimal.tar.bz2

Invoke-WebRequest -Uri "https://cef-builds.spotifycdn.com/$($cefBinTarBz2FileName)" -OutFile $cefBinTarBz2FileLocation

Write-Output "Downloaded CEF to $($cefBinTarBz2FileLocation)"

#Extraction
Write-Output "Extracting files..."

$7zipApp = ""
if($IsLinux)
{
    $7zipApp = "../src/DevTools/7zip/linux-x64/7zz"
}
else
{
    $7zipApp = "../src/DevTools/7zip/win-x64/7za.exe"
}

& "$7zipApp" x $cefBinTarBz2FileLocation "-o$($cefTempDirectory)" *.tar -r -y

$cefBinTarFileLocation = "$($cefTempDirectory)$($cefBinTarFileName)"
& $7zipApp x $cefBinTarFileLocation "-o$($cefTempDirectory)" "$($cefLongName)/" -r -y

#Copy files
Write-Output "Copying files..."

$cefExtractedLocation = (Resolve-Path -Path "$($cefTempDirectory)/$($cefLongName)/").Path
$cefBinReleaseLocation = "$($cefExtractedLocation)Release/"
$cefBinLocation = (Resolve-Path -Path ../src/ThirdParty/Libs/cef/linux-x64).Path

Copy-Item -Path "$($cefBinReleaseLocation)/*" -Destination $cefBinLocation -Force -PassThru -Recurse

Write-Output "Cleaning up..."
Remove-Item -Path $cefTempDirectory -Force -Recurse