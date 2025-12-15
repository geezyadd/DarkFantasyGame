using System;
using System.Collections.Generic;
using Features.CoroutineRunnerModule.Scripts;
using FMODUnity;
using UnityEngine;

namespace Features.AudioModule.Scripts {
    public class FmodAudioService : IFmodAudioService {
        private readonly Dictionary<AudioId, ISoundEventInstanceEntity> _fmodEventInstanceEntities = new();
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly AudioEventReferenceConfiguration _audioEventReferenceConfiguration;
        public event Action<EventReference> OnPlaySound;

        public FmodAudioService(ICoroutineRunner coroutineRunner, AudioEventReferenceConfiguration audioEventReferenceConfiguration) {
            _coroutineRunner = coroutineRunner;
            _audioEventReferenceConfiguration = audioEventReferenceConfiguration;
        }

        public void PlayAudioOneShot(AudioId audioType)
        {
            EventReference eventReference = _audioEventReferenceConfiguration.GetEventReference(audioType);
            if(eventReference.IsNull)
                return;
            
            RuntimeManager.PlayOneShot(eventReference);
            OnPlaySound?.Invoke(eventReference);
        }

        public void PlayAudioOneShot(AudioId audioType, Vector3 position)
        {
            EventReference eventReference = _audioEventReferenceConfiguration.GetEventReference(audioType);
           if(eventReference.IsNull)
               return;

           RuntimeManager.PlayOneShot(eventReference, position);
           OnPlaySound?.Invoke(eventReference);
        }
        
        public ISoundEventInstanceEntity GetFmodEventInstanceEntity(AudioId audioType) {
            EventReference eventReference = _audioEventReferenceConfiguration.GetEventReference(audioType);
            if (eventReference.IsNull)
                return new SoundEventInstanceEntity();
            
            RemoveFmodEventInstanceEntity(audioType);
            ISoundEventInstanceEntity soundEventInstanceEntity = new SoundEventInstanceEntity(eventReference, _coroutineRunner);
            _fmodEventInstanceEntities.Add(audioType, soundEventInstanceEntity);
            return soundEventInstanceEntity;
        }
        
        public void RemoveFmodEventInstanceEntity(AudioId audioType) {
            if(_fmodEventInstanceEntities.ContainsKey(audioType) is false)
                return;
            
            _fmodEventInstanceEntities[audioType].ReleaseEventInstance();
            _fmodEventInstanceEntities.Remove(audioType);
        }
    }
}