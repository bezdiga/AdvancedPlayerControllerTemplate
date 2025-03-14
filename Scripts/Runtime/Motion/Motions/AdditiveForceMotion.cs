using System.Collections.Generic;
using UnityEngine;

namespace HatchStudio.Motion
{
    public class AdditiveForceMotion : MotionBehaviour
    {
        
        [SerializeField]
        private SpringSettings _slowPositionSpring = new(10, 100, 1, 1);

        [SerializeField]
        private SpringSettings _slowRotationSpring = new(15, 95, 1, 1);

        [SerializeField]
        private SpringSettings _fastPositionSpring = new(12, 140, 1, 1.1f);

        [SerializeField]
        private SpringSettings _fastRotationSpring = new(12, 140, 1, 1.1f);
        
        private SpringType _positionSpringMode = SpringType.SlowSpring;
        private SpringType _rotationSpringMode = SpringType.SlowSpring;
        
        private readonly List<DistributedForce> _positionForces = new(2);
        private readonly List<CurveForce> _positionCurves = new(2);
        
        private readonly List<DistributedForce> _rotationForces = new(2);
        private readonly List<CurveForce> _rotationCurves = new(2);
        private bool _requiresUpdate = false;
        
        private const float POSITION_FORCE_MOD = 0.02f;
        
        protected override SpringSettings GetDefaultPositionSpringSettings() => _slowPositionSpring;
        protected override SpringSettings GetDefaultRotationSpringSettings() => _slowRotationSpring;
        public override void UpdateMotion(float deltaTime)
        {
            if (!_requiresUpdate)
                return;

            float time = Time.time;

            Vector3 targetPosition = EvaluatePositionForces(time);
            Vector3 targetRotation = EvaluateRotationForces(time);
            
            SetTargetPosition(targetPosition);
            SetTargetRotation(targetRotation);

            if (targetPosition == Vector3.zero && targetPosition == Vector3.zero && PositionSpring.IsIdle && RotationSpring.IsIdle)
                _requiresUpdate = false;
        }

        private Vector3 EvaluatePositionForces(float time)
        {
            Vector3 force = Vector3.zero;
            for (int i = _positionForces.Count - 1; i >= 0; i--)
            {
                force += _positionForces[i].Force;

                if (time > _positionForces[i].EndTime)
                    _positionForces.RemoveAt(i);
            }

            for (int i = _positionCurves.Count - 1; i >= 0; i--)
            {
                var animCurve = _positionCurves[i].AnimCurve;
                float startTime = _positionCurves[i].StartTime;

                force += animCurve.Evaluate(time - startTime);

                if (time > startTime + animCurve.Duration)
                    _positionCurves.RemoveAt(i);
            }

            return force;
        }

        private Vector3 EvaluateRotationForces(float time)
        {
            Vector3 force = Vector3.zero;
            for (int i = _rotationForces.Count - 1; i >= 0; i--)
            {
                force += _rotationForces[i].Force;

                if (time > _rotationForces[i].EndTime)
                    _rotationForces.RemoveAt(i);
            }

            for (int i = _rotationCurves.Count - 1; i >= 0; i--)
            {
                var animCurve = _rotationCurves[i].AnimCurve;
                float startTime = _rotationCurves[i].StartTime;

                force += animCurve.Evaluate(time - startTime);

                if (time > startTime + animCurve.Duration)
                    _rotationCurves.RemoveAt(i);
            }

            return force;
        }
        
        public void AddPositionForce(SpringForce3D springForce, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            if (springForce.IsEmpty())
                return;

            if (springType != _positionSpringMode)
            {
                _positionSpringMode = springType;
                PositionSpring.Adjust(_positionSpringMode == SpringType.SlowSpring ? _slowPositionSpring : _fastPositionSpring);
            }

            float time = Time.time;
            Vector3 force = springForce.Force * (scale * POSITION_FORCE_MOD);
            _positionForces.Add(new DistributedForce(force, time + springForce.Duration));
            _requiresUpdate = true;
        }
        
        public void AddRotationForce(SpringForce3D springForce, float scale = 1f, SpringType springType = SpringType.SlowSpring)
        {
            if (springForce.IsEmpty())
                return;

            if (springType != _rotationSpringMode)
            {
                _rotationSpringMode = springType;
                RotationSpring.Adjust(_rotationSpringMode == SpringType.SlowSpring ? _slowRotationSpring : _fastRotationSpring);
            }

            float time = Time.time;
            Vector3 force = springForce.Force * scale;
            _rotationForces.Add(new DistributedForce(force, time + springForce.Duration));
            _requiresUpdate = true;
        }
        
        #region Internal
        private readonly struct DistributedForce
        {
            public readonly Vector3 Force;
            public readonly float EndTime;

            public DistributedForce(Vector3 force, float endTime)
            {
                Force = force;
                EndTime = endTime;
            }
        }

        private readonly struct CurveForce
        {
            public readonly AnimCurves3D AnimCurve;
            public readonly float StartTime;

            public CurveForce(AnimCurves3D animCurve, float startTime)
            {
                AnimCurve = animCurve;
                StartTime = startTime;
            }
        }
        #endregion
    }
    
    public enum SpringType
    {
        SlowSpring = 1,
        FastSpring = 2
    }
}