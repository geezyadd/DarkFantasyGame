using Zenject;

namespace Features.TargetSearchModule.Scripts
{
    public class TargetSearchModuleInstaller : Installer<TargetSearchModuleInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ITargetSearchService>().To<TargetSearchService>().AsSingle();
        }
    }
}