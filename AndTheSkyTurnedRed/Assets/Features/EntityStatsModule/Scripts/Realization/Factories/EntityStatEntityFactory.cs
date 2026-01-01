using Features.EntityStatsModule.Scripts.StatsEntity.Factories;

namespace Features.EntityStatsModule.Scripts.Realization.Factories {
    public class EntityStatEntityFactory : StatEntityFactoryBase<EntityStatType> {
        public EntityStatEntityFactory(IStatFactory<EntityStatType> statFactory) : base(statFactory) { }
    }
}