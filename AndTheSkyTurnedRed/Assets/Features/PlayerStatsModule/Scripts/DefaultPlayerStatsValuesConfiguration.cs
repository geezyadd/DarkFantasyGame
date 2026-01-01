using System.Collections.Generic;
using UnityEngine;

namespace Features.PlayerStatsModule.Scripts
{
    [CreateAssetMenu(fileName = nameof(DefaultPlayerStatsValuesConfiguration) + "_Default",
        menuName = "Configurations/Stats/Player/" + nameof(DefaultPlayerStatsValuesConfiguration))]
    public class DefaultPlayerStatsValuesConfiguration : ScriptableObject
    {
        [field: SerializeField] public List<PlayerStatDefaultValueHolder> DefaultValues { get; private set; }
    }
}
