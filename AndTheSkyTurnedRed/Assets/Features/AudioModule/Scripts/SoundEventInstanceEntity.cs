using System.Collections;
using Features.CoroutineRunnerModule.Scripts;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Features.AudioModule.Scripts {
    public class SoundEventInstanceEntity : ISoundEventInstanceEntity {
        private const string INTENSITY_PARAMETER_NAME = "Intensity";
        private readonly EventReference _audioID;
        private readonly ICoroutineRunner _coroutineRunner;
        private EventInstance _eventInstance;
        private Coroutine _fadeCoroutine;
        private Coroutine _positionCoroutine;

        public EventReference AudioID =>
            _audioID;

        public SoundEventInstanceEntity(EventReference audioID, ICoroutineRunner coroutineRunner) {
            _audioID = audioID;
            _eventInstance = RuntimeManager.CreateInstance(audioID);
            _coroutineRunner = coroutineRunner;
        }

        public SoundEventInstanceEntity() {}

        public void PlaySound() {
            ClearFadeCoroutine();
            
            if (IsValid() is false)
                return;

            if (IsPlaying())
                return;
            
            _eventInstance.setVolume(1f);
            _eventInstance.start();
        }

        public void PlaySound(Transform transform) {
            ClearFadeCoroutine();
                                         
            if (IsValid() is false)
                return;

            if (IsPlaying())
                return;
            
            _eventInstance.setVolume(1f);
            _eventInstance.start();
            
            if(_positionCoroutine != null)
                _coroutineRunner.StopCoroutine(_positionCoroutine);
            
            _positionCoroutine = _coroutineRunner.StartCoroutine(Process3DSoundPosition(transform));
        }

        public void PlaySound(Vector3 position) {
            ClearFadeCoroutine();
            
            if (IsValid() is false)
                return;

            if (IsPlaying())
                return;
            
            _eventInstance.setVolume(1f);
            _eventInstance.set3DAttributes(position.To3DAttributes());
            _eventInstance.start();
        }

        public bool IsValid() =>
            _eventInstance.isValid();

        public void StopSound(STOP_MODE stopMode) {
            if (IsValid() is false)
                return;
            
            _eventInstance.stop(stopMode);
        }

        public void SetPaused(bool isPaused) {
            if (IsValid() is false)
                return;

            _eventInstance.setPaused(isPaused);
        }

        public void RestartSound() {
            if (IsValid() is false)
                return;
            
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
            _eventInstance.start();
        }

        public void StopSoundWithFade(float fadeDuration) {
            if (IsValid() is false)
                return;
            
            if(IsPlaying() is false)
                return;
            
            _fadeCoroutine ??= _coroutineRunner.StartCoroutine(FadeOutSound(fadeDuration));
        }

        public void StopSnapshotSoundWithFade(float fadeDuration) {
            if (IsValid() is false)
                return;
            
            if(IsPlaying() is false)
                return;

            _fadeCoroutine ??= _coroutineRunner.StartCoroutine(SnapshotFadeOut(fadeDuration));
        }

        private IEnumerator SnapshotFadeOut(float fadeDuration) {
            _eventInstance.getParameterByName(INTENSITY_PARAMETER_NAME, out float startVolume);
            float elapsedTime = 0f;
            while (elapsedTime <= fadeDuration) {
                float newVolume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                SetParameterValue(INTENSITY_PARAMETER_NAME, newVolume);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null; 
            }
            SetParameterValue(INTENSITY_PARAMETER_NAME, 0);
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
            SetParameterValue(INTENSITY_PARAMETER_NAME, startVolume);
            _fadeCoroutine = null;
        }
        
        private IEnumerator Process3DSoundPosition(Transform transform) {
            while (IsPlaying()) {
                _eventInstance.set3DAttributes(transform.position.To3DAttributes());
                yield return null;
            }

            _positionCoroutine = null;
        }

        private IEnumerator FadeOutSound(float fadeDuration) {
            _eventInstance.getVolume(out float startVolume);

            float elapsedTime = 0f;
            while (elapsedTime <= fadeDuration) {
                float newVolume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                _eventInstance.setVolume(newVolume);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null; 
            }
            _eventInstance.setVolume(0f);
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
            _fadeCoroutine = null;
        }

        public void ReleaseEventInstance() {
            if (IsValid())
                return;
            
            _eventInstance.release();
        }

        public bool IsPlaying() {
            _eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
            return playbackState is PLAYBACK_STATE.PLAYING or PLAYBACK_STATE.STARTING;
        }

        public void SetParameterValue(string parameterName, float value) =>
            _eventInstance.setParameterByName(parameterName, value);

        public void SetTimelinePosition(int time) {
            if (IsValid() is false)
                return;
            
            _eventInstance.setTimelinePosition(time);
        }

        public int GetTimelinePosition() {
            if (IsValid() is false)
                return 0;
            
            _eventInstance.getTimelinePosition(out int position);
            return position;
        }

        private void ClearFadeCoroutine() {
            if (_fadeCoroutine != null) {
                _coroutineRunner.StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }
    }
}