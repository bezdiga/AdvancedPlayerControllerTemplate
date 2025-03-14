using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HatchStudios.ToolBox.Utility
{
    public class TypeField
    {
        public TypeConstraintContext ConstraintContext
        {
            get => constraintContext;
            set
            {
                constraintContext = value ?? throw new NullReferenceException($"Cannot assign null constraint to the {nameof(TypeField)}.");
                AppearanceContext.Constraint = constraintContext;
            }
        }

        public TypeAppearanceContext AppearanceContext
        {
            get => appearanceContext;
            set
            {
                appearanceContext = value ?? throw new NullReferenceException($"Cannot assign null appearance to the {nameof(TypeField)}.");
                ConstraintContext = appearanceContext.Constraint;
            }
        }
        private TypeConstraintContext constraintContext;
        private TypeAppearanceContext appearanceContext;

        public TypeField(TypeConstraintContext context) : this(context, null)
        {
        }

        public TypeField(TypeConstraintContext context, TypeAppearanceContext appearance)
        {
            constraintContext = context ?? new TypeConstraintStandard(null,TypeSettings.Class,false,false);
            appearanceContext =
                appearance ?? new TypeAppearanceContext(this.constraintContext, TypeGrouping.None, true);
        }

        public void OnGUI(Rect position, bool addSearchField, Action<Type> onSelect, Type activeType)
        {
            var collection = TypeUtility.GetCollection(AppearanceContext);
            var values = collection.Values;
            var labels = collection.Labels;
            var index = collection.IndexOf(activeType);
            
            var addEmptyValue = AppearanceContext.AddEmptyValue;

            if (addSearchField)
            {
                /*var buttonLabel = new GUIContent(labels[index]);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, labels, (i) =>
                {
                    var type = RetriveSelectedType(values, i, addEmptyValue);
                    onSelect?.Invoke(type);
                });*/
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, index, labels);
                if (EditorGUI.EndChangeCheck())
                {
                    var type = RetriveSelectedType(values, index, addEmptyValue);
                    onSelect?.Invoke(type);
                }
            }
        }
        
        private Type RetriveSelectedType(IReadOnlyList<Type> types, int selectedIndex, bool includeEmptyValue)
        {
            if (includeEmptyValue)
            {
                selectedIndex -= 1;
            }

            return selectedIndex >= 0 ? types[selectedIndex] : null;
        }
    }
}