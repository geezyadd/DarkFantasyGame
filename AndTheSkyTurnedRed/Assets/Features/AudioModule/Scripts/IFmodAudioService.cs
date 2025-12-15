using System;
using FMODUnity;
using UnityEngine;

namespace Features.AudioModule.Scripts {
    public interface IFmodAudioService {
        public event Action<EventReference> OnPlaySound;
        public void PlayAudioOneShot(AudioId audioType);
        public void PlayAudioOneShot(AudioId audioType, Vector3 position);
        public ISoundEventInstanceEntity GetFmodEventInstanceEntity(AudioId audioType);
        public void RemoveFmodEventInstanceEntity(AudioId audioType);
    }
}