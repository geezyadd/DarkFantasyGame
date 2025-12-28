using System;
using System.Collections;
using Features.AnimationModule.Scriipts;
using Features.AnimationModule.Scriipts.PlayerData;
using Features.CameraModule.Scripts;
using Features.MovableModule.Scripts.PlayerData;
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
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpMultiplier = 2f;
        [SerializeField] private float _gravityMultiplierDistance = 2;
        [SerializeField] private float _maxGravityMultiplierValue = 15;
        [SerializeField] private float _distanceToEndJump;
        [SerializeField] private float _velocitySmoothSpeed;
        private Vector3 _inputDirection;
        private float _gravityMultiplier;
        private Vector3 _currentSmoothedDirection;
        private CameraModel _cameraModel;
        private PlayerMovableModel _playerMovableModel;
        private PlayerControlDataModel _playerControlDataModel;

        [Inject]
        private void InjectDependencies(CameraModel cameraModel, PlayerMovableModel playerMovableModel, PlayerControlDataModel playerControlDataModel)
        {
            _cameraModel = cameraModel;
            _playerMovableModel = playerMovableModel;
            _playerControlDataModel = playerControlDataModel;
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
            _playerControlDataModel.DistanceToGround = distanceToGround;
            if (Input.GetKeyDown(KeyCode.Space) && !_playerControlDataModel.IsAttacking) {
                _playerMovableModel.PlayerMovable.Jump();
            }

            ProcessGravityMultiplier(distanceToGround);
        }

        private void ProcessGravityMultiplier(float distanceToGround)
        {
            if (distanceToGround >= _gravityMultiplierDistance)
            {
                if(_gravityMultiplier >= _maxGravityMultiplierValue)
                    _gravityMultiplier--;
                _playerMovableModel.PlayerMovable.SetGravityMultiplier(_gravityMultiplier);
            }
            else
            {
                _gravityMultiplier = 0;
                _playerMovableModel.PlayerMovable.SetGravityMultiplier(_gravityMultiplier);
            }
        }

        private void FixedUpdate()
        {
            _currentSmoothedDirection = Vector3.Lerp(_currentSmoothedDirection, _inputDirection.normalized, Time.deltaTime * _velocitySmoothSpeed);
            if (_currentSmoothedDirection.magnitude <= 0.01) _currentSmoothedDirection = Vector3.zero;
            
            _playerMovableModel.PlayerMovable.SetDirection(_inputDirection.normalized == Vector3.zero
                ? _currentSmoothedDirection
                : _inputDirection.normalized);

            _playerMovableModel.PlayerMovable.SetSpeed(_speed);
            _playerMovableModel.PlayerMovable.SetJumpDuration(_jumpDuration);
            _playerMovableModel.PlayerMovable.SetJumpMultiplier(_jumpMultiplier);
            _playerMovableModel.PlayerMovable.SetRotationSpeed(_rotationSpeed);
            _playerMovableModel.PlayerMovable.UpdateAngle();
            _playerMovableModel.PlayerMovable.ProcessDirectionRotation();
            _playerMovableModel.PlayerMovable.ProcessHorizontalMovement();
            _playerMovableModel.PlayerMovable.ProcessVerticalMovement();
        }

        private float GetDistanceToGround()
        {
            Ray ray = new Ray(transform.position + Vector3.up * RAYCAST_OFFSET, Vector3.down);
            if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, RAYCAST_DISTANCE, _groundLayerMask))
                return Vector3.Distance(hit.point, transform.position);

            return RAYCAST_DISTANCE;
        }
    }
}
