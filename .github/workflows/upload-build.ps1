git clone https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git
Copy-Item -Path UnityWebBrowserSource/src/CefBrowser/ -Destination Package/ -Recurse -Force -PassThru

Copy-Item -Path Linux-CefBrowserProcess/ -Destination Package/Plugins/CefBrowser/linux-x64/ -Force -Recurse -PassThru
Copy-Item -Path Windows-CefBrowserProcess/ -Destination Package/Plugins/CefBrowser/win-x64/ -Force -Recurse -PassThru
Remove-Item -Path Package/Plugins/CefBrowser/.gitignore -Force

Set-Location -path "Package/"
git config --global user.name "Voltstro"
git config --global user.email "me@voltstro.dev"
git remote set-url origin git@gitlab.com:Voltstro-Studios/WebBrowser/Package.git
git add *
git commit -m "Update"
git push