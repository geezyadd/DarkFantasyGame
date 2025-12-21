using UnityEngine;

namespace Features.CameraModule.Scripts
{
    public class CameraModel
    {
        public CameraControllerBase CurrentController { get; internal set; }
        public Camera CurrentCamera { get; internal set; }
    }
}