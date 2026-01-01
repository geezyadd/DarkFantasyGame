using System;
using Features.EntityStatsModule.Scripts.Realization;

namespace Features.PlayerStatsModule.Scripts
{
    [Serializable]
    public class PlayerStatDefaultValueHolder
    {
        public EntityStatType EntityStatType;
        public float Value;
    }
}