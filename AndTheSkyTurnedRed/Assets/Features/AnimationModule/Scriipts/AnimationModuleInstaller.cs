using Zenject;

namespace Features.AnimationModule.Scriipts
{
    public class AnimationModuleInstaller : Installer<AnimationModuleInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AnimationLayersService>().AsSingle();
        }
    }
}