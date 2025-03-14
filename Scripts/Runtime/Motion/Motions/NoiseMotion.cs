using HatchStudio.Motion.Data;
using UnityEngine;

namespace HatchStudio.Motion
{
    public class NoiseMotion : DataMotionBehaviour<NoiseMotionData>
    {
        
        private const float POSITION_FORCE_MOD = 0.01f;
        private const float ROTATION_FORCE_MOD = 2f;
        [SerializeField]
        private SpringSettings _positionSpring = new(10f, 100f, 1f, 1f);

        [SerializeField]
        private SpringSettings _rotationSpring = new(10f, 100f, 1f, 1f);
        
        protected override SpringSettings GetDefaultPositionSpringSettings() => _positionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => _rotationSpring;
        
        public override void UpdateMotion(float deltaTime)
        {
            if(Data == null)
                return;
            float jitter = Data.NoiseJitter < 0.01f ? 0f : Random.Range(0f, Data.NoiseJitter);
            float speed = Time.time * Data.NoiseSpeed;

            Vector3 posNoise = new()
            {
                x = (Mathf.PerlinNoise(jitter, speed) - 0.5f) * Data.PositionAmplitude.x * POSITION_FORCE_MOD,
                y = (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * Data.PositionAmplitude.y * POSITION_FORCE_MOD,
                z = (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * Data.PositionAmplitude.z * POSITION_FORCE_MOD
            };

            Vector3 rotNoise = new()
            {
                x = (Mathf.PerlinNoise(jitter, speed) - 0.5f) * Data.RotationAmplitude.x * ROTATION_FORCE_MOD,
                y = (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * Data.RotationAmplitude.y * ROTATION_FORCE_MOD,
                z = (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * Data.RotationAmplitude.z * ROTATION_FORCE_MOD
            };

            SetTargetPosition(posNoise);
            SetTargetRotation(rotNoise);
        }
    }
}