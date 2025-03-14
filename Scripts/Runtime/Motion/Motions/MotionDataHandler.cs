using System;
using System.Collections.Generic;

using HatchStudio.Motion.Data;
using HatchStudios.PlayerController;
using HatchStudios.ToolBox;
using UnityEngine;

namespace HatchStudio.Motion
{
    public class MotionDataHandler : MonoBehaviour, IMotionDataHandler
    {
#if UNITY_EDITOR
        [OnValueChanged(nameof(Editor_PresetChanged))]
#endif
        [SerializeField] private MotionProfile _profile;
        private void Start() => SetPreset(_profile);
        private readonly Dictionary<Type, BehaviourEntry> _data = new();
        private MovementStateType _stateType;

        
        public void SetPreset(MotionProfile profile)
        {
            _profile = profile != null ? profile : _profile;
            UpdateAllEntries();
        }
        


        public void SetStateType(MovementStateType stateType)
        {
            _stateType = stateType;
            UpdateAllEntries();
        }
        
        public DataType GetData<DataType>() where DataType : IMotionData
        {
            if (_profile == null)
                return default(DataType);
            return _profile.GetData<DataType>(_stateType) ?? _profile.GetData<DataType>(MovementStateType.None);
        }

        public void RegisterBehaviour<T>(MotionDataChangedDelegate changedCallback)
        {
            if (_data.TryGetValue(typeof(T), out var entry))
                entry.Callbacks.Add(changedCallback);
            else
            {
                entry = new BehaviourEntry(changedCallback);
                _data.Add(typeof(T), entry);
            }
        }

        public void UnregisterBehaviour<T>(MotionDataChangedDelegate changedCallback)
        {
            if (_data.TryGetValue(typeof(T), out var entry))
            {
                var callbacks = entry.Callbacks;
                if (callbacks.Remove(changedCallback))
                {
                    if (callbacks.Count == 0)
                        _data.Remove(typeof(T));
                }
            }
        }
        
        private void InvokeEntry(BehaviourEntry entry, bool forceUpdate)
        {
            var callbacks = entry.Callbacks;
            for (int i = 0; i < callbacks.Count; i++)
                callbacks[i].Invoke(this, forceUpdate);
        }
        
        private void UpdateAllEntries()
        {
            foreach (var entry in _data.Values)
            {
                InvokeEntry(entry, false);
            }
        }
        
        #region Internal
        private sealed class BehaviourEntry
        {
            public readonly List<MotionDataChangedDelegate> Callbacks;
            
            public BehaviourEntry(MotionDataChangedDelegate callback)
            {
                Callbacks = new List<MotionDataChangedDelegate>
                {
                    callback
                };
            }
        }
        #endregion

        
        #region Unity_Editor

        #if UNITY_EDITOR
        
        private void Editor_PresetChanged()
        {
            if (Application.isPlaying)
                SetPreset(_profile);
        }
        #endif

        #endregion

    }
}