using HatchStudios.PlayerController;
using UnityEngine;

namespace HatchStudio.Motion
{
    public abstract class MotionBehaviour : CharacterBehaviour,IMixedMotion
    {
        [SerializeField, Range(0f, 10f)]
        private float _multiplier = 1f;
        
        public float Multiplier { get; set; }
        
        protected IMotionMixer _mixer;
        protected readonly Spring3D PositionSpring = new();
        protected readonly Spring3D RotationSpring = new();

        private void Awake()
        {
            _mixer = GetComponent<IMotionMixer>();
            PositionSpring.Adjust(GetDefaultPositionSpringSettings());
            RotationSpring.Adjust(GetDefaultRotationSpringSettings());
        }

        public abstract void UpdateMotion(float deltaTime);

        protected void SetTargetPosition(Vector3 target, float multiplier)
        {
            target *= _multiplier * multiplier ;
            PositionSpring.SetTargetValue(target);
        }
        protected void SetTargetPosition(Vector3 target)
        {
            target *= _multiplier;
            PositionSpring.SetTargetValue(target);
        }
        protected void SetTargetRotation(Vector3 target, float multiplier)
        {
            target *= _multiplier * multiplier ;
            RotationSpring.SetTargetValue(target);
        }
        protected void SetTargetRotation(Vector3 target)
        {
            target *= _multiplier ;
            RotationSpring.SetTargetValue(target);
        }
        public Vector3 GetPosition(float deltaTime)
        {
            Vector3 value = PositionSpring.Evaluate(deltaTime);
            return value;
        }

        public Quaternion GetRotation(float deltaTime)
        {
            Vector3 value = RotationSpring.Evaluate(deltaTime);
            return Quaternion.Euler(value);
        }

        
        protected override void OnEnable()
        {
            base.OnEnable();
            _mixer.AddMixedMotion(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _mixer.RemoveMixedMotion(this);
        }
        
        protected virtual SpringSettings GetDefaultPositionSpringSettings() => SpringSettings.Default;
        protected virtual SpringSettings GetDefaultRotationSpringSettings() => SpringSettings.Default;
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!Application.isPlaying || PositionSpring == null)
                return;

            PositionSpring.Adjust(GetDefaultPositionSpringSettings());
            RotationSpring.Adjust(GetDefaultRotationSpringSettings());
        }
#endif
    }
}