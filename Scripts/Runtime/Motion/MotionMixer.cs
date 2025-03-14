using System;
using System.Collections.Generic;
using HatchStudio.Manager;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HatchStudio.Motion
{
    [DefaultExecutionOrder(ExecutionOrderConstants.AFTER_DEFAULT_2)]
    public class MotionMixer : MonoBehaviour, IMotionMixer
    {
        public Transform TargetTransform => _targetTransform;
        
        [SerializeField]
        private Vector3 _pivotOffset;

        [SerializeField]
        private Vector3 _positionOffset;

        [SerializeField]
        private Vector3 _rotationOffset;
        
        private readonly List<IMixedMotion> _motions = new();
        private readonly Dictionary<Type, IMixedMotion> _motionsDict = new();
        
        public float WeightMultiplier
        {
            get => _weightMultiplier;
            set
            {
                value = Mathf.Clamp01(value);
                _weightMultiplier = value;
                
            }
        }
        
        [SerializeField] private Transform _targetTransform;
        private float _weightMultiplier = 1f;
        
        private void LateUpdate()
        {
            UpdateMotions(Time.deltaTime);
        }
        
        private void UpdateMotions(float deltaTime)
        {
            Vector3 targetPos = _pivotOffset;
            Quaternion targetRot = Quaternion.identity;

            for (int i = 0; i < _motions.Count; i++)
            {
                var motion = _motions[i];
                motion.UpdateMotion(deltaTime);

                targetPos += targetRot * motion.GetPosition(deltaTime);
                targetRot *= motion.GetRotation(deltaTime);
            }
            
            targetPos = targetPos - targetRot * _pivotOffset + _positionOffset;
            targetRot *= Quaternion.Euler(_rotationOffset);

            _targetTransform.SetLocalPositionAndRotation(targetPos, targetRot);
        }
        
        public void AddMixedMotion(IMixedMotion mixedMotion)
        {
            if (mixedMotion == null)
                return;
            
            var motionType = mixedMotion.GetType();
            if (!_motionsDict.ContainsKey(motionType))
            {
                _motionsDict.Add(motionType, mixedMotion);
                _motions.Add(mixedMotion);
                mixedMotion.Multiplier = _weightMultiplier;
            }
        }

        public void RemoveMixedMotion(IMixedMotion mixedMotion)
        {
            if (mixedMotion == null)
                return;
      
            if (_motionsDict.Remove(mixedMotion.GetType())) 
                _motions.Remove(mixedMotion);
           
        }

        public T GetMotionOfType<T>() where T : class, IMixedMotion
        {
            if (_motionsDict.TryGetValue(typeof(T), out IMixedMotion mixedMotion))
                return (T)mixedMotion;
            
            Debug.LogError("Not motion of type " + typeof(T));
            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying && _targetTransform != null)
                _targetTransform.SetLocalPositionAndRotation(_positionOffset, Quaternion.Euler(_rotationOffset));
        }

        private void Reset() => _targetTransform = transform;

        private void OnDrawGizmos()
        {
            Color pivotColor = new(0.1f, 1f, 0.1f, 0.5f);
            const float PIVOT_RADIUS = 0.08f;

            var prevColor = Handles.color;
            Handles.color = pivotColor;
            Handles.SphereHandleCap(0, transform.TransformPoint(_pivotOffset), Quaternion.identity, PIVOT_RADIUS, EventType.Repaint);
            Handles.color = prevColor;
        }
#endif
        
    }
}