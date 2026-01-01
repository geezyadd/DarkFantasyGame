using System;
using System.Collections;
using UnityEngine;

namespace Features.MovableModule.Scripts
{
    public class SimpleMovable : MovableBase {
        private const float RAYCAST_OFFSET = 3f;
        private const float RAYCAST_DISTANCE = 20f;
        private const float STOP_LANDING_DISTANCE_THRESHOLD = 0.05f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private LayerMask _groundLayerMask;
        private Vector3 _direction;
        private bool _isJumping;
        private float _speed;
        private float _velocityY;
        private float _rotationSpeed;
        private float _jumpDuration;
        private float _jumpMultiplier;
        private float _gravityMultiplier;
        private float _angleToTarget;
        private Vector3 _lastDirection;

        public override bool IsJumping => _isJumping;
        public override float AngleToTarget => _angleToTarget;
        public override float GetVelocity => _rigidbody.linearVelocity.magnitude;
        public override float GetSpeed => _speed;

        public override void SetDirection(Vector3 direction)
        {
            if(direction == Vector3.zero)
                return;
            
            _direction = direction;
        }

        public override void SetSpeed(float speed) => _speed = speed;
        public override void SetRotationSpeed(float speed) => _rotationSpeed = speed;
        public override void SetJumpDuration(float duration) => _jumpDuration = duration;
        public override void SetJumpMultiplier(float jumpMultiplier) => _jumpMultiplier = jumpMultiplier;
        
        public override void ResetHorizontalVelocity() => _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);

        public override void SetGravityMultiplier(float gravityMultiplier)
        {
            if (_isJumping) {
                _gravityMultiplier = 0;
                return;
            }

            _gravityMultiplier = gravityMultiplier;
        }

        public override void ProcessHorizontalMovement()
        {
            Vector3 forwardVelocity = _direction * _speed;
            if (!_isJumping)
                _rigidbody.linearVelocity = new Vector3(forwardVelocity.x, _gravityMultiplier, forwardVelocity.z);
        }
        
        public override void UpdateAngle()
        {
            //if (_direction.sqrMagnitude < 0.001f)
            //    return;

            _angleToTarget = Vector3.SignedAngle(
                transform.forward,
                _direction,
                Vector3.up
            );
        }

        public override void ProcessDirectionRotation()
        {
            if (_direction.sqrMagnitude < 0.001f || _direction == Vector3.zero)
                return;
            
            Vector3 flatDirection = new Vector3(_direction.x, 0f, _direction.z);
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

            Quaternion newRotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRotation,
                _rotationSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(newRotation);
        }

        public override void ProcessVerticalMovement()
        {
            Vector3 forwardVelocity = _direction.normalized * _speed;
            if (_isJumping)
            {
                float yVelocity = (_velocityY - _rigidbody.position.y) / Time.fixedDeltaTime;

                _rigidbody.linearVelocity = new Vector3(forwardVelocity.x, yVelocity, forwardVelocity.z);
            }
        }

        public override void Jump()
        {
            if(_isJumping)
                return;

            StartCoroutine(JumpCoroutine());
        }
        
        private IEnumerator JumpCoroutine()
        {
            transform.forward = _direction.normalized;
            float startY = transform.position.y; 
            float time = 0;
            float jumpDuration = _jumpDuration; 
            float jumpMultiplier = _jumpMultiplier; 
            WaitForFixedUpdate waitForFixedUpdate = new();
            _isJumping = true;
            bool isFarFromGround = false;
            while (time <= jumpDuration && _isJumping) {
                if (!IsNearToGround())
                    isFarFromGround = true;

                if (isFarFromGround) {
                    if(IsNearToGround())
                        break;
                }
                time += Time.fixedDeltaTime;
                _velocityY = startY + _animationCurve.Evaluate(time / jumpDuration) * jumpMultiplier;
                yield return waitForFixedUpdate;
            }

            _isJumping = false;
        }
        
        private bool IsNearToGround() {
            Ray ray = new Ray(transform.position + Vector3.up * RAYCAST_OFFSET, Vector3.down);
            if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, RAYCAST_DISTANCE, _groundLayerMask)){    
                if(Vector3.Distance(hit.point, transform.position) < STOP_LANDING_DISTANCE_THRESHOLD)
                    return true;
            }

            return false;
        }
    }
}
