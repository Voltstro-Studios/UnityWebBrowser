# Unity Web Browser DocsPrj

This project is designed to build out a dll that can be used by UWB's docs (which uses docfx v3).

As all that is really cared about is the XML generated docs, and the publicly exposed methods.

## Building

The Unity dlls are required for a successful build, they will be located by either:

- Using the Unity dlls in the compiled UnityWebBrowser.UnityProject player build (use Unity Volt Builder to build so it is placed in the correct location)
- Or by getting the dlls from `~/Binaries/`.

When you have dealt with that, run the `build.ps1` script with PowerShell.
