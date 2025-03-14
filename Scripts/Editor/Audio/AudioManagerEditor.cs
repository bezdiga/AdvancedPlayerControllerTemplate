using HatchStudio.Audio;
using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.AudioEditor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : EditorBase<AudioManager>
    {
        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Audio Manager"));
            serializedObject.Update();
            EditorDrawing.DrawProperty(Properties["_mixer"]);
            EditorDrawing.DrawProperty(Properties["_defaultSnapshot"]);
            
            using(var scope = new EditorDrawing.BorderBoxScopeExpand(new GUIContent("Mixer Groups"),false))
            {
                if (scope.IsExpand)
                {
                    Properties.Draw("_masterGroup");
                    Properties.Draw("_effectsGroup");
                    Properties.Draw("_ambienceGroup");
                    Properties.Draw("_musicGroup");
                    Properties.Draw("_uIGroup");
                }
            }

            using (new EditorDrawing.BorderBoxScope(new GUIContent("Pool Setting")))
            {
                Properties.Draw("_3dAudioSourceCount");
                Properties.Draw("_2dAudioSourceCount");
                Properties.Draw("_loopAudioSourceCount");
                Properties.Draw("_uIAudioSourceCount");
            }
            using (new EditorDrawing.BorderBoxScope(new GUIContent("Duration Setting")))
            {
                Properties.Draw("_minEaseDuration");
                Properties.Draw("_maxEaseDuration");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}