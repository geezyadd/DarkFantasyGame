using Features.InjectedPrefabFactory.Scripts;
using Features.SceneLoaderModule.Runtime.Scripts;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Features.PlayerSpawnModule.Scripts
{
    public class PlayerSpawnService : IPlayerSpawnService
    {
        private readonly PlayerSpawnConfiguration _playerSpawnConfiguration;
        private readonly IInjectedPrefabFactory _injectedPrefabFactory;

        public PlayerSpawnService(PlayerSpawnConfiguration playerSpawnConfiguration, IInjectedPrefabFactory injectedPrefabFactory)
        {
            _playerSpawnConfiguration = playerSpawnConfiguration;
            _injectedPrefabFactory = injectedPrefabFactory;
        }
        public GameObject SpawnPlayer(Vector3 position) {
            return _injectedPrefabFactory.CreatePrefab(_playerSpawnConfiguration.PlayerPrefab, position, Quaternion.identity);
        }
    }
}
