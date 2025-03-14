using HatchStudio.Manager;
using HatchStudios.PlayerController;
using HatchStudios.ToolBox;
using UnityEngine;

namespace HatchStudios.CameraController
{
    public class CharacterLookHandler : CharacterBehaviour, ILookHandlerCC
    {
        [SerializeField]
        [Tooltip("Optional: Used in lowering/increasing the current sensitivity based on the FOV")]
        private Camera _camera;
        
        [SerializeField, NotNull]
        [Tooltip("Transform to rotate Up & Down.")]
        private Transform _xTransform;
        
        [SerializeField, NotNull]
        [Tooltip("Transform to rotate Left & Right.")]
        private Transform _yTransform;
        
        [SerializeField]
        [Tooltip("Vertical look limits (in angles).")]
        private Vector2 _lookLimits = new(-60f, 90f);
        
        private LookHandlerInputDelegate _additiveInput;
        private LookHandlerInputDelegate _input;
        private bool _hasFOVCamera;
        private float _sensitivity;
        
        public Vector2 ViewAngles { get; private set; }
        public Vector2 LookInput { get; private set; }
        public Vector2 LookDelta { get; private set; }
        
        public void SetLookInput(LookHandlerInputDelegate input)
        {
            Debug.LogError("Set look input");
            enabled = input != null;
            _input = input;
        }

        public void SetAdditiveLookInput(LookHandlerInputDelegate input) => _additiveInput = input;
        
        protected override void OnBehaviourStart(ICharacter character)
        {
            if (!_xTransform)
            {
                Debug.LogError("Assign the X Transform in the inspector!", gameObject);
                return;
            }

            if (!_yTransform)
            {
                Debug.LogError("Assign the Y Transform in the inspector!", gameObject);
                return;
            }

            _hasFOVCamera = _camera != null;
        }
        
        protected override void OnBehaviourEnable(ICharacter character)
        {
            if (character.TryGetCComponent(out IMotor motor))
                motor.Teleported += OnTeleport;
        }
        protected override void OnBehaviourDisable(ICharacter character)
        {
            if (character.TryGetCComponent(out IMotor motor))
                motor.Teleported -= OnTeleport;
        }
        
        private void OnTeleport() => ViewAngles = new Vector2(0f, _yTransform.localEulerAngles.y);
        
        private void LateUpdate()
        {
            _sensitivity = GetTargetSensitivity(_sensitivity, Time.deltaTime);
            
            LookInput = _input() * _sensitivity;
            Vector2 additiveInput = _additiveInput?.Invoke() ?? Vector2.zero;
            
            MoveView(LookInput, additiveInput);
        }
        private float GetTargetSensitivity(float currentSens, float deltaTime)
        {
            float targetSensitivity = InputOptions.Instance.MouseSensitivity;
            targetSensitivity *= _hasFOVCamera ? _camera.fieldOfView / GraphicsOptions.Instance.FieldOfView : 1f;
            return Mathf.MoveTowards(currentSens, targetSensitivity, deltaTime * 5f);
        }
        
        private void MoveView(Vector2 lookInput, Vector2 additiveInput)
        {
            Vector2 prevViewAngles = ViewAngles;
            Vector2 newViewAngles = ViewAngles;

            newViewAngles.x += lookInput.x * (InputOptions.Instance.InvertMouse ? 1f : -1f);
            newViewAngles.y += lookInput.y;

            newViewAngles.x = ClampAngle(newViewAngles.x);
            LookDelta = new Vector2(newViewAngles.x - prevViewAngles.x, newViewAngles.y - prevViewAngles.y);

            var viewAngle = new Vector2
            {
                x = ClampAngle(newViewAngles.x + additiveInput.x),
                y = newViewAngles.y + additiveInput.y
            };

            ViewAngles = newViewAngles;
            _yTransform.localRotation = Quaternion.Euler(0f, viewAngle.y, 0f);
            _xTransform.localRotation = Quaternion.Euler(viewAngle.x, 0f, 0f);
        }
        
        private float ClampAngle(float angle) => Mathf.Clamp(angle, _lookLimits.x, _lookLimits.y);
    }
}