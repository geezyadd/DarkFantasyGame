using System;
using Zenject;

namespace Features.PlayerStatsModule.Scripts
{
    public class PlayerStatsDefaultValuesInitializationSystem : IInitializable, IDisposable
    {
        private readonly PlayerStatsModel _playerStatsModel;
        private readonly DefaultPlayerStatsValuesConfiguration _defaultPlayerStatsValuesConfiguration;

        public PlayerStatsDefaultValuesInitializationSystem(PlayerStatsModel playerStatsModel, DefaultPlayerStatsValuesConfiguration defaultPlayerStatsValuesConfiguration)
        {
            _playerStatsModel = playerStatsModel;
            _defaultPlayerStatsValuesConfiguration = defaultPlayerStatsValuesConfiguration;
        }
        public void Initialize()
        {
            _playerStatsModel.OnPlayerStatEntityChanged += InitializeStats;
        }

        public void Dispose()
        {
            _playerStatsModel.OnPlayerStatEntityChanged -= InitializeStats;
        }

        private void InitializeStats()
        {
            for (int i = 0; i < _defaultPlayerStatsValuesConfiguration.DefaultValues.Count; i++) 
                _playerStatsModel.PlayerStatsEntity.GetStat(_defaultPlayerStatsValuesConfiguration.DefaultValues[i].EntityStatType).OverrideValue(_defaultPlayerStatsValuesConfiguration.DefaultValues[i].Value);
        }
    }
}