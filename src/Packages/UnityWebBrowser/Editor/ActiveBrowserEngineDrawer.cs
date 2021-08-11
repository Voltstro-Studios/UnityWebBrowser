using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityWebBrowser.BrowserEngine;

namespace UnityWebBrowser.Editor
{
    [CustomPropertyDrawer(typeof(ActiveBrowserEngineAttribute))]
    public class ActiveBrowserEngineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Only work on strings
            if (property.propertyType != SerializedPropertyType.String)
            {
                Debug.LogError($"{nameof(ActiveBrowserEngineAttribute)} can only be applied to strings!");
                return;
            }

            //Make sure there is a browser engine installed
            List<BrowserEngine> engines = BrowserEngineManager.Engines;
            if (engines.Count == 0)
            {
                EditorGUI.HelpBox(position, "No browser engines installed!", MessageType.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(property.stringValue))
                //Set to what ever 0 is
                property.stringValue = engines[0].EngineAppFile;

            //Get all engine names
            string[] engineNames = new string[engines.Count];
            for (int i = 0; i < engines.Count; i++) engineNames[i] = engines[i].EngineName;

            //Get current engine
            int index = engines.FindIndex(x => x.EngineAppFile == property.stringValue);

            //Index is negative, reset it back
            if (index < 0)
                index = 0;

            //Dropdown
            index = EditorGUI.Popup(position, "Browser Engine", index, engineNames);

            //Set the property's value
            property.stringValue = engines[index].EngineAppFile;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            List<BrowserEngine> engines = BrowserEngineManager.Engines;

            return engines.Count == 0 ? 38f : base.GetPropertyHeight(property, label);
        }
    }
}