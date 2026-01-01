using System.Collections.Generic;
using UnityEngine;

namespace Features.EntityStatsModule.Scripts.StatsEntity.Configurations {
    public class AccumulativeStatsConfigurationBase<TStatEnum> : ScriptableObject {
        public List<TStatEnum> AccumulativeStats;
    }
}