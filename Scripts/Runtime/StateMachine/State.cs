namespace HatchStudio.StateMachine
{
    public abstract class State
    {
        public virtual void OnEnterState(){}
        public virtual void Tick(){}
        public virtual void OnExitState(){}
    }
}