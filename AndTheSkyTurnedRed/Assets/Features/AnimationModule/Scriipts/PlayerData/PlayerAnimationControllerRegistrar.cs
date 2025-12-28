using UnityEngine;
using Zenject;

namespace Features.AnimationModule.Scriipts.PlayerData
{
    public class PlayerAnimationControllerRegistrar : MonoBehaviour
    {
        [SerializeField] private SimpleAnimationControllerBase _playerAnimationController;
        [SerializeField] private SimpleAttackAnimationFunctionReactor _simpleAttackAnimationFunctionReactor;
        private PlayerAnimationControllerModel _playerAnimationControllerModel;

        [Inject]
        private void InjectDependencies(PlayerAnimationControllerModel playerAnimationControllerModel) => _playerAnimationControllerModel = playerAnimationControllerModel;

        private void OnEnable()
        {
            _playerAnimationControllerModel.PlayerAnimationController = _playerAnimationController;
            _playerAnimationControllerModel.SimplePlayerAttackAnimationFunctionReactor = _simpleAttackAnimationFunctionReactor;
        }
    }
}