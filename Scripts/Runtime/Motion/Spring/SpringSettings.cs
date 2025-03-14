using System;
using UnityEngine;

namespace HatchStudio.Motion
{
    [Serializable]
    public struct SpringSettings
    {
        [Range(0f, 100f)]
        public float Damping;

        [Range(0f, 1000f)]
        public float Stiffness;

        [Range(0f, 10f)]
        public float Mass;

        [Range(0f, 10f)]
        public float Speed;


        public static readonly SpringSettings Default = new(10f, 120f, 1f, 1f);

        public SpringSettings(float damping, float stiffness, float mass, float speed)
        {
            Damping = damping;
            Stiffness = stiffness;
            Mass = mass;
            Speed = speed;
        }

        public readonly bool IsNull()
        {
            bool nullOrEmpty = Damping < 0.01f || Stiffness < 0.01f;
            return nullOrEmpty;
        }

        public override string ToString()
        {
            return $"Damping: {Damping} | Stiffness: {Stiffness} | Mass: {Mass} | Speed: {Speed}";
        }
    }
}