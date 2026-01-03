using Features.CameraModule.Scripts;
using Features.EntityStatsModule.Scripts.Realization;
using Features.MovableModule.Scripts.PlayerData;
using Features.PlayerStatsModule.Scripts;
using UnityEngine;
using Zenject;

namespace Features.PlayerControlModule.Scripts
{
    public class TestMovementController : MonoBehaviour
    {
        private const float RAYCAST_OFFSET = 3f;
        private const float RAYCAST_DISTANCE = 20f;
        [SerializeField] private LayerMask _groundLayerMask;
        [Header("Movement")]
        //[SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpMultiplier = 2f;
        [SerializeField] private float _velocitySmoothSpeed;
        private Vector3 _inputDirection;
        //private Vector3 _currentSmoothedDirection;
        private CameraModel _cameraModel;
        private PlayerMovableModel _playerMovableModel;
        private PlayerControlDataModel _playerControlDataModel;
        private PlayerStatsModel _playerStatsModel;

        [Inject]
        private void InjectDependencies(CameraModel cameraModel, PlayerMovableModel playerMovableModel, PlayerControlDataModel playerControlDataModel, PlayerStatsModel playerStatsModel)
        {
            _cameraModel = cameraModel;
            _playerMovableModel = playerMovableModel;
            _playerControlDataModel = playerControlDataModel;
            _playerStatsModel = playerStatsModel;
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
            _playerControlDataModel.DistanceToGround = _playerMovableModel.PlayerMovable.DistanceToGround;
            _playerControlDataModel.IsNearToGround = _playerMovableModel.PlayerMovable.IsNearToGround;
            if (Input.GetKeyDown(KeyCode.Space) && !_playerControlDataModel.IsAttacking && _playerMovableModel.PlayerMovable.GroundTouchCount > 0) {
                _playerMovableModel.PlayerMovable.Jump();
            }
        }

        private void FixedUpdate()
        {
            _playerControlDataModel.CurrentSmoothedDirection = Vector3.Lerp(_playerControlDataModel.CurrentSmoothedDirection, _inputDirection.normalized, Time.deltaTime * _velocitySmoothSpeed);
            //_currentSmoothedDirection = Vector3.Lerp(_currentSmoothedDirection, _inputDirection.normalized, Time.deltaTime * _velocitySmoothSpeed);
            if (_playerControlDataModel.CurrentSmoothedDirection.magnitude <= 0.01) _playerControlDataModel.CurrentSmoothedDirection = Vector3.zero;
            
            _playerMovableModel.PlayerMovable.SetDirection(_playerControlDataModel.CurrentSmoothedDirection);
            _playerMovableModel.PlayerMovable.SetSpeed(_playerStatsModel.PlayerStatsEntity.GetStat(EntityStatType.Speed).FullValue);
            _playerMovableModel.PlayerMovable.SetJumpDuration(_jumpDuration);
            _playerMovableModel.PlayerMovable.SetJumpMultiplier(_jumpMultiplier);
            _playerMovableModel.PlayerMovable.SetRotationSpeed(_rotationSpeed);
            _playerMovableModel.PlayerMovable.UpdateAngle();
            _playerMovableModel.PlayerMovable.ProcessDirectionRotation();
            _playerMovableModel.PlayerMovable.ProcessHorizontalMovement();
            _playerMovableModel.PlayerMovable.ProcessVerticalMovement();
        }

        //private float GetDistanceToGround()
        //{
        //    Ray ray = new Ray(transform.position + Vector3.up * RAYCAST_OFFSET, Vector3.down);
        //    if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, RAYCAST_DISTANCE, _groundLayerMask))
        //        return Vector3.Distance(hit.point, transform.position);
//
        //    return RAYCAST_DISTANCE;
        //}
    }
}
