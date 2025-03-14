using System;
using System.Collections.Generic;
using System.Linq;
using HatchStudios.PlayerController;
using UnityEngine;

namespace HatchStudio.Motion.Data
{
    [CreateAssetMenu(menuName = "Hatch Studio/Motion/Motion Preset", fileName = "Motion_")]
    public class MotionProfile : ScriptableObject
    {
        [SerializeField] private StateDataPair[] _motionData = Array.Empty<StateDataPair>();

        private Dictionary<Type, IMotionData>[] _states;
        
        public DataType GetData<DataType>(MovementStateType stateType) where DataType : IMotionData
        {
            _states ??= CachedData();
            
            int index = stateType.GetValue();
            if (_states.Length > index)
            {
                var dict = _states[index];

                if (dict != null && dict.TryGetValue(typeof(DataType), out var data))
                    return (DataType)data;
            }

            return default(DataType);
        }
        private Dictionary<Type, IMotionData>[] CachedData()
        {
            int maxState = 0;
            foreach (var data in _motionData)
            {
                int stateValue = data.StateType.GetValue();
                if (stateValue > maxState)
                    maxState = stateValue;
            }
            
            var states = new Dictionary<Type, IMotionData>[maxState + 1];

            foreach (var motionData in _motionData)
            {
                var dict = new Dictionary<Type, IMotionData>(motionData.Data.Length);
                
                foreach (var data in motionData.Data)
                {
                    if (data == null)
                    {
                        Debug.LogError(motionData.StateType, this);
                        return states;
                    }

                    var dataType = data.GetType();
                    dataType = GetMotionDataInterface(dataType) ?? dataType;

                    dict.Add(dataType, data);
                }

                int i = (int)motionData.StateType;
                states[i] = dict;
            }
            return states;
            
            static Type GetMotionDataInterface(Type dataType)
            {
                return dataType.GetInterfaces().FirstOrDefault(type => type != typeof(IMotionData) && typeof(IMotionData).IsAssignableFrom(type));
            }
        }
        #region Internal

        [Serializable]
        private struct StateDataPair
        {
            public MovementStateType StateType;
            public MotionData[] Data;
        }

        #endregion
        
    }
}