#Set location
Push-Location $PSScriptRoot

if(Test-Path -Path "../UnityWebBrowser.UnityProject/Builds/UnityWebBrowser-Quick")
{
    & dotnet build UnityWebBrowser.Docs.sln
}
else
{
    $BinariesDir = Resolve-Path -Path "$($Home)/Binaries/"
    $Env:UWB_UNITY_BINS_PATH = $BinariesDir
    & dotnet build UnityWebBrowser.Docs.sln
}

Pop-Location