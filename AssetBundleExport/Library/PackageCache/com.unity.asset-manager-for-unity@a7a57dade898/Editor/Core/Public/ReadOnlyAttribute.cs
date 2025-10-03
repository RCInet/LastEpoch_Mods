#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace Unity.AssetManager.Editor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    class ReadOnlyAttribute : PropertyAttribute { }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var adjustedPosition = EditorGUI.PrefixLabel(position, -1, label);
            EditorGUI.SelectableLabel(adjustedPosition, property.stringValue);
        }
    }
#endif
}
