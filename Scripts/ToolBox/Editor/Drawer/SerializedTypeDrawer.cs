using System;
using HatchStudios.ToolBox;
using HatchStudios.ToolBox.Utility;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class SerializedTypeDrawer
    {
        private readonly TypeConstraintAttribute _attribute;
        private static readonly TypeConstraintContext sharedConstraint = new TypeConstraintStandard();
        private static readonly TypeAppearanceContext sharedAppearance = new TypeAppearanceContext(sharedConstraint, TypeGrouping.None, true);
        private static readonly TypeField typeField = new TypeField(sharedConstraint, sharedAppearance);

        public SerializedTypeDrawer(SerializedProperty property)
        {
            _attribute = PropertyUtility.GetAttribute<TypeConstraintAttribute>(property);
            UpdateConstraint(_attribute);
            UpdateAppearance(_attribute);
        }


        public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /*var collection = TypeUtility.GetCollection(sharedAppearance);
            EditorGUI.LabelField(position,new GUIContent(_attribute.AssemblyType.Name + " Colection " + collection.Values.Count));*/
            label = EditorGUI.BeginProperty(position, label, property);
            label = property.name != "data" ? label : GUIContent.none;
            position = EditorGUI.PrefixLabel(position, label);
            var addSearchField = _attribute.AddTextSearchField;

            var referenceProperty = property.FindPropertyRelative("typeReference");
            var activeType = SerializedType.GetReferenceType(referenceProperty.stringValue);
            typeField.OnGUI(position,addSearchField, (type) =>
            {
                try
                {
                    referenceProperty.serializedObject.Update();
                    referenceProperty.stringValue = SerializedType.GetReferenceValue(type);
                    referenceProperty.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e) when(e is ArgumentNullException || e is NullReferenceException)
                {
                    Console.WriteLine("Invalid attempt to update disposed property.");
                }
            },activeType);
            EditorGUI.EndProperty();
        }
        
        private void UpdateConstraint(TypeConstraintAttribute attribute)
        {
            sharedConstraint.ApplyTarget(attribute.AssemblyType);
            if (sharedConstraint is TypeConstraintStandard constraint)
            {
                constraint.IsOrdered = attribute.OrderTypes;
                constraint.AllowAbstract = attribute.AllowAbstract;
                constraint.AllowObsolete = attribute.AllowObsolete;
                constraint.Settings = attribute.TypeSettings;
            }
        }
        
        private void UpdateAppearance(TypeConstraintAttribute attribute)
        {
            sharedAppearance.TypeGrouping = attribute.TypeGrouping;
        }
    }
}