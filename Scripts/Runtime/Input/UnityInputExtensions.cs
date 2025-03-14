using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HatchStudios.Input._Assets.Scripts.Runtime.Input
{
    public static class UnityInputExtensions
    {
        private static readonly Dictionary<InputActionReference, int> s_EnabledActions = new();
        
        public static void RegisterStarted(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            CheckForNull(actionRef);

            Enable(actionRef);
            actionRef.action.started += callback;
        }
        
        public static void Enable(this InputActionReference actionRef)
        {
            CheckForNull(actionRef);

            if (s_EnabledActions.TryGetValue(actionRef, out var listenerCount))
                s_EnabledActions[actionRef] = listenerCount + 1;
            else
            {
                s_EnabledActions.Add(actionRef, 1);
                actionRef.action.Enable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckForNull(InputActionReference actionRef)
        {
#if DEBUG
            if (actionRef == null)
                Debug.LogError("The passed input action is null, you need to set it in the inspector.");
#endif
        }
    }
}