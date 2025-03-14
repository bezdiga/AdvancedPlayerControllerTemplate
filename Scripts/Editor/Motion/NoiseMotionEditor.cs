using HatchStudio.Motion;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    [CustomEditor(typeof(NoiseMotion))]
    public class NoiseMotionEditor : MotionBehaviourEditor<NoiseMotion>
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                base.OnInspectorGUI();
                EditorDrawing.DrawClassBorderFoldout(Properties["_positionSpring"], new GUIContent("Position Spring"));
                EditorDrawing.DrawClassBorderFoldout(Properties["_rotationSpring"], new GUIContent("Rotation Spring"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}