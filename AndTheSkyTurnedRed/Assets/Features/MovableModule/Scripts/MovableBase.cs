using UnityEngine;

namespace Features.MovableModule.Scripts
{
    public abstract class MovableBase : MonoBehaviour
    {
        public abstract bool IsJumping { get; }
        public abstract float AngleToTarget { get;}
        public abstract float GetVelocity { get; }
        public abstract float GetSpeed { get; }
        public abstract void SetDirection(Vector3 direction);
        public abstract void SetSpeed(float speed);
        public abstract void SetRotationSpeed(float speed);
        public abstract void SetJumpDuration(float duration);
        public abstract void SetJumpMultiplier(float jumpMultiplier);
        public abstract void ResetHorizontalVelocity();
        public abstract void SetGravityMultiplier(float gravityMultiplier);
        public abstract void ProcessHorizontalMovement();
        public abstract void UpdateAngle();
        public abstract void ProcessDirectionRotation();
        public abstract void ProcessVerticalMovement();
        public abstract void Jump();
    }
}