using System.Collections;
using UnityEngine;

namespace Features.MovableModule.Scripts
{
    public class SimpleMovable : MovableBase {
        private const float RAYCAST_OFFSET = 0.5f;
        private const float RAYCAST_DISTANCE = 50f;
        private const float STOP_LANDING_DISTANCE_THRESHOLD = 0.5f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private float _maxGravityMultiplierValue = -15;
        [SerializeField] private float _gravityDelay = 0.05f; 
        [SerializeField] private float _directionSmoothSpeed;
        [SerializeField] private float _gravityDecreaseValue;
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
        private bool _isNearToGround;
        private float _distanceToGround;
        private float _groundTouchCount;
        private float _airTime = 0f; 

        public override float DistanceToGround => _distanceToGround;
        public override bool IsNearToGround => _isNearToGround;
        public override bool IsJumping => _isJumping;
        public override float GroundTouchCount => _groundTouchCount;
        public override float AngleToTarget => _angleToTarget;
        public override float GetVelocity => _rigidbody.linearVelocity.magnitude;
        public override float GetSpeed => _speed;

        public override void SetDirection(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                _direction = direction;
                return;
            }

            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, _groundLayerMask))
            {
                direction = Vector3.ProjectOnPlane(direction, hit.normal);
            }

            Vector3 forwardOrigin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(forwardOrigin + Vector3.up, direction.normalized, out hit, 1.0f, _groundLayerMask))
            {
                direction = Vector3.ProjectOnPlane(direction, hit.normal);
            }

            _direction = direction;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _direction * 2f);

            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 2f, _groundLayerMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal);
            }
            
            
            
            Vector3 rayOrigin = transform.position + Vector3.up * RAYCAST_OFFSET;
            Vector3 rayDirection = Vector3.down;

            // Нарисовать сам ray
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * RAYCAST_DISTANCE);

            // Нарисовать сферу, имитирующую SphereCast
            Gizmos.color = new Color(0, 1, 0, 0.3f); // зелёная полупрозрачная
            Gizmos.DrawWireSphere(rayOrigin + rayDirection * RAYCAST_DISTANCE, 0.2f);

            // Если что-то под ногами (попадание SphereCast)
            if (Physics.SphereCast(rayOrigin, 0.2f, rayDirection, out RaycastHit hit1, RAYCAST_DISTANCE, _groundLayerMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(hit1.point, 0.1f); // точка контакта
            }
        }



        public override void SetSpeed(float speed) => _speed = speed;
        public override void SetRotationSpeed(float speed) => _rotationSpeed = speed;
        public override void SetJumpDuration(float duration) => _jumpDuration = duration;
        public override void SetJumpMultiplier(float jumpMultiplier) => _jumpMultiplier = jumpMultiplier;
        
        public override void ResetHorizontalVelocity() => _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);

        private void ProcessGravityMultiplier()
        {
            //if (_isJumping) {
            //    _gravityMultiplier = 0;
            //    return;
            //}
            if (_distanceToGround > 0.01)
            {
                if (_gravityMultiplier >= _maxGravityMultiplierValue && !_isJumping)
                {
                    _gravityMultiplier = Mathf.Lerp(_gravityMultiplier, _maxGravityMultiplierValue, _gravityDecreaseValue * Time.deltaTime);
                }
                    //_gravityMultiplier-= _gravityDecreaseValue;
            }
            else
            {
                _gravityMultiplier = 0;
            }
        }

        public override void ProcessHorizontalMovement()
        {
            ProcessGravityMultiplier();
            Vector3 forwardVelocity = _direction * _speed;
            Vector3 directionVelocity;
            if (_groundTouchCount > 0)
            {
                _airTime = 0f; 
                directionVelocity = new Vector3(forwardVelocity.x, forwardVelocity.y, forwardVelocity.z);
            }
            else
            {
                _airTime += Time.fixedDeltaTime;

                if (_airTime >= _gravityDelay && !_isJumping)
                {
                    directionVelocity = new Vector3(forwardVelocity.x, forwardVelocity.y + _gravityMultiplier, forwardVelocity.z);
                }
                else
                {
                    directionVelocity = new Vector3(forwardVelocity.x, forwardVelocity.y, forwardVelocity.z);
                }
            }

            _rigidbody.linearVelocity = new(directionVelocity.x, Mathf.Lerp(_rigidbody.linearVelocity.y, directionVelocity.y, _directionSmoothSpeed * Time.fixedDeltaTime), directionVelocity.z);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & _groundLayerMask) != 0)
            {
                _groundTouchCount++;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & _groundLayerMask) != 0)
            {
                _groundTouchCount--;
            }
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
            _isNearToGround = GetIsNearToGround();
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
            //transform.forward = _direction.normalized;
            float startY = transform.position.y; 
            float time = 0;
            float jumpDuration = _jumpDuration; 
            float jumpMultiplier = _jumpMultiplier; 
            WaitForFixedUpdate waitForFixedUpdate = new();
            _isJumping = true;
            bool isFarFromGround = false;
            while (time <= jumpDuration && _isJumping) {
                if (!_isNearToGround)
                    isFarFromGround = true;

                if (isFarFromGround) {
                    if(_isNearToGround)
                        break;
                }
                time += Time.fixedDeltaTime;
                _velocityY = startY + _animationCurve.Evaluate(time / jumpDuration) * jumpMultiplier;
                yield return waitForFixedUpdate;
            }

            _isJumping = false;
        }
        
        private bool GetIsNearToGround() {
            Ray ray = new Ray(transform.position + Vector3.up * RAYCAST_OFFSET, Vector3.down);
            if (Physics.SphereCast(ray, 0.2f, out RaycastHit hit, RAYCAST_DISTANCE, _groundLayerMask)){    
                _distanceToGround = Vector3.Distance(hit.point, transform.position);
                if(_distanceToGround < STOP_LANDING_DISTANCE_THRESHOLD)
                    return true;
                
                return false;
            }

            _distanceToGround = 100;
            return false;
        }
    }
}
