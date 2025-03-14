using System;
using System.Collections.Generic;
using System.Linq;
using HatchStudio.StateMachine;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace HatchStudios.PlayerController.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(PlayerStatesGroup))]
    public class PlayerStatesGroupEditor : Editor
    {
        public struct StatePair
        {
            public Type stateType;
            public string stateName;
        }
        public static Texture2D FSMIcon => Resources.Load<Texture2D>("Editor/icons/fsm");
        
        PlayerStatesGroup Target;
        private List<StatePair> states = new();
        private SerializedProperty _playerStateAssetSerializedProperty;

        private void OnEnable()
        {
            _playerStateAssetSerializedProperty = serializedObject.FindProperty("PlayerStates");
            Target = target as PlayerStatesGroup;
            foreach (var type in TypeCache.GetTypesDerivedFrom<PlayerStateAsset>().Where(x => !x.IsAbstract))
            {
                if (Target.PlayerStates.Any(x => x.GetType() == type))
                    continue;

                PlayerStateAsset instance = (PlayerStateAsset)CreateInstance(type);
                states.Add(new StatePair()
                {
                    stateType = type,
                    stateName = instance.Name
                });
                instance = null;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawInspectorHeader(new GUIContent("Player States Machine"));
            
            
            serializedObject.Update();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Player States", EditorStyles.miniBoldLabel);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical("box");
            if (_playerStateAssetSerializedProperty.arraySize > 0)
            {
                for (int i = 0; i < _playerStateAssetSerializedProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    SerializedProperty state = _playerStateAssetSerializedProperty.GetArrayElementAtIndex(i);
                    SerializedProperty stateAsset = state.FindPropertyRelative("stateAsset");
                    SerializedProperty isEnabled = state.FindPropertyRelative("isEnabled");
                    
                    bool toggle = isEnabled.boolValue;
                    
                    string name = ((PlayerStateAsset)stateAsset.objectReferenceValue).Name.Split('/').Last();
                    
                    GUIContent title = EditorGUIUtility.TrTextContentWithIcon(" " + name, FSMIcon);

                    EditorGUILayout.LabelField(title);
                    isEnabled.boolValue = toggle;
                    Rect headerRect = EditorGUILayout.GetControlRect(false, 22f);
                    Rect dropdownRect = headerRect;
                    dropdownRect.xMin = headerRect.xMax - EditorGUIUtility.singleLineHeight;
                    dropdownRect.x -= EditorGUIUtility.standardVerticalSpacing;
                    dropdownRect.y += headerRect.height / 2 - 8f;
                    GUIContent dropdownIcon = EditorGUIUtility.TrIconContent("_Menu", "State Menu");
                    
                    int index = i;
                    if (GUI.Button(dropdownRect, dropdownIcon, EditorStyles.iconButton))
                    {
                        GenericMenu popup = new GenericMenu();

                        if (index > 0)
                        {
                            popup.AddItem(new GUIContent("↑ Move Up"), false, () =>
                            {
                                _playerStateAssetSerializedProperty.MoveArrayElement(index, index - 1);
                                serializedObject.ApplyModifiedProperties();
                            });
                        }
                        else popup.AddDisabledItem(new GUIContent("↑ Move Up"));
                        if (index <  _playerStateAssetSerializedProperty.arraySize - 1)
                        {
                            popup.AddItem(new GUIContent("↓ Move Down"), false, () =>
                            {
                                _playerStateAssetSerializedProperty.MoveArrayElement(index, index + 1);
                                serializedObject.ApplyModifiedProperties();
                            });
                        }
                        else popup.AddDisabledItem(new GUIContent("↓ Move Down"));
                        

                        popup.ShowAsContext();
                    }
                    
                    /*if (GUILayout.Button("↑", GUILayout.Width(30)))
                    {
                        if (index > 0)
                        {
                            _playerStateAssetSerializedProperty.MoveArrayElement(index, index - 1);
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    if (GUILayout.Button("↓", GUILayout.Width(30)))
                    {
                        if (index <  _playerStateAssetSerializedProperty.arraySize - 1)
                        {
                            _playerStateAssetSerializedProperty.MoveArrayElement(index, index + 1);
                            serializedObject.ApplyModifiedProperties();
                        }
                    }*/
                    
                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        UnityEngine.Object stateAssetObj = stateAsset.objectReferenceValue;
                        _playerStateAssetSerializedProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.RemoveObjectFromAsset(stateAssetObj);
                        EditorUtility.SetDirty(target);
                        AssetDatabase.SaveAssets();
                        break;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                Rect stateButtonRect = EditorGUILayout.GetControlRect(GUILayout.Width(100f), GUILayout.Height(20f));
                
                using (new EditorGUI.DisabledGroupScope(Application.isPlaying || states.Count <= 0))
                {
                    if (GUI.Button(stateButtonRect, "Add State"))
                    {
                        Rect dropdownRect = stateButtonRect;
                        dropdownRect.width = 250f;
                        dropdownRect.height = 0f;
                        dropdownRect.y += 21f;
                        dropdownRect.x += (stateButtonRect.width - dropdownRect.width) / 2;

                        StatesDropdown statesDropdown = new(new(), states);
                        statesDropdown.OnItemPressed = (type) => AddPlayerState(type);
                        statesDropdown.Show(dropdownRect);
                    }
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspectorHeader(GUIContent title)
        {
            GUIStyle headerStyle = new(EditorStyles.boldLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };

            headerStyle.normal.textColor = Color.white;
            title.text = title.text.ToUpper();
            
            Rect rect = GUILayoutUtility.GetRect(1, 30);
            ColorUtility.TryParseHtmlString("#181818", out Color color);

            EditorGUI.DrawRect(rect, color);
            
            EditorGUI.LabelField(rect, title, headerStyle);
        }
        
        private void AddPlayerState(Type type)
        {
            /*var subObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Target));
            foreach (var obj in subObjects)
            {
                if (obj == myData) 
                    continue;

                // Șterge sub-obiectul
                AssetDatabase.RemoveObjectFromAsset(obj);
                DestroyImmediate(obj, true);
            }*/
            
            ScriptableObject component = CreateInstance(type);
            PlayerStateAsset stateAsset = (PlayerStateAsset)component;
            component.name = stateAsset.Name.Split("/").Last();
            
            Undo.RegisterCreatedObjectUndo(component,"Add Player Stat");
            
            if(EditorUtility.IsPersistent(target))
                AssetDatabase.AddObjectToAsset(component,target);
            
            Target.PlayerStates.Add(new StateData<PlayerStateAsset>
            {
                stateAsset = stateAsset,
                isEnabled = true
            });
            states.RemoveAll(x => x.stateType == type);
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            
        }
        
        public class StatesDropdown : AdvancedDropdown
        {
            private readonly IEnumerable<StatePair> modules;
            public Action<Type> OnItemPressed;

            private class StateElement : AdvancedDropdownItem
            {
                public Type moduleType;

                public StateElement(string displayName, Type moduleType) : base(displayName)
                {
                    this.moduleType = moduleType;
                }
            }

            public StatesDropdown(AdvancedDropdownState state, IEnumerable<StatePair> states) : base(state)
            {
                this.modules = states;
                minimumSize = new Vector2(minimumSize.x, 270f);
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Player States");
                var groupMap = new Dictionary<string, AdvancedDropdownItem>();

                foreach (var module in modules)
                {
                    Type type = module.stateType;
                    string name = module.stateName;

                    // Split the name into groups
                    string[] groups = name.Split('/');

                    // Create or find the groups
                    AdvancedDropdownItem parent = root;
                    for (int i = 0; i < groups.Length - 1; i++)
                    {
                        string groupPath = string.Join("/", groups.Take(i + 1));
                        if (!groupMap.ContainsKey(groupPath))
                        {
                            var newGroup = new AdvancedDropdownItem(groups[i]);
                            parent.AddChild(newGroup);
                            groupMap[groupPath] = newGroup;
                        }
                        parent = groupMap[groupPath];
                    }

                    // Create the item and add it to the last group
                    StateElement item = new StateElement(groups.Last(), type);

                    //item.icon = MotionIcon;
                    parent.AddChild(item);
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                StateElement element = (StateElement)item;
                OnItemPressed?.Invoke(element.moduleType);
            }
        }
    
    }
}