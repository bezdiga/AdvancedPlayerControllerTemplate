using System;
using UnityEngine;
using UnityEngine.Events;

namespace HatchStudio.Manager
{
    public interface IOption
    {
        object BoxedValue { get; set; }
        event UnityAction OptionChanged;
        
    }
    [Serializable]
    public sealed class Option<V> : IOption where V : struct, IEquatable<V>
    {
        [SerializeField] private V _value;


        public Option(V value = default(V))
        {
            _value = value;
        }

        object IOption.BoxedValue
        {
            get => _value;
            set => Value = value as V? ?? default(V);
        }

        public V Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value))
                    return;

                _value = value;
                OptionChanged?.Invoke();
                Changed?.Invoke(value);
            }
        }

        public event UnityAction OptionChanged;
        public event UnityAction<V> Changed;

        public static implicit operator V(Option<V> option) => option.Value;
    }
}