using System;
using System.Collections;
using System.Collections.Generic;
using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using Features.EntityStatsModule.Scripts.Modifier;
using Features.EntityStatsModule.Scripts.Realization;
using Features.EntityStatsModule.Scripts.StatsEntity;
using Features.MovableModule.Scripts.PlayerData;
using Features.PlayerStatsModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private float _attackMovementSpeedDecreaseValue;
        [SerializeField] private float _distanceToEndJump;
        [SerializeField] private float _animationSmoothSpeed;
        [SerializeField] private float _stopRunAnimationVelocityBorder;
        [SerializeField] private float _startRunAnimationVelocityBorder;
        [SerializeField] private float _speedDecreaseValue;
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
        private PlayerStatsModel _playerStatsModel;
        private List<StatModifier> _speedDecreaseModifiers = new();
        private IStat _speedStat;
        private bool _stopDecreaseSpeedOnTurn;

        [Inject]
        private void InjectDependencies(PlayerAnimationControllerModel playerAnimationControllerModel, PlayerMovableModel playerMovableModel,
            IAnimationLayersService animationLayersService, PlayerControlDataModel playerControlDataModel, PlayerStatsModel playerStatsModel)
        {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _playerMovableModel = playerMovableModel;
            _animationLayersService = animationLayersService;
            _playerControlDataModel = playerControlDataModel;
            _playerStatsModel = playerStatsModel;
        }
        
        private void Start()
        {
            _speedStat = _playerStatsModel.PlayerStatsEntity.GetStat(EntityStatType.Speed);
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnEndTurn += ClearSpeedDecreaseModifiers;
        }

        private void OnDestroy()
        {
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnEndTurn += ClearSpeedDecreaseModifiers;
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
            
            //if(!_isTurning || _stopDecreaseSpeedOnTurn)
            //    return;
            //
            //if (_speedStat.FullValue < 0.1f) 
            //    return;
            //
            //StatModifier decreaseModifier = new(-_attackMovementSpeedDecreaseValue, ModifierType.Flat);
            //_speedDecreaseModifiers.Add(decreaseModifier);
            //_speedStat.AddStatModifier(decreaseModifier);
        }
        
        private void ClearSpeedDecreaseModifiers()
        {
            _stopDecreaseSpeedOnTurn = true;
            //StartCoroutine(SmoothIncreaseSpeedAfterTurn());
            for (int i = 0; i < _speedDecreaseModifiers.Count; i++)
                _speedStat.RemoveStatModifierThatEqual(_speedDecreaseModifiers[i]);
            _speedDecreaseModifiers.Clear();
        }

        private IEnumerator SmoothIncreaseSpeedAfterTurn()
        {
            for (int i = 0; i < _speedDecreaseModifiers.Count; i++)
            {
                _speedStat.RemoveStatModifierThatEqual(_speedDecreaseModifiers[i]);
                yield return null;
            }
            
            _speedDecreaseModifiers.Clear();
        }

        private void HandleTurns()
        {
            if (Mathf.Abs(_playerMovableModel.PlayerMovable.AngleToTarget) > 100 && !_isTurning) {
                //if(_endTurnsLayerCoroutine != null) _animationLayersService.StopSmooth(_endTurnsLayerCoroutine);
                //_playerAnimationControllerModel.PlayerAnimationController.SetLayerWeight("BattleTurns", 1);
                if (_playerMovableModel.PlayerMovable.AngleToTarget > 0)
                    _playerAnimationControllerModel.PlayerAnimationController.SetFloat("AngleToTarget", 1);
                else
                    _playerAnimationControllerModel.PlayerAnimationController.SetFloat("AngleToTarget", -1);
                //_speedStat.AddStatModifier(_speedDecreaseModifier);
                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("Turn");
                _stopDecreaseSpeedOnTurn = false;
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
            if(_startStopRunTriggered || _playerControlDataModel.IsAttacking || _isTurning)
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
            Debug.LogError("isTurning true");
            StatModifier decreaseModifier = new(-_attackMovementSpeedDecreaseValue, ModifierType.Flat);
            _speedDecreaseModifiers.Add(decreaseModifier);
            _speedStat.AddStatModifier(decreaseModifier);
            _isTurning = true;
        }


        private void HandleEndTurn() {
            if(!_isTurning)
                return;
            
            AnimatorStateInfo nextAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetNextAnimatorStateInfo("BattleLocomotion");
            AnimatorStateInfo currentAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetCurrentAnimatorStateInfo("BattleLocomotion");
            if ((nextAnimationState.IsTag("Turns") || currentAnimationState.IsTag("Turns")) && !_playerMovableModel.PlayerMovable.IsJumping)
                return;
            //ClearSpeedDecreaseModifiers();
            _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("Turn");
            //_speedStat.RemoveStatModifierThatEqual(_speedDecreaseModifier);
            //_endTurnsLayerCoroutine = _animationLayersService.SmoothLayerToZero(_playerAnimationControllerModel.PlayerAnimationController,"BattleTurns",0.5f);
            _isTurning = false;
            Debug.LogError("isTurning false");
        }
    }
}