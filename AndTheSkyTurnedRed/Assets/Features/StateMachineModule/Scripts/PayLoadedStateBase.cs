namespace Features.StateMachineModule.Scripts
{
    public abstract class PayLoadedStateBase<TPayload> : StateBase
    {
        public virtual void Enter(TPayload payLoad)
        {
            Enter();
        }
    }
}