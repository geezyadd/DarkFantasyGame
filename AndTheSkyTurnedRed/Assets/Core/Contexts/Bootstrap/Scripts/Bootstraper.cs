using Features.GameFlowStateMachine.Scripts;
using Features.SceneLoaderModule.Runtime.Scripts;
using SceneLoaderModule;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Contexts.Bootstrap.Scripts
{
    public class Bootstraper : MonoBehaviour
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
            _gameFlowStateMachine.EnterState(GameFlowState.GlobalSceneState);
            _sceneSwitchService.LoadScene(nameof(Scenes.GlobalScene), LoadSceneMode.Single);
        }
    }
}
