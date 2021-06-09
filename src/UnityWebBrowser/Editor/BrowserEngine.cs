using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityWebBrowser.Editor
{
    public struct BrowserEngine
    {
        public BrowserEngine(string engineName, string engineAppFile, Dictionary<BuildTarget, string> buildFiles)
        {
            EngineName = engineName;
            EngineAppFile = engineAppFile;
            BuildFiles = buildFiles;
        }
        
        public string EngineName { get; }
        
        public string EngineAppFile { get; }
        
        public Dictionary<BuildTarget, string> BuildFiles { get; }
    }
}
