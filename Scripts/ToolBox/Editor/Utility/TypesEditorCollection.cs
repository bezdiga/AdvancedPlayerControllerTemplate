using System;
using System.Collections.Generic;
using UnityEngine;

namespace HatchStudios.ToolBox.Utility
{
    public class TypesEditorCollection : TypesCachedCollection
    {
        public string[] Labels => labels;
        
        private readonly bool hasEmptyValue;
        private string[] labels;
        
        public TypesEditorCollection(List<Type> values) : base(values)
        {
        }
        public TypesEditorCollection(TypesCachedCollection cachedCollection, bool hasEmptyValue)
            : this(cachedCollection, hasEmptyValue, TypeGrouping.None)
        { }
        
        public TypesEditorCollection(TypesCachedCollection cachedCollection, bool hasEmptyValue, TypeGrouping grouping)
            : base(cachedCollection)
        {
            this.hasEmptyValue = hasEmptyValue;
            CreateLabels(grouping);
        }

        private void CreateLabels(TypeGrouping grouping)
        {
            var count = Values.Count;
            var shift = 0;

            if (hasEmptyValue)
            {
                shift += 1;
                count += 1;
                labels = new string[count];
                labels[0] = "<none>";
            }
            else labels = new string[count];

            for (int i = 0; i < count - shift; i++)
            {
                var type = Values[i];
                var name = FormatGroupedTypeName(type,grouping);
                labels[i + shift] = name;
            }

        }

        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            var name = type.FullName;

            switch (grouping)
            {
                case TypeGrouping.None:
                    return name;
                case TypeGrouping.ByNamespace:
                    return name.Replace('.', '/');
                case TypeGrouping.ByNamespaceFlat:
                    var lastPeriodIndex = name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = name.Substring(0, lastPeriodIndex) + "/" + name.Substring(lastPeriodIndex + 1);
                    }

                    return name;
                case TypeGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    return "Scripts/" + type.FullName.Replace('.', '/');

                case TypeGrouping.ByFlatName:
                    return type.Name;
            }

            return name;
        }
        
        public override int IndexOf(Type type)
        {
            var index = -1;
            if (type != null)
            {
                index = base.IndexOf(type);
            }

            return hasEmptyValue
                ? index + 1
                : index;
        }
    }
    
    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    public enum TypeGrouping
    {
        /// <summary>
        /// No grouping, just show type names in a list; for instance, "Some.Nested.Namespace.SpecialClass".
        /// </summary>
        None,
        /// <summary>
        /// Group classes by namespace and show foldout menus for nested namespaces; for
        /// instance, "Some > Nested > Namespace > SpecialClass".
        /// </summary>
        ByNamespace,
        /// <summary>
        /// Group classes by namespace; for instance, "Some.Nested.Namespace > SpecialClass".
        /// </summary>
        ByNamespaceFlat,
        /// <summary>
        /// Group classes in the same way as Unity does for its component menu. This
        /// grouping method must only be used for <see cref="MonoBehaviour"/> types.
        /// </summary>
        ByAddComponentMenu,
        /// <summary>
        /// Only name of the <see cref="Type"/>.
        /// </summary>
        ByFlatName
    }
}