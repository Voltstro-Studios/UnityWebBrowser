#Script to prepare needed dlls for docfx V3

function ErrorExit
{
    param(
    [Parameter (Mandatory = $true)] [String]$ErrorMessage
    )

    #Reset our location
    Pop-Location

    throw $ErrorMessage;
}

#Set location
Push-Location $PSScriptRoot

if (!$Env:UWB_GH_TOKEN)
{
    ErrorExit "The env variable 'UWB_GH_TOKEN' needs to be set to a GitHub token with 'artifact:read' scope!";
}

#Headers that will be used by all requests
$requestHeaders = @{
    "Accept" = "application/vnd.github+json";
    "Authorization" = "Bearer $($Env:UWB_GH_TOKEN)"
};

#Get all artifacts
Write-Host "Getting artifacts..."
$response = Invoke-WebRequest -Headers $requestHeaders -Uri https://api.github.com/repos/Voltstro-Studios/UnityWebBrowser/actions/artifacts

#Check for successful response
if($response.StatusCode -ne 200)
{
    ErrorExit "Response for artifacts returned an error code $($response.StatusCode)!"
}

#Get location of latest artifact
$artifacts = $response | ConvertFrom-Json

$latestArtifact;
foreach ($artifact in $artifacts.artifacts)
{
    if ($artifact.name = "UWBLibs")
    {
        $latestArtifact = $artifact;
        break;
    }
}

#Download latest artifact
if(-not (Test-Path -Path "obj/"))
{
    #NOTE: We have [void] at the start here so it doens't spew out the logs
    [void](New-Item -Path $PSScriptRoot -Name "obj/" -ItemType "directory" -Force)
}

Write-Host "Downloading latest artifact..."
$progressPreference = 'silentlyContinue'
Invoke-WebRequest -Headers $requestHeaders -Uri $latestArtifact.archive_download_url -OutFile "obj/UWBLibs.zip"

#Extract
Write-Host "Extracting..."
Expand-Archive -LiteralPath "obj/UWBLibs.zip" -DestinationPath "obj/Libs" -Force

#Reset
$progressPreference = 'Continue'
Pop-Location
