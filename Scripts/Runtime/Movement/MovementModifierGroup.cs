using System;
using System.Collections.Generic;

namespace HatchStudios.PlayerController
{
    [Serializable]
    public sealed class MovementModifierGroup
    {
        public delegate float ModifierDelegate();

        private readonly List<ModifierDelegate> _modifiers;
        private readonly float _baseValue;
        
        public MovementModifierGroup(float baseValue, MovementModifierGroup group)
        {
            _baseValue = baseValue;
            _modifiers = group?._modifiers ?? new List<ModifierDelegate>();
        }
        
        public float EvaluateValue()
        {
            float mod = _baseValue;

            for (int i = 0; i < _modifiers.Count; i++)
                mod *= _modifiers[i].Invoke();

            return mod;
        }
        public void AddModifier(ModifierDelegate modifier)
        {
            if (modifier == null)
                return;

            if (!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        public void RemoveModifier(ModifierDelegate modifier)
        {
            if (modifier == null)
                return;

            _modifiers.Remove(modifier);
        }
    }
}