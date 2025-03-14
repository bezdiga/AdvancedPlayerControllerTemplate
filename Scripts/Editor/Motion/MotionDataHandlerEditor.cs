using HatchStudio.Motion;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    [CustomEditor(typeof(MotionDataHandler))]
    public class MotionDataHandlerEditor : EditorBase<MotionDataHandler>
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Motion Data Handler"));
            serializedObject.Update();
            EditorDrawing.DrawProperty(Properties["_profile"]);
            serializedObject.ApplyModifiedProperties();
        }
    }
}