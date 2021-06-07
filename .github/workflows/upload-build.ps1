#Clone the repo
git clone https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git

#Detect if there is a change in the package.json files
if(Compare-Object -ReferenceObject $(Get-Content UnityWebBrowserSource/src/UnityWebBrowser/package.json) -DifferenceObject $(Get-Content Package/package.json))
{
    Write-Host "Changes in package.json, deploying..."

    #Remove everything excet the git folder to start from a clean state
    Remove-Item -Recurse Package/* -Exclude .git

    #Lots of copying
    Get-ChildItem -Path UnityWebBrowserSource/src/UnityWebBrowser/ | Copy-Item -Destination Package/ -Recurse -Container -Force -PassThru

    Get-ChildItem -Path Linux-CefBrowserProcess/ | Copy-Item -Destination Package/Plugins/CefBrowser/linux-x64 -Recurse -Container -Force -PassThru
    Get-ChildItem -Path Windows-CefBrowserProcess/ | Copy-Item -Destination Package/Plugins/CefBrowser/win-x64 -Recurse -Container -Force -PassThru

    Remove-Item -Path Package/Plugins/CefBrowser/.gitignore -Force

    #Push the changes with SSH
    Set-Location -path "Package/"
    git config --global user.name "Voltstro"
    git config --global user.email "me@voltstro.dev"
    git remote set-url origin git@gitlab.com:Voltstro-Studios/WebBrowser/Package.git
    git add *
    git commit -m "Update"
    git push
}
else 
{
    Write-Host "No changes in package.json."
}