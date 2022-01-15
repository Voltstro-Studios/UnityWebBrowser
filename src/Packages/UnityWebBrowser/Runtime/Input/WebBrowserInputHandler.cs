using UnityEngine;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Input
{
    public abstract class WebBrowserInputHandler : ScriptableObject
    {
        //Mouse functions
        public abstract float GetScroll();
        public abstract Vector2 GetMousePos();

        public abstract WindowsKey[] GetDownKeys();
        public abstract WindowsKey[] GetUpKeys();

        public abstract string GetFrameInputBuffer();
        
        //General
        public abstract void OnStart();
        public abstract void OnStop();
    }
}