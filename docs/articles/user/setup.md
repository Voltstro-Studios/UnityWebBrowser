# Setup 

Welcome! To start using UWB in your project, there is first some initial setup that needs to be done first.

## Prerequisites

```
Unity 2021.3.x
```

Newer Unity versions should work, but are untested!

## Platform Support

At a base level, UWB supports all major desktop platforms (Windows, Linux and MacOS). However, different engines may have different platform support. Platform different engines support are listed in the [engines section](engines.md).

> [!WARNING]
> UWB does **NOT** support [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html). This is because UWB requires launching a separate process, which uses System.Diagnostics.Process API that [IL2CPP doesn't support](https://docs.unity3d.com/2021.3/Documentation/Manual/ScriptingRestrictions.html).
>
> UWB does however support being code trimmed.

## VoltUPM Setup

VoltUPM is a Unity registry that we provide for hosting some of our packages, including all of UWB's packages. To use it, your project needs to be configured to use the VoltUPM registry.

To setup the registry with your project, [see here](https://github.com/Voltstro/VoltstroUPM#setup). The VoltUPM page also lists some other info that you may be interested in.

**HOWEVER**, an additional scope needs to be added. You need to make sure `com.cysharp.unitask` is added (more details are provided in the [UniTask part](#unitask)). Once you are done configuring your projects registries, your configuration should look like:

![Registry](~/assets/images/articles/user/setup/Registry.webp)

> [!NOTE]
> If you are using [UnityNuGet](https://github.com/xoofx/UnityNuGet), and you choose not to use VoltUPM as a `org.nuget.*` scope mirror, then don't have the `org.nuget` scope defined for VoltUPM.

### UniTask

The reason why we need to add the additional `com.cysharp.unitask` scope to VoltUPM is because UWB depends on [UniTask](https://github.com/Cysharp/UniTask). VoltUPM does provide a mirror copy of UniTask (from OpenUPM),
however you may already have UniTask installed either via [OpenUPM](https://openupm.com/packages/com.cysharp.unitask/), or via [Git](https://github.com/Cysharp/UniTask#install-via-git-url). If you do have it installed already,
and you don't want to use VoltUPM's mirror of it, then DO NOT define the additional scope as apart of VoltUPM.

> [!WARNING]
> If you already have UniTask installed via Git, please make sure it is the latest version!

## Packages Installation

Once you have VoltUPM registry added to your project, you can install the packages via the Unity package manager GUI.

![Packages](~/assets/images/articles/user/setup/Packages.webp)

To know what packages you need, checkout the [packages section](packages.md).

### Standard Loadout

Most developers will probably only need the 'Core' package, and a single engine with it's native Windows binaries. A basic installation might look like this.

![Packages](~/assets/images/articles/user/setup/StandardPackages.webp)

If you plan on providing builds of your game for Linux and MacOS, install their native engine packages as well.

## Usage

Once you have all the required packages that you might need, you can move onto the [usage section](usage.md).
