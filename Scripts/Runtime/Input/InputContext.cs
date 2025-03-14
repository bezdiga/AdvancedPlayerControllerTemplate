using HatchStudios.ToolBox;
using HatchStudios.ToolBox.Utility;
using UnityEngine;

namespace HatchStudios.Input
{
    [CreateAssetMenu(menuName = "Hatch Studio/Input/Input Context",fileName = "InputContext_")]
    public sealed class InputContext : ScriptableObject
    {
        [ClassImplements(typeof(IInputBehaviour), AllowAbstract = false, TypeGrouping = TypeGrouping.ByAddComponentMenu)]
        public SerializedType[] BehaviourTypes;
    }
}