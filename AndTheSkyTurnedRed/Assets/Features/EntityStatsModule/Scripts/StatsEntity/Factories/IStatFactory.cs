using System;

namespace Features.EntityStatsModule.Scripts.StatsEntity.Factories {
    public interface IStatFactory<in TStatEnum>  where TStatEnum : Enum{
        IStat Create(TStatEnum entityStatType);
    }
}