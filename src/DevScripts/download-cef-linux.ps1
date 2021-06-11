#Run the base script first
& "$PSScriptRoot/download-cef-base.ps1" linux64

#"strip" libs of their debug symbols
if($IsLinux)
{
    Write-Output "Stipping Linux CEF libs from their debug symbols..."
    Get-ChildItem -Path "../ThirdParty/Libs/cef/linux64/*.so" -Recurse | % { & strip "$($_.FullName)" }
}
else
{
    Write-Warning "Not striping Linux CEF libs. You need to be running under Linux for that."
}

Write-Output "Done!"