using UnityEditor;
using UnityEngine;
using Resolution = UnityWebBrowser.Shared.Resolution;

namespace UnityWebBrowser.Editor
{
    [CustomPropertyDrawer(typeof(Resolution))]
    public class ResolutionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            Rect xFieldSize = new Rect(position.x, position.y, 50, position.height);
            Rect xLabelSize = new Rect(position.x + 55, position.y, 25, position.height);
            Rect yFieldSize = new Rect(position.x + 70, position.y, 50, position.height);

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