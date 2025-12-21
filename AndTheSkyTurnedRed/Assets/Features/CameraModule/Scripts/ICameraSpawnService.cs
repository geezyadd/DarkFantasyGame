using UnityEngine;

namespace Features.CameraModule.Scripts
{
    public interface ICameraSpawnService
    {
        public CameraControllerBase SpawnCamera(CameraType cameraType, Vector3 position = default);
    }
}