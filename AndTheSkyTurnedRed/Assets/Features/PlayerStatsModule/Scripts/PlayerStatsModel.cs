using System;
using Features.EntityStatsModule.Scripts.Realization;
using Features.EntityStatsModule.Scripts.StatsEntity;

namespace Features.PlayerStatsModule.Scripts
{
    public class PlayerStatsModel
    {
        private IStatEntity<EntityStatType> _playerStatEntity;
        public event Action OnPlayerStatEntityChanged;

        public IStatEntity<EntityStatType> PlayerStatsEntity
        {
            get =>
                _playerStatEntity;
            internal set
            {
                _playerStatEntity = value;
                OnPlayerStatEntityChanged?.Invoke();
            }
        }
    }
}