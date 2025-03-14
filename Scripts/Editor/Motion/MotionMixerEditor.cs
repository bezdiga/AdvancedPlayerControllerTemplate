using HatchStudio.Motion;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    [CustomEditor(typeof(MotionMixer))]
    public class MotionMixerEditor : EditorBase<MotionMixer>
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawProperty(Properties["_targetTransform"]);
            serializedObject.Update();
            using (var scope = new EditorDrawing.BorderBoxScopeExpand(new GUIContent("Pivot"),false))
            {
                if (scope.IsExpand)
                {
                    Properties.Draw("_pivotOffset");
                    Properties.Draw("_positionOffset");
                    Properties.Draw("_rotationOffset");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}