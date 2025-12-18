using System;
using System.Collections;
using Features.AnimationModule.Scriipts;
using UnityEngine;

namespace Features.MovableModule.Scripts
{
    public class TestMovementController : MonoBehaviour
    {
        private const float RAYCAST_OFFSET = 3f;
        private const float RAYCAST_DISTANCE = 20f;

        [Header("References")]
        [SerializeField] private SimpleAnimationController _simpleAnimationController;

        [SerializeField] private SimpleCharacterAnimationFunctionReactor _simpleCharacterAnimationFunctionReactor;
        [SerializeField] private SimpleMovable _movable;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private LayerMask _groundLayerMask;

        [Header("Movement")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpMultiplier = 2f;
        [SerializeField] private float _gravityMultiplierDistance = 2;
        [SerializeField] private float _maxGravityMultiplierValue = 15;
        [SerializeField] private float _distanceToEndJump;
        private Vector3 _inputDirection;
        private float _gravityMultiplier;
        private bool _isSimpleAttackEnded = true;
        [SerializeField] private float _animationSmoothSpeed;
        private float _smoothedVelocity;
        private bool _isMoving;
        private bool _isTurning;

        private void OnEnable()
        {
            _simpleCharacterAnimationFunctionReactor.OnSimpleAttackEnded += EndSimpleAttack;
            _simpleCharacterAnimationFunctionReactor.OnTurnEnded += EndTurn;
        }

        private void OnDisable() {
            _simpleCharacterAnimationFunctionReactor.OnSimpleAttackEnded -= EndSimpleAttack;
            _simpleCharacterAnimationFunctionReactor.OnTurnEnded -= EndTurn;
        }

        private void EndTurn() {
            if(!_isTurning)
                return;
            
            _simpleAnimationController.ResetTrigger("Turn");
            //_simpleAnimationController.SetLayerWeight("BattleTurns", 0);
            StartCoroutine(SmoothEndTurn(0.3f)); 
            
        }
        
        private IEnumerator SmoothEndTurn(float duration)
        {
            float elapsed = 0f;
            //float startWeight = _simpleAnimationController.GetLayerWeight("BattleTurns");
            float startWeight = 1;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float weight = Mathf.Lerp(startWeight, 0f, t);
                _simpleAnimationController.SetLayerWeight("BattleTurns", weight);
                yield return null;
            }

            _simpleAnimationController.SetLayerWeight("BattleTurns", 0f);
            _isTurning = false;
        }

        private void EndSimpleAttack()
        {
            _isSimpleAttackEnded = true;
        }

        private void Update() {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            _inputDirection = camForward * vertical + camRight * horizontal;
            float distanceToGround = GetDistanceToGround();

            if (Input.GetKeyDown(KeyCode.Space)) {
                _movable.Jump();
                _isSimpleAttackEnded = true;
                _simpleAnimationController.ResetTrigger("Run");
                _simpleAnimationController.SetTrigger("Jump");
            }

            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                _isSimpleAttackEnded = false;
                _simpleAnimationController.SetTrigger("SimpleSwordAttack");
            }

            ProcessGravityMultiplier(distanceToGround);

            if (distanceToGround < _distanceToEndJump && _movable.IsJumping) {
                _simpleAnimationController.SetTrigger("JumpEnded");
            }

            _isMoving = _movable.GetVelocity > 0f;

            if (_isMoving && !_movable.IsJumping && distanceToGround < _distanceToEndJump) {
                _simpleAnimationController.SetTrigger("Run");
                _isMoving = true;
                Debug.LogError(_movable.AngleToTarget);
                if (Mathf.Abs(_movable.AngleToTarget) > 170 && !_isTurning) {
                    _simpleAnimationController.SetLayerWeight("BattleTurns", 1);
                    _isTurning = true;
                    if (_movable.AngleToTarget > 0)
                        _simpleAnimationController.SetFloat("AngleToTarget", 1);
                    else {
                        _simpleAnimationController.SetFloat("AngleToTarget", -1);
                    }

                    _simpleAnimationController.SetTrigger("Turn");
                }
            }


            _smoothedVelocity = Mathf.Lerp(_smoothedVelocity, _movable.GetVelocity, Time.deltaTime * _animationSmoothSpeed);
            _simpleAnimationController.SetFloat("Velocity", _smoothedVelocity);
            _simpleAnimationController.SetFloat("RunAnimationSpeed", _movable.GetVelocity/_movable.GetSpeed);
            _simpleAnimationController.SetBool("IsSimpleAttackEnded", _isSimpleAttackEnded);
                

        }
        
        private void ProcessGravityMultiplier(float distanceToGround) {
            if (distanceToGround >= _gravityMultiplierDistance)
            {
                if(_gravityMultiplier >= _maxGravityMultiplierValue)
                    _gravityMultiplier--;
                
                _movable.SetGravityMultiplier(_gravityMultiplier);
            }
            else
            {
                _gravityMultiplier = 0;
                _movable.SetGravityMultiplier(_gravityMultiplier);
            }
        }

        private void FixedUpdate()
        {
            _movable.SetDirection(_inputDirection.normalized);
            _movable.SetSpeed(_speed);
            _movable.SetJumpDuration(_jumpDuration);
            _movable.SetJumpMultiplier(_jumpMultiplier);
            _movable.SetRotationSpeed(_rotationSpeed);
            //if(_isSimpleAttackEnded)
            //    _movable.ProcessHorizontalMovement();
            //else
            //    _movable.ResetHorizontalVelocity();
            _movable.UpdateAngle();
            _movable.ProcessDirectionRotation();
            _movable.ProcessHorizontalMovement();
            _movable.ProcessVerticalMovement();
        }

        private float GetDistanceToGround()
        {
            Ray ray = new Ray(transform.position + Vector3.up * RAYCAST_OFFSET, Vector3.down);
            if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, RAYCAST_DISTANCE, _groundLayerMask))
            {
                return Vector3.Distance(hit.point, transform.position);
            }

            return RAYCAST_DISTANCE;
        }
    }
}
