using System;
using System.Diagnostics;

namespace HatchStudios.ToolBox
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public sealed class DataReferenceDetailsAttribute : Attribute
    {
        public bool HasAssetReference { get; set; }
        public bool HasLabel { get; set; } = true;
        public bool HasIcon { get; set; } = true;
        public bool HasNullElement { get; set; } = true;
        public string NullElementName { get; set; } = "Empty";
    }
}