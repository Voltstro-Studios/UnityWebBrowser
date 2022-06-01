# Version Check
if ($PSVersionTable.PSVersion.Major -lt 7)
{
    throw "You need to use the NEW PowerShell version! You can get it here: https://github.com/powershell/powershell#get-powershell"
}

# Run Scripts
Push-Location "$PSScriptRoot/DevScripts/"

& "$PSScriptRoot/DevScripts/download-all.ps1"
& "$PSScriptRoot/DevScripts/publish-all.ps1"
& "$PSScriptRoot/DevScripts/sync-package-all.ps1"

Pop-Location