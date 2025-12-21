using System.Collections.Generic;
using UnityEngine;

namespace Features.CameraModule.Scripts
{
    [CreateAssetMenu(fileName = nameof(CameraConfiguration) + "_Default",
        menuName = "Configurations/Camera/" + nameof(CameraConfiguration))]
    public class CameraConfiguration : ScriptableObject
    {
        [field:SerializeField] public Camera Camera;
        [field:SerializeField] public List<CameraHolder> Cameras;
    }
}
