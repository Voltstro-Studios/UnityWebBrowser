# Setup and build pages project first
Push-Location "$PSScriptRoot/UnityWebBrowser.Pages/"
& yarn install
& yarn run build

# Run Scripts
Push-Location "$PSScriptRoot/DevScripts/"

& "$PSScriptRoot/DevScripts/download-all.ps1"
& "$PSScriptRoot/DevScripts/publish-all.ps1"
& "$PSScriptRoot/DevScripts/sync-package-all.ps1"

Pop-Location