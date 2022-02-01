& "$PSScriptRoot/download-cef-base.ps1" macosx64 -IncludeResources $false

Move-Item "../ThirdParty/Libs/cef/macosx64/Chromium Embedded Framework.framework/Chromium Embedded Framework" "../ThirdParty/Libs/cef/macosx64/Chromium Embedded Framework.framework/libcef.dylib" -Force

Write-Output "Done!"