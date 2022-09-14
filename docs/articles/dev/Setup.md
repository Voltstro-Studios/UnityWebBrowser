# Setup

Ok, lets setup UWB's development environment.

## Prerequisites

These prerequisites are **mandatory** to compile UWB.

```
Unity 2021.3.x
.NET 6 SDK
PowerShell (formally PowerShell Core)*
Git
```

*[Modern PowerShell](https://github.com/powershell/powershell#get-powershell) is required! The one built into Windows does **NOT** work.

### Additional Optional Prerequisites

These prerequisites are not required, but some areas may require them.

```
NodeJS
Yarn
```

## Repo Setup

We first need to obtain UWB's code. UWB is all contained in one-mono repo, found at `https://github.com/Voltstro-Studios/UnityWebBrowser.git`.

To get the [`UnityWebBrowser`](https://github.com/Voltstro-Studios/UnityWebBrowser) repo, you first need to clone the repo recursively using Git, like so:

```shell
git clone --recursive https://github.com/Voltstro-Studios/UnityWebBrowser.git
```

> [!NOTE]
> If you did NOT clone the repo recursively, you can just init the submodules by running these commands at the root of the repo:
> 
> ```shell
> git submodule init
> git submodule update
> ```

Once you have the repo cloned with the submodules, you must now run the `src/setup-all.ps1` script with PowerShell.

You can go into PowerShell with the command:

```shell
pwsh
```

Once in PowerShell, go to the `src/` directory, and run the `setup-all.ps1` script:

```powershell
./setup-all.ps1
```

Depending on your system, and your download speeds, this script could take upto a minute or longer. You only need to run this once.

You can now open up the `src/UnityWebBrowser.UnityProject` project with Unity.

## Editor Tools

Once in Unity, you can open the provided `UWB` scene provided in the project. By default, this scene is setup to have basic browser controls/window.

When running the project in this scene, a provided 'UWB Debug UI' will be available.

> [!NOTE]
> By default this is "hidden", you can open the UI via the small panel at the top of the player's window.
> ![Panel](~/assets/images/articles/dev/setup/panel.webp)

The 'UWB Debug UI' provided has some useful stats and controls that you may want to use.

![Debug UI](~/assets/images/articles/dev/setup/debug-ui.webp)

If you need to, extra controls can be added by modifying the `Assets/Scripts/UWBPrjDebugUI.cs` script.

(In the future we hope to develop more editor tools to make life easier.)

## Dev Scripts

There a many dev scripts in the `src/DevScripts` directory. The main ones that you will most likely use are:

- `download-cef-<OS>.ps1`
- `publish-<Section>-<OS>.ps1`
