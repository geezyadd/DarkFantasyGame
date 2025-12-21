using Features.CameraModule.Scripts;
using UnityEngine;
using Zenject;
using CameraType = Features.CameraModule.Scripts.CameraType;

namespace Features.PlayerSpawnModule.Scripts
{
    public class PlayerSpawnSystem : IInitializable
    {
        private readonly IPlayerSpawnService _playerSpawnService;
        private readonly ICameraSpawnService _cameraSpawnService;

        public PlayerSpawnSystem(IPlayerSpawnService playerSpawnService, ICameraSpawnService cameraSpawnService)
        {
            _playerSpawnService = playerSpawnService;
            _cameraSpawnService = cameraSpawnService;
        }
        public void Initialize()
        {
            CameraControllerBase cameraController = _cameraSpawnService.SpawnCamera(CameraType.SimpleGameCamera);
            GameObject player = _playerSpawnService.SpawnPlayer(Vector3.zero);
            cameraController.SetFollowTarget(player.transform);
            cameraController.SetLookAtTarget(player.transform);
        }
    }
}