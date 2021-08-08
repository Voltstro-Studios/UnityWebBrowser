$LicensePath = "../../LICENSE.md"
Copy-Item -Path $LicensePath -Destination "../Packages/UnityWebBrowser.Engine.Cef/LICENSE.md" -Force -PassThru
Copy-Item -Path $LicensePath -Destination "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/LICENSE.md" -Force -PassThru
Copy-Item -Path $LicensePath -Destination "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/LICENSE.md" -Force -PassThru

#Find what version of CefGlue we are using
$CefGlueVersionFile = "../ThirdParty/CefGlue/CefGlue/Interop/version.g.cs"
$CefGlueVersionfileContent = Get-Content $CefGlueVersionFile
$CefGlueVersionRegex = [regex] 'CEF_VERSION = \"(\d+.\d+.\d+)'

$CefVersion = ""
foreach($Content in $CefGlueVersionfileContent)
{
    $Match = [System.Text.RegularExpressions.Regex]::Match($Content, $CefGlueVersionRegex)
    if($Match.Success) 
    {
        $CefVersion = $Match.groups[1].value
    }
}

$EngineCefJsonPath = "../Packages/UnityWebBrowser.Engine.Cef/package.json"
$EngineCefJson = Get-Content $EngineCefJsonPath | ConvertFrom-Json -AsHashtable
$EngineCefJson["version"] = $CefVersion
$EngineCefJson["dependencies"]["dev.voltstro.unitywebbrowser.engine.cef.linux.x64"] = $CefVersion
$EngineCefJson["dependencies"]["dev.voltstro.unitywebbrowser.engine.cef.win.x64"] = $CefVersion
$EngineCefJson | ConvertTo-Json | Out-File -FilePath $EngineCefJsonPath

$EngineCefLinuxJsonPath = "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/package.json"
$EngineCefLinuxJson = Get-Content $EngineCefLinuxJsonPath | ConvertFrom-Json -AsHashtable
$EngineCefLinuxJson["version"] = $CefVersion
$EngineCefLinuxJson | ConvertTo-Json | Out-File -FilePath $EngineCefLinuxJsonPath

$EngineCefWinJsonPath = "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/package.json"
$EngineCefWinJson = Get-Content $EngineCefWinJsonPath | ConvertFrom-Json -AsHashtable
$EngineCefWinJson["version"] = $CefVersion
$EngineCefWinJson | ConvertTo-Json | Out-File -FilePath $EngineCefWinJsonPath