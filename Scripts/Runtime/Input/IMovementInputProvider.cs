using UnityEngine;

namespace HatchStudios.PlayerController
{
    public interface IMovementInputProvider
    {
        Vector2 RawMovementInput { get; }
        
        Vector3 MovementInput { get; }
        bool RunInputHold { get; }
        bool CrouchInput { get; }
        bool JumpInput { get; }

        void UseCrouchInput();
        void UseRunInput();
        void UseJumpInput();
    }
}