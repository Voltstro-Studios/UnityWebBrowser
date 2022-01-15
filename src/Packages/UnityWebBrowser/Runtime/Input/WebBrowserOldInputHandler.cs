using System;
using System.Collections.Generic;
using UnityEngine;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Input
{
    [CreateAssetMenu(fileName = "Old Input Handler", menuName = "UWB/Inputs/Old Input Handler")]
    public class WebBrowserOldInputHandler : WebBrowserInputHandler
    {
        public string scrollAxisName = "Mouse ScrollWheel";
        public float scrollSensitivity = 2.0f;
        
        private static readonly KeyCode[] Keymap = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
        private readonly List<WindowsKey> keysDown = new();
        private readonly List<WindowsKey> keysUp = new();
        
        
        public override float GetScroll()
        {
            return UnityEngine.Input.GetAxis(scrollAxisName) * scrollSensitivity;
        }

        public override Vector2 GetMousePos()
        {
            return UnityEngine.Input.mousePosition;
        }

        public override WindowsKey[] GetDownKeys()
        {
            keysDown.Clear();
            
            foreach (KeyCode key in Keymap)
            {
                //Why are mouse buttons considered key codes???
                if (key is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2 or KeyCode.Mouse3 or KeyCode.Mouse4 or KeyCode.Mouse5 or KeyCode.Mouse6)
                    continue;

                try
                { 
                    if(UnityEngine.Input.GetKeyDown(key))
                        keysDown.Add(key.UnityKeyCodeToWindowKey());
                }
                catch (ArgumentOutOfRangeException)
                {
                    //Safe to ignore
                }
            }

            return keysDown.ToArray();
        }

        public override WindowsKey[] GetUpKeys()
        {
            keysUp.Clear();
            
            foreach (KeyCode key in Keymap)
            {
                //Why are mouse buttons considered key codes???
                if (key is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2 or KeyCode.Mouse3 or KeyCode.Mouse4 or KeyCode.Mouse5 or KeyCode.Mouse6)
                    continue;

                try
                {
                    if (UnityEngine.Input.GetKeyUp(key))
                        keysUp.Add(key.UnityKeyCodeToWindowKey());
                }
                catch (ArgumentOutOfRangeException)
                {
                    //Safe to ignore
                }
            }

            return keysUp.ToArray();
        }

        public override string GetFrameInputBuffer()
        {
            return UnityEngine.Input.inputString;
        }

        public override void OnStart()
        {
            keysDown.Clear();
            keysUp.Clear();
        }

        public override void OnStop()
        {
        }
    }
}