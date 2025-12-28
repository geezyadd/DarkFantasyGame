using System.Collections;
using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using Features.MovableModule.Scripts.PlayerData;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private float _distanceToEndJump;
        [SerializeField] private float _animationSmoothSpeed;
        [SerializeField] private float _stopRunAnimationVelocityBorder;
        [SerializeField] private float _startRunAnimationVelocityBorder;
        private PlayerAnimationControllerModel _playerAnimationControllerModel;
        private PlayerMovableModel _playerMovableModel;
        private IAnimationLayersService _animationLayersService;
        private bool _isMoving;
        private float _lastSmoothedVelocity;
        private float _smoothedVelocity;
        private Coroutine _endTurnsLayerCoroutine;
        private bool _isTurning;
        private bool _startStopRunTriggered;
        private bool _isStartStopAnimation;
        private PlayerControlDataModel _playerControlDataModel;

        [Inject]
        private void InjectDependencies(PlayerAnimationControllerModel playerAnimationControllerModel, PlayerMovableModel playerMovableModel,
            IAnimationLayersService animationLayersService, PlayerControlDataModel playerControlDataModel)
        {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _playerMovableModel = playerMovableModel;
            _animationLayersService = animationLayersService;
            _playerControlDataModel = playerControlDataModel;
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space) && !_playerControlDataModel.IsAttacking) {
                _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("Run");
                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("Jump");
            }

            if (_playerControlDataModel.DistanceToGround < _distanceToEndJump && _playerMovableModel.PlayerMovable.IsJumping) {
                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("JumpEnded");
            }

            _isMoving = _playerMovableModel.PlayerMovable.GetVelocity > 0f;

            if (_isMoving && !_playerMovableModel.PlayerMovable.IsJumping && _playerControlDataModel.DistanceToGround < _distanceToEndJump) {
                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("Run");
                _isMoving = true;
            }
            
            HandleTurns();
            HandleEndTurn();
            _lastSmoothedVelocity = _smoothedVelocity;
            _smoothedVelocity = Mathf.Lerp(_smoothedVelocity, _playerMovableModel.PlayerMovable.GetVelocity, Time.deltaTime * _animationSmoothSpeed);
            if (_smoothedVelocity < 0.01)
            {
                _smoothedVelocity = 0;
            }
            
            HandleStopRun();
            //HandleStartRun();
            HandleStopStartRunEnd();
            
            _playerAnimationControllerModel.PlayerAnimationController.SetFloat("Velocity", _smoothedVelocity);
            _playerAnimationControllerModel.PlayerAnimationController.SetFloat("RunAnimationSpeed", _playerMovableModel.PlayerMovable.GetVelocity/_playerMovableModel.PlayerMovable.GetSpeed);
            
            if (!_playerControlDataModel.IsAttacking)
            {
                float clampedDistance = Mathf.Clamp01(_playerControlDataModel.DistanceToGround);
                if (clampedDistance < 0.1)
                {
                    clampedDistance = 0;
                }

                _playerAnimationControllerModel.PlayerAnimationController.SetFloat("IsGrounded", clampedDistance);
            }
        }

        private void HandleTurns()
        {
            if (Mathf.Abs(_playerMovableModel.PlayerMovable.AngleToTarget) > 170 && !_isTurning) {
                if(_endTurnsLayerCoroutine != null) _animationLayersService.StopSmooth(_endTurnsLayerCoroutine);
                _playerAnimationControllerModel.PlayerAnimationController.SetLayerWeight("BattleTurns", 1);
                if (_playerMovableModel.PlayerMovable.AngleToTarget > 0)
                    _playerAnimationControllerModel.PlayerAnimationController.SetFloat("AngleToTarget", 1);
                else
                    _playerAnimationControllerModel.PlayerAnimationController.SetFloat("AngleToTarget", -1);

                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("Turn");
                StartCoroutine(WaitOneFrameToStartCheckTurnsEnd());
            }
        }

        private void HandleStopStartRunEnd()
        {
            if(!_isStartStopAnimation)
                return;
            
            AnimatorStateInfo nextAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetNextAnimatorStateInfo("BattleStartStopRun");
            AnimatorStateInfo currentAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetCurrentAnimatorStateInfo("BattleStartStopRun");
            if ((nextAnimationState.IsTag("StartStopRun") || currentAnimationState.IsTag("StartStopRun")) && !_playerMovableModel.PlayerMovable.IsJumping)
                return;

            _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("StartRun");
            _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("StopRun");
            _animationLayersService.SmoothLayerToZero(_playerAnimationControllerModel.PlayerAnimationController,"BattleStartStopRun",0.3f, () => _startStopRunTriggered = false);
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
            _playerAnimationControllerModel.PlayerAnimationController.SetLayerWeight("BattleStartStopRun", 1);
            _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("StopRun");
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
            _playerAnimationControllerModel.PlayerAnimationController.SetLayerWeight("BattleStartStopRun", 1);
            _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("StartRun");
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
            
            AnimatorStateInfo nextAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetNextAnimatorStateInfo("BattleTurns");
            AnimatorStateInfo currentAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetCurrentAnimatorStateInfo("BattleTurns");
            if ((nextAnimationState.IsTag("Turns") || currentAnimationState.IsTag("Turns")) && !_playerMovableModel.PlayerMovable.IsJumping)
                return;

            _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("Turn");
            _endTurnsLayerCoroutine = _animationLayersService.SmoothLayerToZero(_playerAnimationControllerModel.PlayerAnimationController,"BattleTurns",0.2f);
            _isTurning = false;
        }
    }
}