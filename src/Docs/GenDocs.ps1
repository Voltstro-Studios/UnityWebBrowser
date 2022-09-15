if (-Not (Test-Path "Out/")) {
    New-Item -Path "Out/" -ItemType Directory
}

if (-Not (Test-Path "Out/UnityWebBrowser/")) {
    New-Item -Path "Out/UnityWebBrowser/" -ItemType Directory
}

& xmldocmd "UnityWebBrowser.DocsPrj/bin/Debug/netstandard2.1/UnityWebBrowser.dll" "Out/UnityWebBrowser/"

if (-Not (Test-Path "Out/UnityWebBrowser.Shared/")) {
    New-Item -Path "Out/UnityWebBrowser.Shared/" -ItemType Directory
}

& xmldocmd "UnityWebBrowser.DocsPrj/bin/Debug/netstandard2.1/UnityWebBrowser.Shared.dll" "Out/UnityWebBrowser.Shared/"