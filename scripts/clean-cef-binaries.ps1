Write-Output "Cleaning..."

$cefLibsDir = (Resolve-Path -Path "../src/ThirdParty/Libs/cef/").Path
Remove-Item -Path "$($cefLibsDir)/*" -Force -Recurse -Exclude ".gitignore"