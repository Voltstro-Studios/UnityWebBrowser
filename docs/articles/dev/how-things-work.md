# How Things Work?

So, lets talk about how UWB works internally.

UWB is split into 3 sections: 

- Core (Unity side)
- Shared (Glues the two sections together)
- Engine (Externally ran process)

## The Three Sections

### Core

The core is responsible for anything Unity related (such as handling getting Input, or rendering to a <xref:UnityEngine.Texture>). It is provided as a [custom UPM package](https://docs.unity3d.com/Manual/CustomPackages.html).

### Shared

The shared library provides the abstraction layer that is used by the two sides.

There is also a shared library for engines, which provides some shared functions that only engines need.

### Engine

The engine is an external process that the core starts up. The engine is what runs the actual web browser, and is responsible for externally managing it outside of Unity.

Since the engine is an external process, it has it own set of application files. This application files get packaged into Unity packages that can also be installed via UPM.

## Communication

Ok, so we got the external engine process, and the core (Unity) process. We also have the shared abstraction layer between the two. But how do these things talk to each other.

Communication is done via [RPC](https://en.wikipedia.org/wiki/Remote_procedure_call)s. We mainly uses RPCs instead of just directly writing data to an IPC pipe because using RPCs make programming and maintaining **a lot** easier. Under the hood, we use our own RPC library called [VoltRpc](https://github.com/Voltstro-Studios/VoltRpc).

## Why not just run the web browser inside of Unity?

Well technically you *could*, but there are two major issues:

1. Modern web browsers run multiple processes them selves, with each process having its own dedicated task. They often launch these process from their own original application file. In the Unity editor that is essentially impossible, and with the Unity player it would mean having the overhead of Unity for every single sub-process.
2. Most web browser are programmed with running one instance of them selves once, and if the browser needs to shutdown, then it probably means the user has requested to close the app. So the developers making web browsers under that assumption don't cleanup after native resources very well, since the OS will. With Unity, this a major issue. The Unity editor when changing play modes doesn't open and close, plus with Unity, the scene typically changes. This *probably* also wouldn't be an issue if [Unity could unload native binaries](https://docs.unity3d.com/Manual/PluginInspector.html).

An example of this would be a project called [cef-unity-sample](https://github.com/aleab/cef-unity-sample). It runs CEF directly inside of Unity. It avoids the first point by running CEF in "Single Process mode", which is a now removed feature of CEF that allowed running CEF in a single process. But the second issue it suffers from, resulting in Unity crashing when reloading the scene in anyway.

By running the web browser in a separate process, you avoid both issues. Yes it means having an entire second app ship with your Unity project, but you are already trying to run a web browser inside of Unity, so clearly resources are not your biggest concern.
