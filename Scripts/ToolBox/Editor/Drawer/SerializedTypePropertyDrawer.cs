using HatchStudios.ToolBox;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializedType))]
    [CustomPropertyDrawer(typeof(TypeConstraintAttribute))]
    public class SerializedTypePropertyDrawer : PropertyDrawerBase
    {
        private SerializedProperty _property;
        private SerializedTypeDrawer _referenceDrawer;

        public override bool IsPropertyValid(SerializedProperty property) => !property.hasMultipleDifferentValues;

        protected override void OnGUISafe(Rect pozition, SerializedProperty property, GUIContent label)
        {
            HandlePropertyChanges(property);
            _referenceDrawer.OnGUI(pozition,property,label);
        }
        
        private void HandlePropertyChanges(SerializedProperty property)
        {
            if (property != _property)
            {
                _property = property;
                _referenceDrawer = new SerializedTypeDrawer(property);
            }
        }
        
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }
    }
}