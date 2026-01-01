using Features.EntityStatsModule.Scripts.Realization;
using Features.EntityStatsModule.Scripts.StatsEntity;
using UnityEngine;
using Zenject;

namespace Features.PlayerStatsModule.Scripts
{
    public class PlayerStatEntityRegistrar : MonoBehaviour
    {
        [SerializeField] private StatEntityMonoBase<EntityStatType> _statEntityMonoBase;
        private PlayerStatsModel _playerStatsModel;

        [Inject]
        private void InjectDependencies(PlayerStatsModel playerStatsModel)
        {
            _playerStatsModel = playerStatsModel;
        }

        private void OnEnable()
        {
            _playerStatsModel.PlayerStatsEntity = _statEntityMonoBase;
        }
    }
}