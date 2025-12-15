using Features.GameFlowStateMachine.Scripts;
using Features.SceneLoaderModule.Runtime.Scripts.Installers;
using Zenject;

namespace Core.Contexts.ProjectContext.Scripts
{
    public class ProjectContextInstaller : MonoInstaller<ProjectContextInstaller>
    {
        public override void InstallBindings()
        {
            SceneLoaderInstaller.Install(Container);
            GameFlowStateMachineInstaller.Install(Container);
        }
    }
}
