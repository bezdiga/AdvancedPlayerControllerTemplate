using System.Reflection;
using UnityEditor;
using System;
namespace HatchStudios.ToolBox.Utility
{
    public static class PropertyUtility
    {
        //NOTE: last non-reflection implementation was ok but support for [SerializeReference] makes it a bit slow
        // unfortunately UnityEditor.ScriptAttributeUtility.GetFieldInfoFromProperty is internal so we have to retrive it using reflection
        private static readonly MethodInfo getGetFieldInfoFromPropertyMethod =
            ReflectionUtility.GetEditorMethod("UnityEditor.ScriptAttributeUtility", "GetFieldInfoFromProperty",
                BindingFlags.NonPublic | BindingFlags.Static);

        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            return GetFieldInfo(property, out _);
        }

        public static FieldInfo GetFieldInfo(SerializedProperty property, out Type propertyType)
        {
            var parameters = new object[] { property, null };
            var result = getGetFieldInfoFromPropertyMethod.Invoke(null, parameters) as FieldInfo;
            propertyType = parameters[1] as Type;
            return result;
        }
        
        public static T GetAttribute<T>(SerializedProperty property) where T : Attribute
        {
            return GetAttribute<T>(property, GetFieldInfo(property, out _));
        }
        public static T GetAttribute<T>(SerializedProperty property, FieldInfo fieldInfo) where T : Attribute
        {
            return fieldInfo.GetCustomAttribute<T>(true);
        }
    }
}