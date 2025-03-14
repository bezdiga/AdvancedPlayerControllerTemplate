using HatchStudio.Motion;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.MotionEditor
{
    [CustomEditor(typeof(BobMotion))]
    public class BobMotionEditor : MotionBehaviourEditor<BobMotion>
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            using (new EditorDrawing.BorderBoxScope(new GUIContent("Setting")))
            {
                Properties.Draw("_resetSpeed");
                Properties.Draw("_rotationDelay");
            }
            serializedObject.ApplyModifiedProperties();
            
            
        }
    }
}