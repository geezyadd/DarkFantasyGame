using Zenject;

namespace Features.AudioModule.Scripts {
    public class AudioServiceInstaller : Installer<AudioServiceInstaller> {
        public override void InstallBindings()
        {
            BindAudioServices();
        }

        private void BindAudioServices()
        {
            Container.BindInterfacesAndSelfTo<FmodAudioService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<AudioVolumeService>()
                .AsSingle();
        }
    }
}
