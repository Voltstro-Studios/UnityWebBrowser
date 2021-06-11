#This script downloads the CEF binaries, since we can't include them in the repo (They are bigger then your mum surprisingly)
#CefGlue wants the EXACT correct CEF version, so we look at what CEF version CefGlue is targeting and download the right tar.bz2 
#file off from the CEF build server

param
(
    [Parameter(Mandatory=$true)]
    [string] $OperatingSystem
)

function Reset 
{
    #Reset our location
    Pop-Location
}

#We will set our location first
Push-Location $PSScriptRoot

#Find what version CefGlue wants
$CefGlueVersionFile = "../ThirdParty/CefGlue/CefGlue/Interop/version.g.cs"
$CefGlueVersionfileContent = Get-Content $CefGlueVersionFile
$CefGlueVersionRegex = [regex] 'CEF_VERSION = \"(.*)\"'

$CefVersion = ""
foreach($Content in $CefGlueVersionfileContent)
{
    $Match = [System.Text.RegularExpressions.Regex]::Match($Content, $CefGlueVersionRegex)
    if($Match.Success) 
    {
        $CefVersion = $Match.groups[1].value
    }
}

#Create a temp directory
#NOTE: We have [void] at the start here so it doens't spew out the logs
[void](New-Item -Path "../ThirdParty/Libs/cef/" -Name "temp" -ItemType "directory" -Force)

#Lots of variables we gonna use
$TempDirectory = (Resolve-Path -Path ../ThirdParty/Libs/cef/temp/).Path
$CefBinName = "cef_binary_$($cefVersion)_$($OperatingSystem)_minimal"
$CefBinTarFileName = "$($CefBinName).tar"
$CefBinTarFileLocation = "$($TempDirectory)$($CefBinTarFileName)"
$CefBinTarBz2FileName = "$($CefBinTarFileName).bz2"
$CefBinTarBz2FileLocation = "$($TempDirectory)$($CefBinTarBz2FileName)"

Write-Output "Downloading CEF version $($CefVersion) for $($OperatingSystem)..."

#We download the CEF builds off from spotify's build server
#The URL look like this:
#   https://cef-builds.spotifycdn.com/cef_binary_[CEF-VERSION]_[OPERATING-SYSTEM]_minimal.tar.bz2
#   Example: https://cef-builds.spotifycdn.com/cef_binary_85.3.12+g3e94ebf+chromium-85.0.4183.121_linux64_minimal.tar.bz2

$progressPreference = 'silentlyContinue'
Invoke-WebRequest -Uri "https://cef-builds.spotifycdn.com/$($CefBinTarBz2FileName)" -OutFile $CefBinTarBz2FileLocation
$progressPreference = 'Continue'

#Check to make sure the file downloaded
if(-not (Test-Path -Path $CefBinTarBz2FileLocation))
{
    Reset
    throw "Cef failed to download!"
}

Write-Output "Exracting CEF bins..."

#Get 7Zip
$7zipApp = ""
if($IsLinux)
{
    $7zipApp = "../DevTools/7zip/linux-x64/7zz"
}
else
{
    $7zipApp = "../DevTools/7zip/win-x64/7za.exe"
}

#Extract our files
& $7zipApp x $CefBinTarBz2FileLocation "-o$($TempDirectory)" *.tar -r -y
& $7zipApp x $CefBinTarFileLocation "-o$($TempDirectory)" "$($CefBinName)/" -r -y

#Setup some variables to using the copying phase
$CefExtractedLocation = (Resolve-Path -Path "$($TempDirectory)/$($CefBinName)/").Path
$CefExtractedReleaseLocation = "$($CefExtractedLocation)Release/"
$CefExtractedResourcesLocation = "$($CefExtractedLocation)Resources/"
$CefLibsLocation = (Resolve-Path -Path ../ThirdParty/Libs/cef/$($OperatingSystem)).Path

#Copy files
Write-Output "Copying files..."
Copy-Item -Path "$($CefExtractedReleaseLocation)/*" -Destination $CefLibsLocation -Force -PassThru -Recurse
Copy-Item -Path "$($CefExtractedResourcesLocation)/*" -Destination $CefLibsLocation -Force -PassThru -Recurse
Copy-Item -Path "$($CefExtractedLocation)/README.txt" -Destination $CefLibsLocation -Force -PassThru
Copy-Item -Path "$($CefExtractedLocation)/LICENSE.txt" -Destination $CefLibsLocation -Force -PassThru

#Cleanup
Write-Output "Cleaning up..."
Remove-Item -Path $CefBinTarFileLocation -Force
Remove-Item -Path $CefBinTarBz2FileLocation -Force
Remove-Item -Path $CefExtractedLocation -Force -Recurse
Reset