using UnityEngine;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace HatchStudio.StateMachine
{
    public class StateMachine<TState,TStateData,TStateAsset> 
        where TState : State 
        where TStateAsset : StateAsset
        where TStateData : StateData<TStateAsset>
    {
        public Action<TState, TState> OnChangeState;
        public Func<TState, TStateData> GetStateData { get; set; }
        private Dictionary<string, TState> _states = new Dictionary<string, TState>();
        private Dictionary<Type, Transition[]> _transitions = new Dictionary<Type, Transition[]>();
        private Transition[] _currentTransitions;
        private TState _currentState;

        private readonly List<Object>[] _stateLockers;
        private string[] _stateKeys;
        private bool isEnable;

        public StateMachine(string[] stateKeys)
        {
            _stateKeys = stateKeys;
            _stateLockers = new List<Object>[stateKeys.Length];
        }
        public void Tick()
        {
            if (!isEnable) return;
            var transition = GetTransition();
            if (transition != null)
                TrySetState(transition.StateKey);
            _currentState?.Tick();
        }

       
        private void SetState(string stateKey)
        {
            if (_states.TryGetValue(stateKey, out var state) == false)
                return;
            isEnable = true;
            var previousState = _currentState;
            _currentState?.OnExitState();
            _currentState = state;
            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            OnChangeState?.Invoke(previousState,_currentState);
            if (_currentTransitions == null)
                _currentTransitions = new Transition[0];
            
            _currentState?.OnEnterState();
        }

        public bool TryGetStateData(string stateKey,out StateData<TStateAsset> stateData) => TryGetStateData(_states[stateKey],out stateData);

        public bool TrySetState(string stateKey)
        {
            if (_states.TryGetValue(stateKey, out TState state))
            {
                return TrySetState(state);
            }
            else
            {
                Debug.LogError($"State Machine: Key {stateKey} not found. key count {_states.Count}");
                return false;
            }
        }

        private bool TryGetStateData(TState state, out StateData<TStateAsset> stateData)
        {
            stateData =  GetStateData?.Invoke(state);
            return stateData != null;
        }

        private bool TrySetState(TState newState)
        {
            var stateData = GetStateData?.Invoke(newState);
            if (newState != null && _currentState != newState && stateData.isEnabled)
            {
                SetState(stateData.stateAsset.StateKey);
                return true;
            }
            return false;
        }
        public void SetStateTransitions(string stateKey,TState state, Transition[] transition)
        {
            if (_states.TryGetValue(stateKey, out var outState) == false)
            {
                outState = state;
                _states[stateKey] = outState;
                _transitions[outState.GetType()] = transition;
            }
            else Debug.LogError($"State_Machine : State {state.GetType()} already exist");
        }
        
        private Transition GetTransition()
        {
            foreach (var transition in _currentTransitions)
                if (transition.Value)
                    return transition;

            return null;
        }

        public bool IsCurrent(Type stateType) => _currentState?.GetType() == stateType;

        #region State Bloking

        public void AddStateBlocker(Object blocker, int index)
        {
            // Create a new locker
            if (_stateLockers[index] == null)
            {
                var list = new List<Object>()
                {
                    blocker
                };
                _stateLockers[index] = list;
                if (TryGetStateData(_stateKeys[index], out StateData<TStateAsset> stateData))
                {
                    stateData.isEnabled = false;
                }
            }
            else
            {
                _stateLockers[index].Add(blocker);
                if (TryGetStateData(_stateKeys[index], out StateData<TStateAsset> stateData))
                {
                    stateData.isEnabled = false;
                }
            }
        }

        public void RemoveStateBlocker(Object blocker, int index)
        {
            // Gets existing locker list for the given state type if available
            var list = _stateLockers[index];
            if (list != null && list.Remove(blocker))
            {
                if (list.Count == 0)
                {
                    if (TryGetStateData(_stateKeys[index], out StateData<TStateAsset> stateData))
                    {
                        stateData.isEnabled = true;
                    }
                }
            }
        }

        #endregion
    }
}