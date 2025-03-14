using System.Collections.Generic;
using System;
using System.Collections;

namespace HatchStudios.ToolBox.Utility
{
    public class TypesCachedCollection : IEnumerable<Type>
    {
        public IReadOnlyList<Type> Values => values;
        
        private readonly List<Type> values;
        
        
        public TypesCachedCollection() : this(new List<Type>())
        { }
        public TypesCachedCollection(List<Type> values)
        {
            this.values = values;
        }


        public virtual int IndexOf(Type type)
        {
            return values.IndexOf(type);
        }

        public virtual bool Contains(Type type)
        {
            return values.Contains(type);
        }
        
        public IEnumerator<Type> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
        
        public static implicit operator List<Type>(TypesCachedCollection collection) => collection.values;
    }
}