using System;
using System.Collections;
using Features.AnimationModule.Scriipts;
using Features.CameraModule.Scripts;
using Features.WeaponModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.MovableModule.Scripts
{
    public class TestMovementController : MonoBehaviour
    {
        private const float RAYCAST_OFFSET = 3f;
        private const float RAYCAST_DISTANCE = 20f;
        [Header("Combat")] 
        [SerializeField] private WeaponBase _weapon;
        [SerializeField] private float _attackMovementSpeedDecreaseValue;
        private Coroutine _endAttackLayerCoroutine;
        private bool _attackStarted;
        
        [Header("References")]
        [SerializeField] private SimpleAnimationController _simpleAnimationController;

        [SerializeField] private SimpleCharacterAnimationFunctionReactor _simpleCharacterAnimationFunctionReactor;
        [SerializeField] private SimpleMovable _movable;
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
        private bool _isMoving;
        private bool _isTurning;
        private float _smoothedVelocity;
        private Coroutine _endTurnsLayerCoroutine;
        private float _cachedSpeed;
        private float _lastSmoothedVelocity;
        private bool _isStartStopAnimation;
        private Coroutine _endStartStopCoroutine;
        [SerializeField] private float _animationSmoothSpeed;
        [SerializeField] private float _stopRunAnimationVelocityBorder;
        private Vector3 _currentSmoothedDirection;
        [SerializeField] private float _velocitySmoothSpeed;
        private bool _startStopRunTriggered;
        [SerializeField] private float _startRunAnimationVelocityBorder;
        
        private CameraModel _cameraModel;

        [Inject]
        private void InjectDependencies(CameraModel cameraModel)
        {
            _cameraModel = cameraModel;
        }
        
        private void OnEnable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _cachedSpeed = _speed;
            _simpleCharacterAnimationFunctionReactor.OnAttack += HandleAttack;
            _simpleCharacterAnimationFunctionReactor.OnEndAttack += HandleAttackEnd;
        }

        private void OnDisable() {
            _simpleCharacterAnimationFunctionReactor.OnAttack -= HandleAttack;
            _simpleCharacterAnimationFunctionReactor.OnEndAttack -= HandleAttackEnd;
        }

        private void HandleAttack()
        {
            _weapon.Attack();
        }

        private void HandleAttackEnd()
        {
            _weapon.StopAttack();
        }


        private IEnumerator SmoothLayerToZero(string layerName, float duration, Action onEnd = null)
        {
            float elapsed = 0f;
            float startWeight = _simpleAnimationController.GetLayerWeight(layerName);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float weight = Mathf.Lerp(startWeight, 0f, t);
                _simpleAnimationController.SetLayerWeight(layerName, weight);
                yield return null;
            }

            _simpleAnimationController.SetLayerWeight(layerName, 0f);
            if(onEnd == null)
                yield break;
            
            onEnd();
        }

        private void Update() {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 camForward = _cameraModel.CurrentCamera.transform.forward;
            Vector3 camRight = _cameraModel.CurrentCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            _inputDirection = camForward * vertical + camRight * horizontal;
            float distanceToGround = GetDistanceToGround();

            if (Input.GetKeyDown(KeyCode.Space) && !_attackStarted) {
                _movable.Jump();
                _simpleAnimationController.ResetTrigger("Run");
                _simpleAnimationController.SetTrigger("Jump");
            }

            CombatController();
            HandleAttackEnded();
            ProcessGravityMultiplier(distanceToGround);

            if (distanceToGround < _distanceToEndJump && _movable.IsJumping) {
                _simpleAnimationController.SetTrigger("JumpEnded");
            }

            _isMoving = _movable.GetVelocity > 0f;

            if (_isMoving && !_movable.IsJumping && distanceToGround < _distanceToEndJump) {
                _simpleAnimationController.SetTrigger("Run");
                _isMoving = true;
            }
            
            HandleTurns();

            HandleEndTurn();
            _lastSmoothedVelocity = _smoothedVelocity;
            _smoothedVelocity = Mathf.Lerp(_smoothedVelocity, _movable.GetVelocity, Time.deltaTime * _animationSmoothSpeed);
            if (_smoothedVelocity < 0.01)
            {
                _smoothedVelocity = 0;
            }
            
            HandleStopRun();
            //HandleStartRun();
            HandleStopStartRunEnd();
            
            _simpleAnimationController.SetFloat("Velocity", _smoothedVelocity);
            _simpleAnimationController.SetFloat("RunAnimationSpeed", _movable.GetVelocity/_movable.GetSpeed);

            //if (_attackStarted)
            //{
            //    _speed -= _attackMovementSpeedDecreaseValue;
            //    if (_speed < 0.01)
            //        _speed = 0.01f;
            //}
                
        }

        private void HandleTurns()
        {
            if (Mathf.Abs(_movable.AngleToTarget) > 100 && !_isTurning) {
                if(_endTurnsLayerCoroutine != null) StopCoroutine(_endTurnsLayerCoroutine);
                _simpleAnimationController.SetLayerWeight("BattleTurns", 1);
                if (_movable.AngleToTarget > 0)
                    _simpleAnimationController.SetFloat("AngleToTarget", 1);
                else {
                    _simpleAnimationController.SetFloat("AngleToTarget", -1);
                }

                _simpleAnimationController.SetTrigger("Turn");
                StartCoroutine(WaitOneFrameToStartCheckTurnsEnd());
            }
        }

        private void HandleStopStartRunEnd()
        {
            if(!_isStartStopAnimation)
                return;
            
            AnimatorStateInfo nextAnimationState = _simpleAnimationController.GetNextAnimatorStateInfo("BattleStartStopRun");
            AnimatorStateInfo currentAnimationState = _simpleAnimationController.GetCurrentAnimatorStateInfo("BattleStartStopRun");
            if ((nextAnimationState.IsTag("StartStopRun") || currentAnimationState.IsTag("StartStopRun")) && !_movable.IsJumping)
                return;

            _simpleAnimationController.ResetTrigger("StartRun");
            _simpleAnimationController.ResetTrigger("StopRun");
            _endStartStopCoroutine = StartCoroutine(SmoothLayerToZero("BattleStartStopRun",0.3f, () => _startStopRunTriggered = false));
            _isStartStopAnimation = false;
        }

        private void HandleStopRun()
        {
            if(_isStartStopAnimation)
                return;
            if (!(_smoothedVelocity < _stopRunAnimationVelocityBorder)) return;
            HandleStopRunAnimation();
            StartCoroutine(WaitOneFrameToCheckStartStopEnd());
        }

        private void HandleStopRunAnimation()
        {
            if(_startStopRunTriggered)
                return;

            if (_lastSmoothedVelocity < _smoothedVelocity) return;
            
            if (Mathf.Approximately(_lastSmoothedVelocity, _smoothedVelocity)) return;
            
            _startStopRunTriggered = true;
            _simpleAnimationController.SetLayerWeight("BattleStartStopRun", 1);
            _simpleAnimationController.SetTrigger("StopRun");
        }
        
        private void HandleStartRun()
        {
            if(_isStartStopAnimation)
                return;
            if (_smoothedVelocity > _startRunAnimationVelocityBorder) return;
            HandleStartRunAnimation();
            StartCoroutine(WaitOneFrameToCheckStartStopEnd());
        }

        private void HandleStartRunAnimation()
        {
            if(_startStopRunTriggered)
                return;
            
            if (_lastSmoothedVelocity >= _smoothedVelocity) return;
            
            //if(_endStartStopCoroutine != null) StopCoroutine(_endStartStopCoroutine);
            
            _startStopRunTriggered = true;
            _simpleAnimationController.SetLayerWeight("BattleStartStopRun", 1);
            _simpleAnimationController.SetTrigger("StartRun");
        }
        
        private IEnumerator WaitOneFrameToCheckStartStopEnd()
        {
            yield return null;
            _isStartStopAnimation = true;
        }

        private IEnumerator WaitOneFrameToStartCheckTurnsEnd()
        {
            yield return null;
            _isTurning = true;
        }


        private void HandleEndTurn() {
            if(!_isTurning)
                return;
            
            AnimatorStateInfo nextAnimationState = _simpleAnimationController.GetNextAnimatorStateInfo("BattleTurns");
            AnimatorStateInfo currentAnimationState = _simpleAnimationController.GetCurrentAnimatorStateInfo("BattleTurns");
            if ((nextAnimationState.IsTag("Turns") || currentAnimationState.IsTag("Turns")) && !_movable.IsJumping)
                return;

            _simpleAnimationController.ResetTrigger("Turn");
            _endTurnsLayerCoroutine = StartCoroutine(SmoothLayerToZero("BattleTurns",0.2f));
            _isTurning = false;
        }

        private void HandleAttackEnded()
        {
            if (!_attackStarted)
                return;
            
            AnimatorStateInfo nextAnimationState = _simpleAnimationController.GetNextAnimatorStateInfo("BattleAttack");
            AnimatorStateInfo currentAnimationState = _simpleAnimationController.GetCurrentAnimatorStateInfo("BattleAttack");
            if (nextAnimationState.IsTag("Attack") || currentAnimationState.IsTag("Attack"))
                return;

            _endAttackLayerCoroutine = StartCoroutine(SmoothLayerToZero("BattleAttack", 0.1f, ()=> _simpleAnimationController.SetTrigger("ResetAnimationLayer")));
            _attackStarted = false;
            _speed = _cachedSpeed;
        }

        private void CombatController()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _speed = 8;
                if (_endAttackLayerCoroutine != null) StopCoroutine(_endAttackLayerCoroutine);
                _simpleAnimationController.ResetTrigger("ResetAnimationLayer");
                _simpleAnimationController.SetLayerWeight("BattleAttack", 1);
                _simpleAnimationController.SetTrigger("SimpleSwordAttack");
                StartCoroutine(WaitOneFrameToStartCheckAttacksEnd());
            }
        }

        private IEnumerator WaitOneFrameToStartCheckAttacksEnd() {
            yield return null;
            _attackStarted = true;
        }

        private void ProcessGravityMultiplier(float distanceToGround)
        {
            if (!_attackStarted)
            {
                float clampedDistance = Mathf.Clamp01(distanceToGround);
                if (clampedDistance < 0.1)
                {
                    clampedDistance = 0;
                }

                _simpleAnimationController.SetFloat("IsGrounded", clampedDistance);
            }

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
            _currentSmoothedDirection = Vector3.Lerp(_currentSmoothedDirection, _inputDirection.normalized, Time.deltaTime * _velocitySmoothSpeed);
            if (_currentSmoothedDirection.magnitude <= 0.01) _currentSmoothedDirection = Vector3.zero;
            
            _movable.SetDirection(_inputDirection.normalized == Vector3.zero
                ? _currentSmoothedDirection
                : _inputDirection.normalized);

            _movable.SetSpeed(_speed);
            _movable.SetJumpDuration(_jumpDuration);
            _movable.SetJumpMultiplier(_jumpMultiplier);
            _movable.SetRotationSpeed(_rotationSpeed);
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
