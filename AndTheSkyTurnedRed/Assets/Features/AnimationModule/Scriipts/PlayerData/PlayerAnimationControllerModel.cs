using System;

namespace Features.AnimationModule.Scriipts.PlayerData
{
    public class PlayerAnimationControllerModel
    {
        public SimpleAnimationControllerBase PlayerAnimationController { get; internal set; }
        public SimpleAttackAnimationFunctionReactor SimplePlayerAttackAnimationFunctionReactor { get; internal set; }
    }
}
