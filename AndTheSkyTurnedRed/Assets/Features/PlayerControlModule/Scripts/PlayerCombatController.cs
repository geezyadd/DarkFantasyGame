using System.Collections;
using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using Features.CameraModule.Scripts;
using Features.MovableModule.Scripts.PlayerData;
using Features.WeaponModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class PlayerCombatController : MonoBehaviour
    {
        [Header("Combat")] 
        [SerializeField] private WeaponBase _weapon;
        [SerializeField] private float _attackMovementSpeedDecreaseValue;
        private Coroutine _endAttackLayerCoroutine;
        private bool _attackStarted;
        private PlayerAnimationControllerModel _playerAnimationControllerModel;
        private IAnimationLayersService _animationLayersService;
        private PlayerControlDataModel _playerControlDataModel;

        [Inject]
        private void InjectDependencies(CameraModel cameraModel, PlayerAnimationControllerModel playerAnimationControllerModel, 
            PlayerMovableModel playerMovableModel, IAnimationLayersService animationLayersService, PlayerControlDataModel playerControlDataModel) {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _animationLayersService = animationLayersService;
            _playerControlDataModel = playerControlDataModel;
        }
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //_cachedSpeed = _speed;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnAttack += HandleAttack;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor.OnEndAttack += HandleAttackEnd;
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
            
            //if (_attackStarted)
            //{
            //    _speed -= _attackMovementSpeedDecreaseValue;
            //    if (_speed < 0.01)
            //        _speed = 0.01f;
            //}
        }
        
        private void CombatController()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //_speed = 8;
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
            _playerControlDataModel.IsAttacking = false;
            //_speed = _cachedSpeed;
        }

        private IEnumerator WaitOneFrameToStartCheckAttacksEnd() {
            yield return null;
            _attackStarted = true;
            _playerControlDataModel.IsAttacking = true;
        }
    }
}