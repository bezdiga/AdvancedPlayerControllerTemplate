using System;
using UnityEngine;

namespace HatchStudio.Motion.Data
{
    [CreateAssetMenu(menuName = MOTION_DATA_MENU_PATH + "Noise", fileName = "Noise_")]
    public sealed class NoiseMotionData : MotionData
    {
        
        [SerializeField, Range(0f, 5f)]
        private float _noiseSpeed = 1f;

        [SerializeField, Range(0f, 1f)]
        private float _noiseJitter;
        
        [SerializeField]
        private Vector3 _positionAmplitude = Vector3.zero;

        [SerializeField]
        private Vector3 _rotationAmplitude = Vector3.zero;
        
        public float NoiseSpeed => _noiseSpeed;
        public float NoiseJitter => _noiseJitter;
        public Vector3 PositionAmplitude => _positionAmplitude;
        public Vector3 RotationAmplitude => _rotationAmplitude;
        
    }
}