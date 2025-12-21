using UnityEngine;

namespace Features.PlayerSpawnModule.Scripts
{
    [CreateAssetMenu(fileName = nameof(PlayerSpawnConfiguration) + "_Default",
        menuName = "Configurations/PlayerSpawn/" + nameof(PlayerSpawnConfiguration))]
    public class PlayerSpawnConfiguration : ScriptableObject
    {
        [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
    }
}
