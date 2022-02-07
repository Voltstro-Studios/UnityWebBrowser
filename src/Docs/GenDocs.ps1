if (-Not (Test-Path "Out/")) {
    New-Item -Path "Out/" -ItemType Directory
}

& xmldocmd "UnityWebBrowser.DocsPrj/bin/Debug/netstandard2.1/UnityWebBrowser.dll" "Out/"
