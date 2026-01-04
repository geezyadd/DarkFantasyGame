using Features.AnimationModule.Scriipts.PlayerData;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class LeanController : MonoBehaviour
    {
        [SerializeField] private float _leanSpeed = 5f;
        private PlayerAnimationControllerModel _playerAnimationControllerModel;
        private Vector3 _previousForward;
        private float _leanValue;
        private PlayerControlDataModel _playerControlDataModel;

        [Inject]
        private void InjectDependencies(PlayerAnimationControllerModel playerAnimationControllerModel, PlayerControlDataModel playerControlDataModel) {
            _playerAnimationControllerModel = playerAnimationControllerModel;
            _playerControlDataModel = playerControlDataModel;
        }

        private void Update()
        {
            CalculateLean(!_playerControlDataModel.IsTurning);
            _playerAnimationControllerModel.PlayerAnimationController.SetFloat("Lean", _leanValue);
            _previousForward = transform.forward;
        }
        
        private void CalculateLean(bool leansActivated)
        {
            if (!leansActivated)
            {
                _leanValue = Mathf.Lerp(_leanValue, 0f, _leanSpeed * Time.deltaTime); 
                return;
            }

            Vector3 currentForward = _playerControlDataModel.CurrentSmoothedDirection;
            float rotationRate = _previousForward != currentForward
                ? Vector3.SignedAngle(currentForward, _previousForward, Vector3.up) / Time.deltaTime * -1f
                : 0f;
            _previousForward = currentForward;

            float maxLeanRate = 275f;
            float change = Mathf.Clamp(rotationRate / maxLeanRate, -1f, 1f);

            _leanValue = Mathf.Lerp(_leanValue, change, _leanSpeed * Time.deltaTime);
        }
    }
}