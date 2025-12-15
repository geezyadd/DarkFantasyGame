namespace Features.AudioModule.Scripts {
    public interface IAudioVolumeService {
        public void SetMasterBusValue(float value);
        public void SetMusicValue(float value);
        public void SetEffectValue(float value);
    }
}