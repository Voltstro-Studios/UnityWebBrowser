// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Reflection;
using System.Runtime.Loader;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;

internal class CommunicationLayerLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver resolver;

    public CommunicationLayerLoadContext(string dllPath)
    {
        resolver = new AssemblyDependencyResolver(dllPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
}