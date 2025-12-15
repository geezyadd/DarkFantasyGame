using FMOD.Studio;
using FMODUnity;

namespace Features.AudioModule.Scripts {
    public class AudioVolumeService : IAudioVolumeService {
        private const string ALL_BUS_NAME = "bus:/";
        private const string MUSIC_BUS_NAME = "bus:/Music";
        private const string SFX_BUS_NAME = "bus:/SFX";
        private const string REVERB_BUS_NAME = "bus:/Reverb";
        private Bus _masterBus;
        private Bus _effectsBus;
        private Bus _musicBus;
        private Bus _reverbBus;

        public void SetMasterBusValue(float value) {
            if(!_masterBus.isValid())
                _masterBus = RuntimeManager.GetBus(ALL_BUS_NAME);

            if (value < 0)
                value = 0;

            if (value > 1)
                value = 1;
                
            _masterBus.setVolume(value);
        }

        public void SetMusicValue(float value) {
            if(!_musicBus.isValid())
                _musicBus = RuntimeManager.GetBus(MUSIC_BUS_NAME);

            if (value < 0)
                value = 0;

            if (value > 1)
                value = 1;
                
            _musicBus.setVolume(value);
        }

        public void SetEffectValue(float value) {
            if(!_effectsBus.isValid())
                _effectsBus = RuntimeManager.GetBus(SFX_BUS_NAME);
            
            if(!_reverbBus.isValid())
                _reverbBus = RuntimeManager.GetBus(REVERB_BUS_NAME);

            if (value < 0)
                value = 0;

            if (value > 1)
                value = 1;
                
            _effectsBus.setVolume(value);
            _reverbBus.setVolume(value);
        }
    }
}