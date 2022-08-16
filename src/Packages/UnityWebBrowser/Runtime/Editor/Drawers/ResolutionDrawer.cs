// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Resolution = VoltstroStudios.UnityWebBrowser.Shared.Resolution;

namespace VoltstroStudios.UnityWebBrowser.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(Resolution))]
    internal class ResolutionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect xFieldSize = new(position.x, position.y, 50, position.height);
            Rect xLabelSize = new(position.x + 55, position.y, 25, position.height);
            Rect yFieldSize = new(position.x + 70, position.y, 50, position.height);

            SerializedProperty widthProp = property.FindPropertyRelative("Width");
            SerializedProperty heightProp = property.FindPropertyRelative("Height");

            int widthValue = EditorGUI.IntField(xFieldSize, widthProp.intValue);
            if (widthValue < 0)
                Debug.LogError("Width cannot be lower then 0!");
            else
                widthProp.intValue = widthValue;

            EditorGUI.LabelField(xLabelSize, "x");

            int heightValue = EditorGUI.IntField(yFieldSize, heightProp.intValue);
            if (heightValue < 0)
                Debug.LogError("Height cannot be lower then 0!");
            else
                heightProp.intValue = heightValue;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}

#endif