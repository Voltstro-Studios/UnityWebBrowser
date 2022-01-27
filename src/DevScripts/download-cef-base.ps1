#This script downloads the CEF binaries, since we can't include them in the repo (They are bigger then your mum surprisingly)
#CefGlue wants the EXACT correct CEF version, so we look at what CEF version CefGlue is targeting and download the right tar.bz2 
#file off from the CEF build server

param
(
    [Parameter(Mandatory=$true)]
    [string] $OperatingSystem,

    [Parameter(Mandatory=$false)]
    [bool] $Cleanup = $true,

    [Parameter(Mandatory=$false)]
    [bool] $IncludeResources = $true
)

function Reset 
{
    #Reset our location
    Pop-Location
}

function CheckProcess
{
    param(
    [Parameter (Mandatory = $true)] [String]$ErrorMessage,
    [Parameter (Mandatory = $true)] [System.Diagnostics.Process]$Process
    )

    if($Process.ExitCode -ne 0) 
    {
        Reset
        throw $ErrorMessage
    }
}

#Set location
Push-Location $PSScriptRoot

#Find what version CefGlue wants
$CefGlueVersionFile = "../ThirdParty/CefGlue/CefGlue/Interop/version.g.cs"

#Check if the version.g.cs file exists, if it doesn't then there is a good chance the user didn't clone the repo recursively, 
#and didn't init the submodules.
if(-not (Test-Path -Path $CefGlueVersionFile))
{
    Write-Warning "The CefGlue version file doesn't exist! Initalizing the submodules for you..."
    Push-Location "$($PSScriptRoot)../../"

    #Run git submodule init and update
    $p = Start-Process git -ArgumentList 'submodule init' -Wait -NoNewWindow -PassThru
    CheckProcess "Error running git submodule init!" $p

    $p = Start-Process git -ArgumentList 'submodule update' -Wait -NoNewWindow -PassThru
    CheckProcess "Error running git submodule update!" $p

    #Return location
    Push-Location $PSScriptRoot
}

$CefGlueVersionfileContent = Get-Content $CefGlueVersionFile
$CefGlueVersionRegex = [regex] 'CEF_VERSION = \"(.*)\"'

if(!$CefGlueVersionfileContent)
{
    Reset
    throw "Failed to read version info!"
}

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

#We download the CEF builds off from Spotify's CEF build server
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
    throw "CEF build failed to download!"
}

Write-Output "Downloaded CEF build to '$($CefBinTarBz2FileLocation)'."
Write-Output "Exracting CEF build..."

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

$7zipApp = (Resolve-Path -Path $7zipApp).Path

#Extract our files
$p = Start-Process $7zipApp -ArgumentList "x $($CefBinTarBz2FileLocation) -o$($TempDirectory) *.tar -r -y" -Wait -NoNewWindow -PassThru
CheckProcess "Extracting failed!" $p

$p = Start-Process $7zipApp -ArgumentList "x $($CefBinTarFileLocation) -o$($TempDirectory) $($CefBinName)/ -r -y" -Wait -NoNewWindow -PassThru
CheckProcess "Extracting failed!" $p

#Setup some variables to using the copying phase
$CefExtractedLocation = (Resolve-Path -Path "$($TempDirectory)/$($CefBinName)/").Path
$CefExtractedReleaseLocation = "$($CefExtractedLocation)Release/"

$CefLibsLocation = (Resolve-Path -Path ../ThirdParty/Libs/cef/$($OperatingSystem)).Path

#Copy files
Write-Output "Copying files..."
Copy-Item -Path "$($CefExtractedReleaseLocation)/*" -Destination $CefLibsLocation -Force -PassThru -Recurse

if($IncludeResources)
{
    $CefExtractedResourcesLocation = "$($CefExtractedLocation)Resources/"
    Copy-Item -Path "$($CefExtractedResourcesLocation)/*" -Destination $CefLibsLocation -Force -PassThru -Recurse
}

Copy-Item -Path "$($CefExtractedLocation)/README.txt" -Destination $CefLibsLocation -Force -PassThru
Copy-Item -Path "$($CefExtractedLocation)/LICENSE.txt" -Destination $CefLibsLocation -Force -PassThru

#Cleanup
if($Cleanup)
{
    Write-Output "Cleaning up..."
    Remove-Item -Path $CefBinTarFileLocation -Force
    Remove-Item -Path $CefBinTarBz2FileLocation -Force
    Remove-Item -Path $CefExtractedLocation -Force -Recurse
}

Reset