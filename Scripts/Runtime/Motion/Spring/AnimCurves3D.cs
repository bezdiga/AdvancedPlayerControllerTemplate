using System;
using UnityEngine;

namespace HatchStudio.Motion
{
    [Serializable]
    public sealed class AnimCurves3D
    {
        [SerializeField, Range(-10f, 10f)]
        private float _multiplier = 1f;

        [SerializeField]
        private AnimationCurve _xCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private AnimationCurve _yCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private AnimationCurve _zCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private float? _duration;
        private const float MAX_DURATION = 1000f;


        public AnimCurves3D() { }

        public AnimCurves3D(float multiplier, AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve zCurve)
        {
            _multiplier = multiplier;
            _xCurve = xCurve;
            _yCurve = yCurve;
            _zCurve = zCurve;
        }

        public float Duration => _duration ??= GetDuration();

        public Vector3 Evaluate(float time)
        {
            return new Vector3
            {
                x = _xCurve.Evaluate(time) * _multiplier,
                y = _yCurve.Evaluate(time) * _multiplier,
                z = _zCurve.Evaluate(time) * _multiplier
            };
        }

        public Vector3 Evaluate(float xTime, float yTime, float zTime)
        {
            return new Vector3
            {
                x = _xCurve.Evaluate(xTime) * _multiplier,
                y = _yCurve.Evaluate(yTime) * _multiplier,
                z = _zCurve.Evaluate(zTime) * _multiplier
            };
        }

        private float GetDuration()
        {
            float curvesDuration = 0f;

            curvesDuration = GetKeyTimeLargerThan(_xCurve, curvesDuration);
            curvesDuration = GetKeyTimeLargerThan(_yCurve, curvesDuration);
            curvesDuration = GetKeyTimeLargerThan(_zCurve, curvesDuration);

            return Mathf.Clamp(curvesDuration, 0f, MAX_DURATION);
        }

        private static float GetKeyTimeLargerThan(AnimationCurve animCurve, float largerThan)
        {
            if (animCurve.length == 0)
                return largerThan;

            float duration = animCurve.keys[animCurve.length - 1].time;
            return duration > largerThan ? duration : largerThan;
        }
    }
}