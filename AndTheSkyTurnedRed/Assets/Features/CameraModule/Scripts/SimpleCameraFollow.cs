using UnityEngine;

namespace Features.CameraModule.Scripts
{
    public class SimpleCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;

        [Header("Follow Settings")]
        [SerializeField] private float _followSpeed = 5f;
        [SerializeField] private Vector3 _positionOffset = new Vector3(0, 5, -10);
        [SerializeField] private Vector3 _rotationOffset = new Vector3(10, 0, 0); // В градусах

        [Header("Rotation Settings")]
        [SerializeField] private float _rotationSpeed = 5f;

        private void FixedUpdate()
        {
            if (_target == null)
                return;

            Vector3 targetPosition = _target.position + _positionOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(_target.position - transform.position);
            targetRotation *= Quaternion.Euler(_rotationOffset); 
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }
    }
}