using System;

namespace Features.EntityStatsModule.Scripts.StatsEntity.Factories {
    public interface IStatEntityFactory<TStatEnum> where TStatEnum : Enum{
        StatEntityBase<TStatEnum> Create();
    }
}