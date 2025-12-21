using UnityEngine;

namespace Features.PlayerSpawnModule.Scripts
{
    public interface IPlayerSpawnService
    {
        public GameObject SpawnPlayer(Vector3 position);
    }
}