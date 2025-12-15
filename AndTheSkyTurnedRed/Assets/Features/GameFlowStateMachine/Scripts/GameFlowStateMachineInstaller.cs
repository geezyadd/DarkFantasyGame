using Zenject;

namespace Features.GameFlowStateMachine.Scripts
{
    public class GameFlowStateMachineInstaller : Installer<GameFlowStateMachineInstaller> {
        public override void InstallBindings() {
            Container.Bind<GameFlowStateMachine>()
                .AsSingle();
        }
    }
}