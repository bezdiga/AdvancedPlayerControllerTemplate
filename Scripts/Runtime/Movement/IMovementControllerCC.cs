using UnityEngine.Events;

namespace HatchStudios.PlayerController
{
    public interface IMovementControllerCC : ICharacterComponent
    {
        public float StepCycle { get; }
        event UnityAction StepCycleEnded;
        event UnityAction<MovementStateType> OnChangeState;
    }
    
}