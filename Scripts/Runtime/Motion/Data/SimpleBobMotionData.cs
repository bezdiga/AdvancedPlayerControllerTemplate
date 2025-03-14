using UnityEngine;

namespace HatchStudio.Motion.Data
{
    [CreateAssetMenu(menuName = MOTION_DATA_MENU_PATH + "Simple Bob",fileName = "Bob_")]
    public class SimpleBobMotionData : MotionData,IBobMotionData
    {
        
        [SerializeField]
        private BobMode _bobType = BobMode.StepCycleBased;

        [SerializeField, Range(0.01f, 10f)]
        
        private float _bobSpeed = 1f;

        [SerializeField]
        private SpringSettings _positionSpring = SpringSettings.Default;

        [SerializeField]
        private SpringForce3D _positionStepForce = SpringForce3D.Default;

        [SerializeField]
        private Vector3 _positionAmplitude = Vector3.zero;

        [SerializeField]
        private SpringSettings _rotationSpring = SpringSettings.Default;

        [SerializeField]
        private SpringForce3D _rotationStepForce = SpringForce3D.Default;

        [SerializeField]
        private Vector3 _rotationAmplitude = Vector3.zero;

        public BobMode BobType => _bobType;
        public Vector3 PositionAmplitude => _positionAmplitude;
        public Vector3 RotationAmplitude => _rotationAmplitude;

        public SpringSettings PositionSettings => _positionSpring;
        public SpringSettings RotationSettings => _rotationSpring;
        public SpringForce3D PositionStepForce => _positionStepForce;
        public SpringForce3D RotationStepForce => _rotationStepForce;
        public float BobSpeed => _bobSpeed;
    }
}