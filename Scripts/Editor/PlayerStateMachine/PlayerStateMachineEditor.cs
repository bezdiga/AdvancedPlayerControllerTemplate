using System;
using System.Linq;

using HatchStudios.Editor.Core;
using HatchStudios.Editor.Utils;
using UnityEngine;

namespace HatchStudios.PlayerController.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(PlayerMovementController))]
    public class PlayerStateMachineEditor : EditorBase<PlayerMovementController>
    {
        public static Texture2D FSMIcon => Resources.Load<Texture2D>("Editor/icons/fsm");
        private string _lastStateName = "";

        private Color _highlightColor = new Color(1f, 0.8f, 0.2f);
        bool showDebugInfo = false;

        /*private FieldInfo stateMachineField;*/
        
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                Properties.Draw("_playerStatesGroup");
                Properties.Draw("_inputHandler");
                Properties.Draw("_motor");
            }
            EditorGUILayout.Space();
            using (new EditorDrawing.BorderBoxScope(new GUIContent("Modifier")))
            {
                Properties.Draw("_speedMultiplier");
                Properties.Draw("_baseAcceleration");
                Properties.Draw("_baseDeceleration");
            }
            //EditorDrawing.DrawClassBorderFoldout(Properties["_inputHandler"], new GUIContent("InputHandler"));
            //EditorDrawing.DrawClassBorderFoldout(Properties["PlayerControllerSettings"], new GUIContent("Controller Settings"));
            
            if (Properties["_playerStatesGroup"].objectReferenceValue != null)
            {
                SerializedObject statesSerializeObject = Application.isPlaying && Target.StateAssetRuntime != null
                    ? new SerializedObject(Target.StateAssetRuntime)
                    : new SerializedObject(Properties["_playerStatesGroup"].objectReferenceValue);
                
                PropertyCollection stateProperties = EditorDrawing.GetAllProperties(statesSerializeObject);
                
                SerializedProperty statesProperty = stateProperties["PlayerStates"];
                
                statesSerializeObject.Update();
                {
                    if (statesProperty.arraySize > 0)
                    {
                        Type currentState = null;
                        Type previousState = null;
                        if (Application.isPlaying)
                        {
                            if (Target.CurrentState != null)
                                currentState = Target.CurrentState.stateAsset?.GetType();

                            if (Target.PreviousState != null)
                                previousState = Target.PreviousState.stateAsset?.GetType();
                        }

                        using (var scope = new EditorDrawing.BorderBoxScopeExpand(new GUIContent("Stats"),true))
                        {
                            if (scope.IsExpand)
                            {
                                for (int i = 0; i < statesProperty.arraySize; i++)
                                {
                                    SerializedProperty state = statesProperty.GetArrayElementAtIndex(i);
                                    SerializedProperty stateAsset = state.FindPropertyRelative("stateAsset");
                                    
                                    SerializedProperty isEnabled = state.FindPropertyRelative("isEnabled");
                                    
                                    bool expanded = stateAsset.isExpanded;
                                    bool toggle = isEnabled.boolValue;
                                    
                                    string name = ((PlayerStateAsset)stateAsset.objectReferenceValue).Name.Split('/')
                                        .Last();
                                    EditorDrawing.SetIconSize(12f);
                                    GUIContent title = EditorGUIUtility.TrTextContentWithIcon(" " + name, FSMIcon);
                                    
                                    Rect header = EditorDrawing.DrawScriptableFoldout(stateAsset, title, ref expanded,ref toggle);
                                    EditorDrawing.ResetIconSize();
                                    isEnabled.boolValue = toggle;
                                    
                                    if (Application.isPlaying)
                                    {
                                        //Debug.LogError("current state: " + currentState + " reference: " + stateAsset.objectReferenceValue.GetType() + " is true: " + (stateAsset.objectReferenceValue.GetType() == currentState));
                                        if (currentState != null && stateAsset.objectReferenceValue.GetType() == currentState)
                                        {
                                            
                                            Rect currStateRect = header;
                                            currStateRect.xMin = header.xMax - EditorGUIUtility.singleLineHeight;

                                            GUIContent currStateIndicator = EditorGUIUtility.TrIconContent("greenLight", "Current State");
                                            EditorGUI.LabelField(currStateRect, currStateIndicator);
                                        }

                                        if (previousState != null && stateAsset.objectReferenceValue.GetType() == previousState)
                                        {
                                            Rect prevStateRect = header;
                                            prevStateRect.xMin = header.xMax - EditorGUIUtility.singleLineHeight;

                                            GUIContent prevStateIndicator = EditorGUIUtility.TrIconContent("orangeLight", "Previous State");
                                            EditorGUI.LabelField(prevStateRect, prevStateIndicator);
                                        }
                                        Repaint();
                                    }
                                }
                            }
                        }
                        
                        
                    }
                }
                statesSerializeObject.ApplyModifiedProperties();
                
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}