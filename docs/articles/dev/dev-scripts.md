# Dev Scripts

UWB contains all of its dev scripts in `src/DevScripts/`. Dev scripts are used for automating tasks like downloading external files or building projects.

## Usage

There are three main scripts you will want to use.

1. `download_cef_<platform>.py`
   - Used for downloading the required version of CEF and extracting it.
2. `build_cef_<platform>.py`
   - Builds the `UnityWebBrowser.Engine.Cef` project in publish mode and outputs the built files into `src/Packages/UnityWebBrowser.Engine.Cef.<Platform>/Engine~/`. Needs to be ran if the CEF engine project has any changes made to it.
3. `build_shared.py`
   - Builds the `VoltstroStudios.UnityWebBrowser.Shared` project in `ReleaseUnity` configuration mode, which will place the built files into `src/Packages/UnityWebBrowser/Plugins`. Needs to be ran if the shared project has any changes made to it.

## Base Scripts

There are a couple of scripts with `_base` in it's name. These scripts are shared modules that are used by the scripts that the user executes.

## Why Python

In older versions of UWB, we used PowerShell as the language of choice for our dev scripts. PowerShell has one main advantage, its cross-platform. But so is Python. Python's runtime also includes a lot of useful utilities like archive extraction. While in PowerShell technically this is possible (you can call .NET methods), its a bit of a pain to do.
