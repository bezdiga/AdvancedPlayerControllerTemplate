using System;
using System.Diagnostics;
using UnityEngine;

namespace HatchStudios.ToolBox
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public string CallbackMethodName { get; private set; }

        public OnValueChangedAttribute(string callbackMethodName)
        {
            CallbackMethodName = callbackMethodName;
        }
    }
}