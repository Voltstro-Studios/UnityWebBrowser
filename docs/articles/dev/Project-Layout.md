# Project Layout

Lets explain the layout of the project. UWB is one big mono-repo. Everything you need (other then tools) are contained within the repo. Yes, it means it is a large repo, but it so much easier to develop for.

## Repo Layout

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
  - UnityWebBrowser.Pages/
  - UnityWebBrowser.Engine.*/
  - UnityWebBrowser.UnityProject/
  - VoltstroStudios.*/
  - *Misc Stuff*
```

- `docs/`, `media/` and `src/` are all fairly explanatory.

- `DevScripts/` and `DevTools/` provide developer scripts and other external applications that are needed.

- `Imports/` contains shared `.targets` and `.props` files that are used by the .NET projects.

- `Packages/` contains the UPM packages. The provided Unity project has all packages added locally.

- `ThirdParty/` contains stuff that ain't ours, and cannot be included by a package management system.

The rest of the folders contain dotnet projects, with their respected code.

## Projects

Quick description of all the individual projects.

### UnityWebBrowser.UnityProject

This is the provided Unity project that can be used for UWB development. It has all of UWB's packages included locally.

### UnityWebBrowser.Pages

NodeJs/Yarn project that used to build out HTML that engines use for internal pages.

### VoltstroStudios.UnityWebBrowser.Shared

.NET project that contains shared code that used by both Engines and Core.

### VoltstroStudios.UnityWebBrowser.Engine.Shared

.NET project that contains shared code that is used by Engines.

### UnityWebBrowser.Engine.Cef

UWB's CEF Engine. .NET project that is built out as an application. In publish mode it will build the app into a single file.
