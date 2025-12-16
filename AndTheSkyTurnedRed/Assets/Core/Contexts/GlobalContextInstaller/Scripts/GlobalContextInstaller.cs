using Core.Data.Scripts;
using Features.Input.Scripts.Installers;
using Zenject;

namespace Core.Contexts.GlobalContextInstaller.Scripts
{
    public class GlobalContextInstaller : MonoInstaller<GlobalContextInstaller>
    {
        public override void InstallBindings() {
            DataInstaller.Install(Container);
        }
    }
}
