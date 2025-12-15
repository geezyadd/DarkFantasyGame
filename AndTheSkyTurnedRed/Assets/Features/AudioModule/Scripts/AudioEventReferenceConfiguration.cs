using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Features.AudioModule.Scripts
{
    [CreateAssetMenu(fileName = nameof(AudioEventReferenceConfiguration) + "_Default",
        menuName = "Configurations/AudioEntitiesConfiguration/" + nameof(AudioEventReferenceConfiguration))]
    public class AudioEventReferenceConfiguration : ScriptableObject {
        [field:SerializeField] public List<AudioHolder> AudioHolders { get; private set; }

        public EventReference GetEventReference(AudioId audioID) {
            for (int i = 0; i < AudioHolders.Count; i++)
            {
                if (AudioHolders[i].ID == audioID)
                {
                    return AudioHolders[i].Event;
                }
            }

            throw new Exception($"There is no event with ID: {audioID}");;
        }
    }
}