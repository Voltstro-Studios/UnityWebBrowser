# Project Layout

Lets explain the layout of the project.

This is the basic layout of repo:

```
docs/
media/
src/
  - DevScripts/
  - DevTools/
  - Docs/
  - Imports/
  - Packages/
      - UnityWebBrowser/
      - *Other UPM packages*/
  - ThirdParty/
  - UnityWebBrowser.Pages
  - UnityWebBrowser.Engine.*/
  - UnityWebBrowser.UnityProject/
  - VoltstroStudios.*/
  - *Misc Stuff*
```

`docs/`, `media/` and `src/` are all fairly explanatory.

`DevScripts/` and `DevTools/` provide developer scripts and other external applications that are needed.

`Docs/` provide a way of building the UWB package as a dll, so docfx can use it.

`Imports/` contains shared `.targets` and `.props` files.

`Packages/` contains the UPM packages.

`ThirdParty/` contains stuff that ain't ours.

The rest of the folders contain dotnet projects, with their respected code.
