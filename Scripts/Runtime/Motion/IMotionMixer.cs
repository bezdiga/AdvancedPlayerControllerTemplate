using UnityEngine;

namespace HatchStudio.Motion
{
    public interface IMotionMixer
    {
        Transform TargetTransform { get; }
        float WeightMultiplier { get; set; }
        void AddMixedMotion(IMixedMotion mixedMotion);
        void RemoveMixedMotion(IMixedMotion motionBehaviour);
        T GetMotionOfType<T>() where T : class, IMixedMotion;
    }
}