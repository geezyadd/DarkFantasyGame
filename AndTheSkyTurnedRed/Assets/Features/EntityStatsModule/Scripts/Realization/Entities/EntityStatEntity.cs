using Features.EntityStatsModule.Scripts.StatsEntity;
using Features.EntityStatsModule.Scripts.StatsEntity.Factories;

namespace Features.EntityStatsModule.Scripts.Realization.Entities {
    public class EntityStatEntity : StatEntityBase<EntityStatType> {
        public EntityStatEntity(IStatFactory<EntityStatType> statFactory) : base(statFactory) { }
    }
}