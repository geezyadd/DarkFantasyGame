using Core.InputModule.Scripts.Generated;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Features.CameraModule.Scripts {
    public class SimpleGameCameraController : CameraControllerBase
    {
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;

        [Header("Settings")]
        [SerializeField] private float _horizontalSensitivity = 180f;
        [SerializeField] private float _verticalSensitivity = 120f;
        [SerializeField] private float _minVerticalAngle = -30f;
        [SerializeField] private float _maxVerticalAngle = 60f;

        private IInputService _inputService;

        private float _horizontalAngle;
        private float _verticalAngle;

        [Inject]

        private void InjectDependencies(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void FixedUpdate()
        {
            Vector2 lookDelta = _inputService.CameraDeltaVector2ReadValue();
            UpdateRotation(lookDelta);
        }

        private void UpdateRotation(Vector2 delta)
        {
            _horizontalAngle += delta.x * _horizontalSensitivity * Time.deltaTime;
            _verticalAngle -= delta.y * _verticalSensitivity * Time.deltaTime;

            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);

            _orbitalFollow.HorizontalAxis.Value = _horizontalAngle;
            _orbitalFollow.VerticalAxis.Value = _verticalAngle;
        }
    }
}