using System;

namespace HatchStudio.StateMachine
{
    [Serializable]
    public class StateData<TStateAsset> where TStateAsset : StateAsset
    {
        public TStateAsset stateAsset;
        public bool isEnabled;
        
    }
}