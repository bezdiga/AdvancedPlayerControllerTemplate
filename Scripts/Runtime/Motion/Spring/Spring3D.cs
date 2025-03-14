using System;
using UnityEngine;

namespace HatchStudio.Motion
{
    [Serializable]
    public class Spring3D
    {
        private SpringSettings _settings;
        private Vector3 _acceleration;
        private Vector3 _targetValue;
        private Vector3 _value;
        private Vector3 _velocity;
        private bool _isIdle;
        
        private const float MAX_STEP_SIZE = 1f / 61f;


        public Spring3D() : this(SpringSettings.Default) { }

        public Spring3D(SpringSettings settings)
        {
            _settings = settings;
            _isIdle = true;
            _targetValue = Vector3.zero;
            _velocity = Vector3.zero;
            _acceleration = Vector3.zero;
        }

        public bool IsIdle => _isIdle;

        public void Adjust(SpringSettings settings)
        {
            _settings = settings;
            _isIdle = false;
        }

        /// <summary>
        /// Reset all values to initial states.
        /// </summary>
        public void Reset()
        {
            _isIdle = true;
            _value = Vector3.zero;
            _velocity = Vector3.zero;
            _acceleration = Vector3.zero;
        }

        /// <summary>
        /// Sets the target value in the middle of motion.
        /// This reuse the current velocity and interpolate the value smoothly afterwards.
        /// </summary>
        /// <param name="value">Target value</param>
        public void SetTargetValue(Vector3 value)
        {
            _targetValue = value;
            _isIdle = false;
        }

        /// <summary>
        /// Advance a step by deltaTime(seconds).
        /// </summary>
        /// <param name="deltaTime">Delta time since previous frame</param>
        /// <returns>Evaluated Value</returns>
        public Vector3 Evaluate(float deltaTime)
        {
            if (_isIdle)
                return Vector3.zero;

            float damp = _settings.Damping;
            float stf = _settings.Stiffness;
            float mass = _settings.Mass;

            Vector3 val = _value;
            Vector3 vel = _velocity;
            Vector3 acc = _acceleration;

            float stepSize = deltaTime * _settings.Speed;
            float maxStepSize = stepSize > MAX_STEP_SIZE ? MAX_STEP_SIZE : stepSize - 0.001f;
            float steps = (int)(stepSize / maxStepSize + 0.5f);

            for (var i = 0; i < steps; i++)
            {
                var dt = Math.Abs(i - (steps - 1)) < 0.01f ? stepSize - i * maxStepSize : maxStepSize;

                val += vel * dt + acc * (dt * dt * 0.5f);

                Vector3 calcAcc = (-stf * (val - _targetValue) + -damp * vel) / mass;

                vel += (acc + calcAcc) * (dt * 0.5f);
                acc = calcAcc;
            }

            _value = val;
            _velocity = vel;
            _acceleration = acc;

            if (Mathf.Approximately(acc.sqrMagnitude, 0f))
                _isIdle = true;

            return _value;
        }
    }
}