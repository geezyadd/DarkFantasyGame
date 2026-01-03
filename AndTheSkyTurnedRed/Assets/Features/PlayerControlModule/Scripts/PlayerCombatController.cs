using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using Features.CameraModule.Scripts;
using Features.DamageableModule.Scripts;
using Features.EntityStatsModule.Scripts.Modifier;
using Features.EntityStatsModule.Scripts.Realization;
using Features.EntityStatsModule.Scripts.StatsEntity;
using Features.MovableModule.Scripts.PlayerData;
using Features.PlayerStatsModule.Scripts;
using Features.TargetSearchModule.Scripts;
using Features.WeaponModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private WeaponBase _weapon;
        [SerializeField] private float _attackMovementSpeedDecreaseValue;
        private Coroutine _endAttackLayerCoroutine;
        private bool _attackStarted;
        private PlayerAnimationControllerModel _playerAnimationControllerModel;
        private IAnimationLayersService _animationLayersService;
        private PlayerControlDataModel _playerControlDataModel;
        private PlayerStatsModel _playerStatsModel;
        private IStat _speedStat;
        private List<StatModifier> _speedDecreaseModifiers = new();
        private ITargetSearchService _targetSearchService;
        private IDamageable _currentTarget;
        private PlayerMovableModel _playerMovableModel;
        private int _combatCount;

        [Inject]
        private void InjectDependencies(CameraModel cameraModel, PlayerAnimationControllerModel playerAnimationControllerModel, 
            PlayerMovableModel playerMovableModel, IAnimationLayersService animationLayersService, PlayerControlDataModel playerControlDataModel,
            PlayerStatsModel playerStatsModel, ITargetSearchService targetSearchService) {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _animationLayersService = animationLayersService;
            _playerControlDataModel = playerControlDataModel;
            _playerStatsModel = playerStatsModel;
            _targetSearchService = targetSearchService;
            _playerMovableModel = playerMovableModel;
        }
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //_cachedSpeed = _speed;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnAttack += HandleAttack;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnEndAttack += HandleAttackEnd;
            _speedStat = _playerStatsModel.PlayerStatsEntity.GetStat(EntityStatType.Speed);
        }

        private void OnDestroy() {
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnAttack -= HandleAttack;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnEndAttack -= HandleAttackEnd;
        }

        private void HandleAttack()
        {
            _weapon.Attack();
        }

        private void HandleAttackEnd()
        {
            _weapon.StopAttack();
        }

        private void Update()
        {
            CombatController();
            HandleAttackEnded();
            if (!_attackStarted) 
                return;


            if (_currentTarget != null && _currentTarget.IsActive)
            {
                Vector3 direction = _currentTarget.Transform.position - transform.position;
                direction.y = 0;
                if (direction.magnitude > 4)
                {
                    _playerMovableModel.PlayerMovable.SetDirection(direction.normalized);
                    _playerMovableModel.PlayerMovable.ProcessHorizontalMovement();
                }
            }
            else
            {
                _currentTarget = null;
                ChooseTarget();
            }
            

            if (_speedStat.FullValue < 0.1f || _combatCount > 2) 
                return;
            
            StatModifier decreaseModifier = new(-_attackMovementSpeedDecreaseValue, ModifierType.Flat);
            _speedDecreaseModifiers.Add(decreaseModifier);
            _speedStat.AddStatModifier(decreaseModifier);

        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if(_currentTarget != null)
                Gizmos.DrawSphere(_currentTarget.Transform.position + Vector3.up * 2, 0.1f);
        }

        private void ChooseTarget()
        {
            List<IDamageable> targets = _targetSearchService.GetTargetsInRange(transform.position, 10, _enemyLayerMask).ToList();
            if(targets.Count > 0)
                _currentTarget = targets[Random.Range(0, targets.Count)];
        }

        private void CombatController()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _combatCount++;
                ClearSpeedDecreaseModifiers();
                if (_endAttackLayerCoroutine != null) _animationLayersService.StopSmooth(_endAttackLayerCoroutine);
                _playerAnimationControllerModel.PlayerAnimationController.ResetTrigger("ResetAnimationLayer");
                _playerAnimationControllerModel.PlayerAnimationController.SetLayerWeight("BattleAttack", 1);
                _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("SimpleSwordAttack");
                StartCoroutine(WaitOneFrameToStartCheckAttacksEnd());
            }
        }
        
        private void HandleAttackEnded()
        {
            if (!_attackStarted)
                return;
            
            AnimatorStateInfo nextAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetNextAnimatorStateInfo("BattleAttack");
            AnimatorStateInfo currentAnimationState = _playerAnimationControllerModel.PlayerAnimationController.GetCurrentAnimatorStateInfo("BattleAttack");
            if (nextAnimationState.IsTag("Attack") || currentAnimationState.IsTag("Attack"))
                return;

            _endAttackLayerCoroutine = _animationLayersService.SmoothLayerToZero(_playerAnimationControllerModel.PlayerAnimationController,"BattleAttack", 0.1f, ()=> _playerAnimationControllerModel.PlayerAnimationController.SetTrigger("ResetAnimationLayer"));
            _attackStarted = false;
            _combatCount = 0;
            ClearSpeedDecreaseModifiers();
            _playerControlDataModel.IsAttacking = false;
        }

        private void ClearSpeedDecreaseModifiers()
        {
            for (int i = 0; i < _speedDecreaseModifiers.Count; i++)
                _speedStat.RemoveStatModifierThatEqual(_speedDecreaseModifiers[i]);
            _speedDecreaseModifiers.Clear();
        }

        private IEnumerator WaitOneFrameToStartCheckAttacksEnd() {
            yield return null;
            _attackStarted = true;
            _playerControlDataModel.IsAttacking = true;
        }
    }
}