using UnityEngine.Events;

namespace HatchStudios.PlayerController
{
    public interface IMovementControllerCC : ICharacterComponent
    {
        public float StepCycle { get; }
        
        public MovementStateType ActiveStateType { get; }
        public MovementModifierGroup SpeedModifier { get;}
        public MovementModifierGroup AccelerationModifier { get;}
        public MovementModifierGroup DecelerationModifier { get;}
        event UnityAction StepCycleEnded;
        event UnityAction<MovementStateType> OnChangeState;
    }
    
}