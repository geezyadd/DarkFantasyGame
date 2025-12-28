using UnityEngine;
using Zenject;

namespace Features.MovableModule.Scripts.PlayerData
{
    public class PlayerMovableRegistrar : MonoBehaviour
    {
        [SerializeField] private MovableBase _playerMovable;
        private PlayerMovableModel _playerMovableModel;

        [Inject]
        private void InjectDependencies(PlayerMovableModel playerMovableModel) => _playerMovableModel = playerMovableModel;

        private void OnEnable()
        {
            _playerMovableModel.PlayerMovable = _playerMovable;
        }
    }
}