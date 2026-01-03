using UnityEngine;

namespace Features.PlayerControlModule.Scripts
{
    public class PlayerControlDataModel
    {
        public bool IsAttacking { get; internal set; }
        public float DistanceToGround { get; internal set; }
        public bool IsNearToGround { get; internal set; }
        public Vector3 CurrentSmoothedDirection { get; internal set; }
    }
}