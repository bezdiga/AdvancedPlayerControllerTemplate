using UnityEditor;
using UnityEngine;

namespace HatchStudios.ToolBox
{
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUI.GetPropertyHeight(property);
          
            if (property.objectReferenceValue)
            {
                //draw property in the default way
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
            }
            else
            {
                //create HelpBox
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.boxHeight );
                EditorGUI.HelpBox(helpBoxRect, Attribute.Label, MessageType.Error);
                position.y += Style.boxHeight + Style.spacing ;
                
                using (new BackgroundColor(Style.errorBackgroundColor))
                {
                    EditorGUI.PropertyField(position, property, label, property.isExpanded);
                }
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.objectReferenceValue ? EditorGUI.GetPropertyHeight(property, label) : EditorGUI.GetPropertyHeight(property, label) + Style.boxHeight + Style.spacing;
        }
        
        public bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
        private NotNullAttribute Attribute => attribute as NotNullAttribute;
        private static class Style
        {
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
            
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly Color errorBackgroundColor = Color.red;
        }
    }
}
