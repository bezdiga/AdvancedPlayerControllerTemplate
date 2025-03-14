using HatchStudio.Utilities;
using HatchStudios.PlayerController;
using UnityEngine;

namespace HatchStudio.Surface
{
    public class FootStepsController : CharacterBehaviour
    {
        private enum Footstep { Left, Right }
        
        [SerializeField, Range(0.01f, 1f)]
        private float _raycastDistance = 0.3f;
        [SerializeField, Range(0.01f, 0.5f)]
        private float _raycastRadius = 0.3f;
        
        [SerializeField, Range(0f, 24f)]
        private float _minVolumeSpeed = 1f;

        [SerializeField, Range(0f, 24f)] 
        private float _maxVolumeSpeed = 7f;
        
        [SerializeField, Range(0f, 25f)]
        [Tooltip("If the impact speed is higher than this threshold, an effect will be played.")]
        private float _fallImpactThreshold = 5f;

        [SerializeField, Range(0f, 25f)]
        [Tooltip("If the impact speed is at this threshold, the fall impact effect audio will be at full effect.")]
        private float _maxFallImpactThreshold = 12f;
        
        private SurfaceDefinition _lastSurface;
        private IMovementControllerCC _movement;
        private Footstep _lastFootDown;
        private float _fallImpactTimer;
        private bool _hasLastSurface;
        private IMotor _motor;
        
        private SurfaceDefinition LastSteppedSurface
        {
            set
            {
                _lastSurface = value;
                _hasLastSurface = _lastSurface != null;
            }
        }
        protected override void OnBehaviourStart(ICharacter character)
        {
            _movement = character.GetCComponent<IMovementControllerCC>();
            _motor = character.GetCComponent<IMotor>();
        }

        protected override void OnBehaviourEnable(ICharacter character)
        {
            _movement.StepCycleEnded += PlayFootstepEffect;
            
            _movement.SpeedModifier.AddModifier(GetVelocity);
            _movement.AccelerationModifier.AddModifier(GetAcceleration);
            _movement.DecelerationModifier.AddModifier(GetDeceleration);
        }
        protected override void OnBehaviourDisable(ICharacter character)
        {
            _movement.StepCycleEnded -= PlayFootstepEffect;

            _movement.SpeedModifier.RemoveModifier(GetVelocity);
            _movement.AccelerationModifier.RemoveModifier(GetAcceleration);
            _movement.DecelerationModifier.RemoveModifier(GetDeceleration);
        }
        private float GetVelocity() => _hasLastSurface ? _lastSurface.VelocityModifier : 1f;
        private float GetAcceleration() => _hasLastSurface ? _lastSurface.SurfaceFriction : 1f;
        private float GetDeceleration() => _hasLastSurface ? _lastSurface.SurfaceFriction : 1f;
        
        private void PlayFootstepEffect()
        {
            MovementStateType stateType = _movement.ActiveStateType;
            
            if (CheckGround(out RaycastHit hitInfo))
            {
                _lastFootDown = _lastFootDown == Footstep.Left ? Footstep.Right : Footstep.Left;
                float volumeMod = Mathf.Clamp(_motor.Velocity.magnitude + _motor.TurnSpeed, _minVolumeSpeed, _maxVolumeSpeed) / _maxVolumeSpeed;
                LastSteppedSurface = SurfaceManager.Instance.SpawnEffectFromHit(ref hitInfo, GetEffectType(stateType), volumeMod);
            }
        }
        
        private bool CheckGround(out RaycastHit hitInfo)
        {
            var ray = new Ray(transform.position + Vector3.up * 0.3f, Vector3.down);

            const int MASK = LayerConstants.SIMPLE_SOLID_OBJECTS_MASK | LayerConstants.INTERACTABLES_MASK;
            
            bool hitSomething = Physics.Raycast(ray, out hitInfo, _raycastDistance, MASK, QueryTriggerInteraction.Ignore);

            if (!hitSomething)
                hitSomething = Physics.SphereCast(ray, _raycastRadius, out hitInfo, _raycastDistance, MASK, QueryTriggerInteraction.Ignore);

            return hitSomething;
        }
        
        private static SurfaceEffectType GetEffectType(MovementStateType stateType)
        {
            bool isRunning = stateType == MovementStateType.Run;
            SurfaceEffectType footstepType = isRunning ? SurfaceEffectType.HardFootstep : SurfaceEffectType.SoftFootstep;

            return footstepType;
        }
    }
}