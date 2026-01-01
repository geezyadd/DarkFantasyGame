using Features.EntityStatsModule.Scripts.StatsEntity.Configurations;
using UnityEngine;

namespace Features.EntityStatsModule.Scripts.Realization.Configurations {
    [CreateAssetMenu(fileName = nameof(EntityAccumulativeStatsConfiguration) + "_Default", menuName = "Configurations/StatsModule/" + nameof(EntityAccumulativeStatsConfiguration))]
    public class EntityAccumulativeStatsConfiguration : AccumulativeStatsConfigurationBase<EntityStatType> { }
}