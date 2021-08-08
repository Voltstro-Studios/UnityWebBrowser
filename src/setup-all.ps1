Push-Location "$PSScriptRoot/DevScripts/"

& "$PSScriptRoot/DevScripts/download-all.ps1"
& "$PSScriptRoot/DevScripts/publish-all.ps1"
& "$PSScriptRoot/DevScripts/sync-package-all.ps1"

Pop-Location