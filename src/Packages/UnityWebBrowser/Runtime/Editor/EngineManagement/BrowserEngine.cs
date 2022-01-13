#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;

namespace UnityWebBrowser.Editor.EngineManagement
{
    /// <summary>
    ///     Contains info about an engine that can be used with UWB
    /// </summary>
    public readonly struct BrowserEngine
    {
        /// <summary>
        ///     Creates a new <see cref="BrowserEngine" /> instance
        /// </summary>
        /// <param name="engineName">The name of the engine</param>
        /// <param name="engineAppFile">The executable's name (without any extensions)</param>
        /// <param name="buildFiles"><see cref="Dictionary{TKey,TValue}" /> containing what files to include</param>
        public BrowserEngine(string engineName, string engineAppFile, Dictionary<BuildTarget, string> buildFiles)
        {
            EngineName = engineName;
            EngineAppFile = engineAppFile;
            BuildFiles = buildFiles;
        }

        /// <summary>
        ///     The name of the engine
        /// </summary>
        public string EngineName { get; }

        /// <summary>
        ///     The executable's name (without any extensions)
        /// </summary>
        public string EngineAppFile { get; }

        /// <summary>
        ///     <see cref="Dictionary{TKey,TValue}" /> containing what files to include.
        /// </summary>
        public Dictionary<BuildTarget, string> BuildFiles { get; }
    }
}

#endif