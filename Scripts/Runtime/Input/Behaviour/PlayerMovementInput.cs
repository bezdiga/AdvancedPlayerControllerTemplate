using HatchStudios.PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;


namespace HatchStudios.Input.Behaviours
{
    [AddComponentMenu("Input/Movement Input")]
    public class PlayerMovementInput : PlayerInputBehaviour, IMovementInputProvider
    {
        public InputContext DeathContext;
        public Vector2 RawMovementInput => _movementInputValue;
        public Vector3 MovementInput => Vector3.ClampMagnitude(_rootTransform.TransformVector(new Vector3(_movementInputValue.x, 0f, _movementInputValue.y)), 1f);

        public bool RunInputHold
        {
            get => _runHold;
            private set
            {
                _runHold = value;
            }
        }
        public bool CrouchInput
        {
            get => _crouchHold;
            private set
            {
                _crouchHold = value;
            }
        }
        
        public bool JumpInput { get; private set; }
        

        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private InputActionReference _runHoldInput;
        [SerializeField] private InputActionReference _crouchHoldInput;
        [SerializeField] private InputActionReference _jumpInput;

        
        [SerializeField, Range(0f, 1f)]
        private float _jumpReleaseDelay = 0.05f;
        
        private bool _runHold;
        private bool _crouchHold;
        
        private Transform _rootTransform;
        private float _releaseJumpBtnTime;
        private Vector2 _movementInputValue;


        protected override void Awake()
        {
            base.Awake();
            _rootTransform = transform.root;
        }

        protected override void OnBehaviourEnable(ICharacter character)
        {
            _moveInput.action.Enable();
            _runHoldInput.action.Enable();
            _crouchHoldInput.action.Enable();
            _jumpInput.action.Enable();
        }
        protected override void OnBehaviourDisable(ICharacter character)
        {
            _moveInput.action.Disable();
            _runHoldInput.action.Disable();
            _crouchHoldInput.action.Disable();
            _jumpInput.action.Disable();
        }
        
        public void UseCrouchInput() => CrouchInput = false;

        public void UseRunInput() => RunInputHold = false;

        public void UseJumpInput() => JumpInput = false;
        
        public void ResetAllInputs() 
        {
            RunInputHold = false;
            JumpInput = false;
            CrouchInput = false;
            _releaseJumpBtnTime = 0f;
            _movementInputValue = Vector2.zero;
        }
        public void Update()
        {
            if (!enabled) return;
            _movementInputValue = _moveInput.action.ReadValue<Vector2>();
            
            _runHold = _runHoldInput.action.ReadValue<float>() > 0.1f;
            _crouchHold = _crouchHoldInput.action.ReadValue<float>() > 0.1f;
            
            if (Time.time > _releaseJumpBtnTime || !JumpInput)
            {
                bool jumpInput = _jumpInput.action.triggered;
                JumpInput = jumpInput;

                if (jumpInput)
                    _releaseJumpBtnTime = Time.time + _jumpReleaseDelay;
            }
        }
    }
}