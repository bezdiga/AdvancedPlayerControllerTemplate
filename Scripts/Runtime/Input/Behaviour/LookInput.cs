
using HatchStudios.CameraController;
using HatchStudios.PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatchStudios.Input.Behaviours
{
    [AddComponentMenu("Input/Look Input")]
    public class LookInput : PlayerInputBehaviour
    {
        [SerializeField]
        private InputActionReference _lookInput;
        
        private ILookHandlerCC _lookHandler;
        
        protected override void OnBehaviourStart(ICharacter character)
        {
            _lookHandler = character.GetCComponent<ILookHandlerCC>();
        }
        protected override void OnBehaviourEnable(ICharacter character)
        {
            _lookInput.action.Enable();
            _lookHandler.SetLookInput(GetInput);
        }

        protected override void OnBehaviourDisable(ICharacter character)
        {
            _lookInput.action.Disable();
            _lookHandler.SetLookInput(null);
        }
        
        private Vector2 GetInput()
        {
            Vector2 lookInput = _lookInput.action.ReadValue<Vector2>();
            (lookInput.x, lookInput.y) = (lookInput.y, lookInput.x);
            return lookInput;
        }
    }
}