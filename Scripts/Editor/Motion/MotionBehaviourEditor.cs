using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    public abstract class MotionBehaviourEditor<T> : EditorBase<T> where T : Object
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawProperty(Properties["_multiplier"]);
        }
    }
}