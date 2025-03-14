using System;
using System.Diagnostics;
using HatchStudios.ToolBox.Utility;
using UnityEngine;

namespace HatchStudios.ToolBox
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class TypeConstraintAttribute : PropertyAttribute
    {
        public Type AssemblyType { get; private set; }
        public bool AllowAbstract { get; set; }
        /// <summary>
        /// Gets or sets whether obsolete classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowObsolete { get; set; }

        /// <summary>
        /// Indicates if created popup menu should have an additional search field.
        /// </summary>
        public bool AddTextSearchField { get; set; }

        /// <summary>
        /// Indicates if types should be sorted alphabetically.
        /// </summary>
        public bool OrderTypes { get; set; } = true;
        /// <summary>
        /// Gets or sets grouping of selectable classes.
        /// Defaults to <see cref="TypeGrouping.None"/> unless explicitly specified.
        /// </summary>
        public TypeGrouping TypeGrouping { get; set; } = TypeGrouping.None;
        /// <summary>
        /// Indicates what kind of types are accepted.
        /// </summary>
        public TypeSettings TypeSettings { get; set; } = TypeSettings.Class | TypeSettings.Interface;
        
        public TypeConstraintAttribute(Type assemblyType)
        {
            AssemblyType = assemblyType;
        }
    }
    
    ///<inheritdoc/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ClassImplementsAttribute : TypeConstraintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        [Obsolete]
        public ClassImplementsAttribute() : base(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public ClassImplementsAttribute(Type interfaceType) : base(interfaceType)
        {
            TypeSettings = TypeSettings.Class;
        }
    }
}