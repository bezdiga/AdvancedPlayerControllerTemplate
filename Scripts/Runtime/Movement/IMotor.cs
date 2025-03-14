using UnityEngine;
using UnityEngine.Events;

namespace HatchStudios.PlayerController
{
    public interface IMotor : ICharacterComponent
    {
        event UnityAction Teleported;
        public bool IsGrounded { get; }
        float Gravity { get; }
        Vector3 Velocity { get; }
        float TurnSpeed { get; }
        public Vector3 SimulatedVelocity { get; }
        float GroundSurfaceAngle { get; }
        public float SlopeLimit { get; }
        float GetSlopeSpeedMultiplier();
        public void SetMotionInput(MotionInputCallback motionInput);
    }

    public delegate Vector3 MotionInputCallback(Vector3 velosity, out bool useGravity, out bool snapToGround);
}