using HatchStudio.Motion;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    [CustomEditor(typeof(AdditiveForceMotion))]
    public class AdditiveForceMotionEditor : MotionBehaviourEditor<AdditiveForceMotion>
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                base.OnInspectorGUI();
                using (var scope = new EditorDrawing.BorderBoxScopeExpand(new GUIContent("Slow Spring"),false))
                {
                    if (scope.IsExpand)
                    {
                        EditorDrawing.DrawClassBorderFoldout(Properties["_slowPositionSpring"], new GUIContent("Position Spring"));
                        EditorDrawing.DrawClassBorderFoldout(Properties["_slowRotationSpring"], new GUIContent("Rotation Spring"));
                    }
                }

                using (var scope = new EditorDrawing.BorderBoxScopeExpand(new GUIContent("Fast Spring"),false))
                {
                    if (scope.IsExpand)
                    {
                        EditorDrawing.DrawClassBorderFoldout(Properties["_fastPositionSpring"], new GUIContent("Position Spring"));
                        EditorDrawing.DrawClassBorderFoldout(Properties["_fastRotationSpring"], new GUIContent("Rotation Spring"));
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}