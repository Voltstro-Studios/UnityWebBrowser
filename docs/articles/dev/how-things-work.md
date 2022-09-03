# How Things Work?

So, lets talk about how UWB works.

UWB is split into 3 sections: 

- Core (Unity side)
- Shared (Glues the two sections together)
- Engine (Externally ran process)

All three sections are written in C#. As the engines are standalone applications, they use .NET 6, while the Unity side, is well... Unity.

## Core

The core is responsible for anything Unity related (such as handling getting Input or rendering to a <xref:UnityEngine.Texture>). It is provided as [standard custom UPM package](https://docs.unity3d.com/Manual/CustomPackages.html).

## Engine

The engine is an external process that is responsible for taking the abstraction layer, and converting the calls to the calls that a web engine expects. This means that all that the core cares about is that the engine has support for the abstraction layer provided.

Modern web engines are quite large (in binary size), and these binaries often have to be shipped with the engine them-selves, making them quite large as well.

## Shared

The shared library provides the abstraction layer that is used by the two sides.

There is also a shared library for engines, which provides some shared functions that only engines need.

## Communication

Ok, so we got the external engine process, and the core (Unity) process. We also have shared abstraction layer between the two. But how do these things talk to each other. Two different processes can't talk to each other, right? 

Communication is done via [RPC](https://en.wikipedia.org/wiki/Remote_procedure_call)s. We mainly uses RPCs instead of just directly writing data to a IPC pipe because using RPCs make programming and maintaining **a lot** easier. Under the hood, we use our own RPC library called [VoltRpc](https://github.com/Voltstro-Studios/VoltRpc).
