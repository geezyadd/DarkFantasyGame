using System;

namespace Features.StateMachineModule.Scripts
{
    public class StateMachine<T> where T : Enum {
        public T CurrentState;
        public event Action<T> OnStateEnter;
        public event Action<T> OnStateExit;

        public void EnterState(T state) {
            OnStateExit?.Invoke(CurrentState);
            CurrentState = state;
            OnStateEnter?.Invoke(CurrentState);
        }
    }
}