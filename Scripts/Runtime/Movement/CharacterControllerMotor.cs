
using HatchStudio.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace HatchStudios.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMotor : MonoBehaviour,IMotor
    {
        public bool IsGrounded => _isGrounded;
        public float Gravity => _gravity;
        public Vector3 Velocity => _velocity;
        public float TurnSpeed => _turnSpeed;
        public Vector3 SimulatedVelocity => _simulatedVelocity;
        
        public float GroundSurfaceAngle => Vector3.Angle(Vector3.up, _groundNormal);
        public float SlopeLimit => _controller.slopeLimit;
        
        [SerializeField]
        [Tooltip("Gravity Strength")]
        private float _gravity = 9;
        [SerializeField, Range(0f, 10f)]
        [Tooltip("A force that will be applied to any rigidbody this motor will collide with.")]
        private float _pushForce = 1f;
        [SerializeField, Range(0f, 100f)]
        private float _groundingForce = 3f;
        
        [SerializeField]
        [Tooltip("Lowers/Increases the moving speed of the character when moving on sloped surfaces (i.e. lower speed when walking up a hill).")]
        private AnimationCurve _slopeSpeedMod;
        
        [SerializeField]
        [Tooltip("Layers that are considered obstacles.")]
        private LayerMask _collisionMask;
        
        private Vector3 _groundNormal;
        private Vector3 _velocity;
        private bool _applyGravity;
        private bool _snapToGround;
        private bool _isGrounded = true;
        private bool _disableSnapToGround;
        private float _defaultStepOffset;
        private float _lastYRotation;
        private float _turnSpeed;
        private Vector3 _simulatedVelocity;
        
        private RaycastHit _raycastHit;
        private CollisionFlags _collisionFlags;
        private CharacterController _controller;
        private MotionInputCallback _motionInput;
        private Transform _cachedTransform;
        
        private Vector3 velocity;
        private float _pitch;
        
        public event UnityAction Teleported;
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _cachedTransform = transform;
            _defaultStepOffset = _controller.stepOffset;
            _lastYRotation = transform.localEulerAngles.y;
        }
        
        public void Teleport(Vector3 position, Quaternion rotation, bool resetMotor = false)
        {
            _controller.enabled = false;

            rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
            position = new Vector3(position.x, position.y + _controller.skinWidth, position.z);
            _cachedTransform.SetPositionAndRotation(position, rotation);

            if (resetMotor)
                ResetMotor();

            _controller.enabled = true;

            Teleported?.Invoke();
        }
        private void OnEnable() => _controller.enabled = true;
        private void OnDisable() => _controller.enabled = false;

        public void SetMotionInput(MotionInputCallback motionInput) => _motionInput = motionInput;
        private void Update()
        {
            
            if (_motionInput == null)
                return;
            var deltaTime = Time.deltaTime;
            var wasGrounded = _isGrounded;
            var groundingForce = 0f;
            
            _simulatedVelocity = _motionInput.Invoke(_simulatedVelocity, out _applyGravity, out _snapToGround);//(transform.forward * moveZ + transform.right * moveX).normalized * speed * Time.deltaTime;
            
            if (!_disableSnapToGround && wasGrounded && _snapToGround)
            {
                // Grounding force...
                groundingForce = GetGroundingTranslation();
            }
            
            if (_applyGravity)
                _simulatedVelocity.y -= _gravity * deltaTime;
            
            Vector3 translation = _simulatedVelocity * deltaTime;
            translation.y -= groundingForce;
            _collisionFlags = _controller.Move(translation);
            
            _isGrounded = _controller.isGrounded;
            _velocity = _controller.velocity;

            
            float currentYRot = _cachedTransform.localEulerAngles.y;
            _turnSpeed = Mathf.Abs(currentYRot - _lastYRotation);
            _lastYRotation = currentYRot;
        }
        public void ResetMotor()
        {
            _groundNormal =  Vector3.zero;
            _velocity = _simulatedVelocity = Vector3.zero;
            _isGrounded = true;
        }
        public float GetSlopeSpeedMultiplier()
        {
            if (!_isGrounded)
                return 1f;

            // Make sure to lower the speed when ascending steep surfaces.
            float surfaceAngle = GroundSurfaceAngle;
            if (surfaceAngle > 5f)
            {
                bool isAscendingSlope = Vector3.Dot(_groundNormal, _simulatedVelocity) < 0f;

                if (isAscendingSlope)
                    return _slopeSpeedMod.Evaluate(surfaceAngle / SlopeLimit);
            }

            return 1f;
        }
        
        private float GetGroundingTranslation()
        {
            // Predict next world position
            float distanceToGround = 0.001f;

            var ray = new Ray(_cachedTransform.position, Vector3.down);
            if (PhysicsUtils.RaycastOptimized(ray, _defaultStepOffset, out _raycastHit, _collisionMask))
                distanceToGround = _raycastHit.distance;
            else
                _applyGravity = true;

            return distanceToGround * _groundingForce;
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _groundNormal = hit.normal;

            // make sure we hit a non kinematic rigidbody
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & _collisionMask.value) == 0) return;

            // We don't want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = new(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

            // Apply the push and take strength into account
            body.AddForce(pushDir * _pushForce, ForceMode.Impulse);
        }

    }
}