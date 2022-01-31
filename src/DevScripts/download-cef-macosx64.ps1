& "$PSScriptRoot/download-cef-base.ps1" macosx64 -IncludeResources $false

Rename-Item "../ThirdParty/Libs/cef/macosx64/Chromium Embedded Framework.framework/Chromium Embedded Framework" "libcef.dylib"

Write-Output "Done!"