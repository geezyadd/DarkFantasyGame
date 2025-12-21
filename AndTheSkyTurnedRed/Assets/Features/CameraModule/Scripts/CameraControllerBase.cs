using Unity.Cinemachine;
using UnityEngine;

namespace Features.CameraModule.Scripts
{
    public abstract class CameraControllerBase : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _camera;

        public void SetFollowTarget(Transform target) => _camera.Target.TrackingTarget = target;

        public void SetLookAtTarget(Transform target) => _camera.Target.LookAtTarget = target;
    }
}