using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Features.AudioModule.Scripts {
    public interface ISoundEventInstanceEntity {
        public EventReference AudioID { get; }
        public void PlaySound();
        public void PlaySound(Transform transform);
        public void PlaySound(Vector3 position);
        public bool IsValid();
        public void StopSound(STOP_MODE stopMode);
        public void SetPaused(bool isPaused);
        public void RestartSound();
        public void StopSoundWithFade(float fadeDuration);
        public void StopSnapshotSoundWithFade(float fadeDuration);
        public void ReleaseEventInstance();
        public bool IsPlaying();
        public void SetParameterValue(string parameterName, float value);
        public void SetTimelinePosition(int time);
        public int GetTimelinePosition();
    }
}