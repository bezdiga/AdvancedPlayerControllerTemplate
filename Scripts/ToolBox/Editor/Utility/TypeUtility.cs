using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

namespace HatchStudios.ToolBox.Utility
{
    public static class TypeUtility
    {
        internal static readonly Dictionary<int, TypesCachedCollection> s_cachedCollections = new Dictionary<int, TypesCachedCollection>();
        internal static readonly Dictionary<int, TypesEditorCollection> s_editorCollections = new Dictionary<int, TypesEditorCollection>();
        public static TypesCachedCollection GetCollection(TypeConstraintContext typeConstraint)
        {
            var key = typeConstraint.GetHashCode();
            if (s_cachedCollections.TryGetValue(key, out TypesCachedCollection collection))
            {
                return collection;
            }

            var typesList = FindType(typeConstraint);
            return s_cachedCollections[key] = new TypesCachedCollection(typesList);

        }

        public static TypesEditorCollection GetCollection(TypeAppearanceContext appearanceContext)
        {
            var key = appearanceContext.GetHashCode();
            if (s_editorCollections.TryGetValue(key, out TypesEditorCollection collection))
                return collection;
            var types = GetCollection(appearanceContext.Constraint);
            
            return s_editorCollections[key] = new TypesEditorCollection(types,
                appearanceContext.AddEmptyValue, appearanceContext.TypeGrouping);
        }
        
        public static List<Type> FindType(TypeConstraintContext typeConstraint)
        {
            var targetType = typeConstraint.TargetType;
            var typeCache = TypeCache.GetTypesDerivedFrom(targetType);
            var typesList = typeCache.ToList();
            typesList.Add(targetType);
            
            for (var i = typesList.Count - 1; i >= 0; i--)
            {
                var type = typesList[i];
                if (typeConstraint.IsSatisfied(type))
                {
                    continue;
                }
                typesList.RemoveAt(i);
            }

            if (typeConstraint.IsOrdered)
            {
                var comparer = typeConstraint.Comparer;
                typesList.Sort(comparer);
            }

            return typesList;
        }
    }
}