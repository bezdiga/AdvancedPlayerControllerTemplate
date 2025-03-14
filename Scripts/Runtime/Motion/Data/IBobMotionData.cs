using UnityEngine;

namespace HatchStudio.Motion.Data
{
    public interface IBobMotionData : IMotionData
    {
        public BobMode BobType { get; }
        public Vector3 PositionAmplitude { get; }
        public Vector3 RotationAmplitude { get; }
        public SpringSettings PositionSettings { get; }
        public SpringSettings RotationSettings { get; }
        public SpringForce3D PositionStepForce { get; }
        public SpringForce3D RotationStepForce { get; }
        public float BobSpeed { get; }
    }
}