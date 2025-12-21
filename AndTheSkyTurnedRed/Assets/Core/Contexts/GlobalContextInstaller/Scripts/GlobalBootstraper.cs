using System.Collections.Generic;
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
            IReadOnlyList<string> scenes = new List<string>
            {
                nameof(Scenes.GameScene),
                nameof(Scenes.PrototypeScene)
                
            };
            _sceneSwitchService.UpdateLoadedScenes(scenes, nameof(Scenes.PrototypeScene));
        }
    }
}
