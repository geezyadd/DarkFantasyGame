using Core.Data.Scripts;
using Zenject;

namespace Core.Contexts.GlobalContextInstaller.Scripts
{
    public class GlobalContextInstaller : MonoInstaller<GlobalContextInstaller>
    {
        public override void InstallBindings()
        {
            DataInstaller.Install(Container);
        }
    }
}
