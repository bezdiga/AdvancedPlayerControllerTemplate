using System;


namespace HatchStudio.StateMachine
{
    public class Transition
    {
        public string StateKey { get; private set; }
        public Func<bool> Contidion { get; private set; }
        public bool Value => Contidion.Invoke();

        public static Transition To(string targetStateKey, Func<bool> condition)
        {
            return new Transition()
            {
                StateKey = targetStateKey,
                Contidion = condition
            };
        }
    }
}