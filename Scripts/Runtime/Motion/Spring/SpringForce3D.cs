using System;
using UnityEngine;

namespace HatchStudio.Motion
{
    [Serializable]
    public struct SpringForce3D
    {
        public Vector3 Force;

        [Range(0f, 1f)]
        public float Duration;


        public static readonly SpringForce3D Default = new(Vector3.zero, 0.125f);

        public SpringForce3D(Vector3 force, float duration)
        {
            Force = force;
            Duration = Mathf.Max(0f, duration);
        }

        public bool IsEmpty() => Duration < 0.01f;

        public static SpringForce3D operator +(SpringForce3D springForce, Vector3 force)
        {
            springForce.Force += force;
            return springForce;
        }

        public static SpringForce3D operator *(SpringForce3D springForce, float mod)
        {
            springForce.Force *= mod;
            return springForce;
        }
    }
}