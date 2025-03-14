using System;
using System.Collections.Generic;
using HatchStudio.Manager;
using HatchStudios.ToolBox;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HatchStudios.Input
{
    [DefaultExecutionOrder(ExecutionOrderConstants.SCRIPTABLE_SINGLETON)]
    [CreateAssetMenu(menuName = MANAGERS_MENU_PATH + "Input Manager",fileName = nameof(InputManager))]
    public class InputManager : Manager<InputManager>
    {
        public bool HasEscapeCallbacks => _escapeCallbacks.Count > 0 || _lastEscapeCallbackRemoveFrame == Time.frameCount;
        [SerializeField]
        private InputActionReference _escapeInput;
        private readonly List<UnityAction> _escapeCallbacks = new();
        private readonly List<IInputBehaviour> _behaviours = new(16);
        public InputContext ActiveContext { get; private set; }
        private readonly List<InputContext> _contextStack = new();
        [SerializeField, NotNull]
        
        private InputContext _defaultContext;
        
        private int _lastEscapeCallbackRemoveFrame;
        


        #region Initialization

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Init() => LoadOrCreateInstance();

        
        protected override void OnInitialized()
        {
#if UNITY_EDITOR
            _lastEscapeCallbackRemoveFrame = -1;
            ActiveContext = null;
            _behaviours.Clear();
            _escapeCallbacks.Clear();
            _contextStack.Clear();
#endif
            _escapeInput.action.Enable();
            _escapeInput.action.performed += RaiseEscapeCallback;
            
            PushContext(_defaultContext);
        }
        #endregion

        #region Escape Callback

        public void PushEscapeCallback(UnityAction action)
        {
            if (action == null)
                return;
            
            int index = _escapeCallbacks.IndexOf(action);

            if (index != -1)
                _escapeCallbacks.RemoveAt(index);
            
            _escapeCallbacks.Add(action);
        }
        
        public void PopEscapeCallback(UnityAction action)
        {
            int index = _escapeCallbacks.IndexOf(action);
            if (index != -1)
                _escapeCallbacks.RemoveAt(index);
        }
        
        private void RaiseEscapeCallback(InputAction.CallbackContext context)
        {
            if (_escapeCallbacks.Count == 0)
                return;

            int lastCallbackIndex = _escapeCallbacks.Count - 1;
            var callback = _escapeCallbacks[lastCallbackIndex];
            _escapeCallbacks.RemoveAt(lastCallbackIndex);
            _lastEscapeCallbackRemoveFrame = Time.frameCount;
            callback.Invoke();
        }
        #endregion
        
        
        #region Input Context
        public void PushContext(InputContext context)
        {
            if(context == null)
                return;
            _contextStack.Add(context);
            if (ActiveContext != context)
            {
                ActiveContext = context;
                foreach (var behaviour in _behaviours)
                    UpdateBehaviourStatus(behaviour,context.BehaviourTypes);
                
                // can impliment callback
            }
        }

        public void PopContext(InputContext context)
        {
            if (context == null)
                return;
            int index = _contextStack.IndexOf(context);
            if (index != -1)
            {
                _contextStack.RemoveAt(index);
                if (ActiveContext == context)
                {
                    if(_contextStack.Count == 0)
                        PushContext(_defaultContext);
                    else
                    {
                        var contextToEnable = _contextStack[^1];
                        ActiveContext = contextToEnable;
                        
                        foreach (var behaviour in _behaviours)
                            UpdateBehaviourStatus(behaviour, contextToEnable.BehaviourTypes);
                        // can impliment callback
                    }
                }
            }
        }
        #endregion
        
        #region Input Behaviour

        
        public void RegisterBehaviour(IInputBehaviour playerInputBehaviour)
        {
#if DEBUG
            if (playerInputBehaviour == null)
                throw new ArgumentNullException(nameof(playerInputBehaviour));
            if (_behaviours.Contains(playerInputBehaviour))
            {
                Debug.LogWarning("This behaviour has already been registered.");
                return;
            }
#endif
            
            _behaviours.Add(playerInputBehaviour);
            UpdateBehaviourStatus(playerInputBehaviour, ActiveContext.BehaviourTypes);
        }

        public void UnregisterBehaviour(IInputBehaviour playerInputBehaviour)
        {
#if DEBUG

            if (playerInputBehaviour == null)
                throw new ArgumentNullException(nameof(playerInputBehaviour));

            if (!_behaviours.Remove(playerInputBehaviour))
                Debug.LogError("Trying to unregister a behaviour that has not been registered.");
#else
            _behaviours.Remove(playerInputBehaviour);
#endif
        }
        
        private static void UpdateBehaviourStatus(IInputBehaviour behaviour, SerializedType[] types)
        {
            behaviour.Enabled = behaviour.EnableMode switch
            {
                InputEnableMode.BasedOnContext => Array.Exists(types, serializedType => serializedType.Type == behaviour.GetType()),
                InputEnableMode.AlwaysEnabled => true,
                InputEnableMode.AlwaysDisabled => false,
                InputEnableMode.Manual => behaviour.Enabled,
                _ => false
            };
        }
        #endregion
    }
}