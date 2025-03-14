using UnityEngine;

namespace HatchStudio.StateMachine
{
    public abstract class StateAsset : ScriptableObject
    {
        public virtual string StateKey => ToString();
        
        /// <summary>
        /// Get State display Name
        /// </summary>
        public virtual string Name => GetType().Name;
    }
}