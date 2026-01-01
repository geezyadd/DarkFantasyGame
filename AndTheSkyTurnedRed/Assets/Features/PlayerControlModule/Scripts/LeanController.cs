using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class LeanController : MonoBehaviour {
        private PlayerAnimationControllerModel _playerAnimationControllerModel;
        private IAnimationLayersService _animationLayersService;
        private PlayerControlDataModel _playerControlDataModel;
        private Vector3 _previousForward;
        private float _leanValue;

        [Inject]
        private void InjectDependencies(PlayerAnimationControllerModel playerAnimationControllerModel,
            IAnimationLayersService animationLayersService, PlayerControlDataModel playerControlDataModel) {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _animationLayersService = animationLayersService;
            _playerControlDataModel = playerControlDataModel;
        }

        private void Update()
        {
            CalculateLean(true);
            _playerAnimationControllerModel.PlayerAnimationController.SetFloat("Lean", _leanValue);
            _previousForward = transform.forward;
        }
        
        private void CalculateLean(bool leansActivated)
        {
            if (!leansActivated)
            {
                _leanValue = Mathf.Lerp(_leanValue, 0f, 5f * Time.deltaTime); 
                return;
            }

            // 1. Скорость поворота
            Vector3 currentForward = transform.forward;
            float rotationRate = _previousForward != currentForward
                ? Vector3.SignedAngle(currentForward, _previousForward, Vector3.up) / Time.deltaTime * -1f
                : 0f;
            _previousForward = currentForward;

            // 2. Ограничиваем скорость наклона
            float maxLeanRate = 275f;
            float change = Mathf.Clamp(rotationRate / maxLeanRate, -1f, 1f);

            _leanValue = Mathf.Lerp(_leanValue, change, 5f * Time.deltaTime);
        }
    }
}