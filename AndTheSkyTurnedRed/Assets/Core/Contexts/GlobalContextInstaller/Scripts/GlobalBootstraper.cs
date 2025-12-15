using Features.GameFlowStateMachine.Scripts;
using Features.SceneLoaderModule.Runtime.Scripts;
using SceneLoaderModule;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Contexts.GlobalContextInstaller.Scripts
{
    public class GlobalBootstraper : MonoBehaviour
    {
        private GameFlowStateMachine _gameFlowStateMachine;
        private ISceneSwitchService _sceneSwitchService;

        [Inject]
        private void InjectDependencies(GameFlowStateMachine gameFlowStateMachine, ISceneSwitchService sceneSwitchService)
        {
            _gameFlowStateMachine = gameFlowStateMachine;
            _sceneSwitchService = sceneSwitchService;
        }

        private void Start()
        {
            _gameFlowStateMachine.EnterState(GameFlowState.SurfaceGameState);
            _sceneSwitchService.LoadScene(nameof(Scenes.PrototypeScene), LoadSceneMode.Additive);
        }
    }
}
