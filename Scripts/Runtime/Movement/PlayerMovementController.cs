using System;
using System.Collections.Generic;
using HatchStudio.StateMachine;
using HatchStudio.Utilities;
using HatchStudios.Input.Behaviours;
using HatchStudios.ToolBox;
using UnityEngine;
using UnityEngine.Events;

namespace HatchStudios.PlayerController
{
    public class PlayerMovementController : CharacterBehaviour,IMovementControllerCC
    {
        public float StepCycle { get; private set; }
        
        [SerializeField] private CharacterControllerMotor _motor;
        [SerializeField,NotNull] private PlayerMovementInput _inputHandler;
        
        
        private StateMachine<CharacterMovementState,StateData<PlayerStateAsset>,PlayerStateAsset> _stateMachine;
        public StateData<PlayerStateAsset> CurrentState { get; private set; }
        public StateData<PlayerStateAsset> PreviousState { get; private set; }
        
        [SerializeField, Range(0.1f, 10f)]
        [Tooltip("Speed of transition between different step lengths.")]
        private float _stepLerpSpeed = 1.5f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("Step length when turning.")]
        private float _turnStepLength = 0.8f;
        
        [SerializeField, Range(0f, 10f)]
        [Tooltip("Maximum step velocity when turning.")]
        private float _maxTurnStepVelocity = 1.75f;
        
        [SerializeField, Range(0f, 10f)]
        [Tooltip("Multiplier for movement speed.")]
        private float _speedMultiplier = 1f;

        [SerializeField, Range(1f, 100f)]
        private float _baseAcceleration = 8f;
    
        [SerializeField, Range(1f, 100f)]
        private float _baseDeceleration = 10f;
        
        public MovementModifierGroup SpeedModifier { get; private set; }
        public MovementModifierGroup AccelerationModifier { get; private set; }
        public MovementModifierGroup DecelerationModifier { get; private set; }
        
        [SerializeField,NotNull]
        private PlayerStatesGroup _playerStatesGroup;
        
        [HideInInspector] public PlayerStatesGroup StateAssetRuntime;
        private Dictionary<CharacterMovementState,StateData<PlayerStateAsset>> _playerStates = new Dictionary<CharacterMovementState, StateData<PlayerStateAsset>>();

        private CharacterMovementState activeState;
        public event UnityAction StepCycleEnded;
        public event UnityAction<MovementStateType> OnChangeState;
        
        private float _distMovedSinceLastCycleEnded;
        private float _currentStepLength = 1f;
        private void Awake()
        {
            SpeedModifier = new MovementModifierGroup(_speedMultiplier, SpeedModifier);
            AccelerationModifier = new MovementModifierGroup(_baseAcceleration, AccelerationModifier);
            DecelerationModifier = new MovementModifierGroup(_baseDeceleration, DecelerationModifier);
        }
        
        protected override void OnBehaviourStart(ICharacter player)
        {
            _stateMachine = new StateMachine<CharacterMovementState,StateData<PlayerStateAsset>,PlayerStateAsset>(Enum.GetNames(typeof(MovementStateType)));
            _stateMachine.GetStateData += (stateData) =>
            {
                _playerStates.TryGetValue(stateData, out StateData<PlayerStateAsset> assetData);
                return assetData;
            };
            _stateMachine.OnChangeState += StateChanged;
            if (_playerStatesGroup == null)
            {
                Debug.LogError("PlayerStateMachine: Player State Group is not set");
                return;
            }
            StateAssetRuntime = Instantiate(_playerStatesGroup);
            
            foreach (var state in StateAssetRuntime.GetState(this,_inputHandler,_motor))
            {
                _playerStates[state.CharacterMovementState] = state.stateData;
                _stateMachine.SetStateTransitions(state.stateData.stateAsset.StateKey,state.CharacterMovementState,state.CharacterMovementState.Transitions);
            }
            
            _stateMachine.TrySetState(MovementStateType.Idle.ToString());
            
            void StateChanged(CharacterMovementState prevState, CharacterMovementState currentState)
            {
                if(prevState != null)
                    PreviousState = _playerStates[prevState];
                if (currentState != null)
                {
                    CurrentState = _playerStates[currentState];
                    activeState = currentState;
                }
                
                OnChangeState?.Invoke(activeState.StateType);
            }
        }
        protected override void OnBehaviourEnable(ICharacter character) => _motor.SetMotionInput(GetMotionInput);

        private Vector3 GetMotionInput(Vector3 velocity, out bool useGravity, out bool snapToGround)
        {
            float deltaTime = Time.deltaTime;
            useGravity = activeState.ApplyGravity;
            snapToGround = activeState.SnapToGround;
            
            var newVelocity = activeState.UpdateVelocity(velocity, deltaTime);
            
            UpdateStepCycle(deltaTime);
            
            _stateMachine.Tick();
            return newVelocity;
        }
        
        #region Step Cycle
        private void UpdateStepCycle(float deltaTime)
        {
            if (!_motor.IsGrounded)
                return;

            // Advance the step cycle based on the current velocity.
            _distMovedSinceLastCycleEnded += _motor.Velocity.GetHorizontal().magnitude * deltaTime;
            float targetStepLength = Mathf.Max(activeState.StepCycleLength, 1f);
            _currentStepLength = Mathf.MoveTowards(_currentStepLength, targetStepLength, deltaTime * _stepLerpSpeed);

            // Advance the step cycle based on the character turn.
            _distMovedSinceLastCycleEnded += Mathf.Clamp(_motor.TurnSpeed, 0f, _maxTurnStepVelocity) * deltaTime * _turnStepLength;

            // If the step cycle is complete, reset it, and send a notification.
            if (_distMovedSinceLastCycleEnded > _currentStepLength)
            {
                _distMovedSinceLastCycleEnded -= _currentStepLength;
                StepCycleEnded?.Invoke();
                Debug.LogError("EndStepp");
            }

            StepCycle = _distMovedSinceLastCycleEnded / _currentStepLength;
        }
        #endregion
    }
}