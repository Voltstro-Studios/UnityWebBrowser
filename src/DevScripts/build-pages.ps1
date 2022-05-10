Push-Location "$PSScriptRoot/../UnityWebBrowser.Pages/"

& yarn
& yarn run build

Pop-Location 
