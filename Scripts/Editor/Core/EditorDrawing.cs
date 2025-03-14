using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace HatchStudios.Editor.Utils
{

    public static class EditorDrawing
    {
        public static class Styles
        {
            public static Color? labelColor = null;
            public static GUIStyle borderBoxHeaderStyle
            {
                get
                {
                    GUIStyle style = new();
                    style.margin = new RectOffset(3, 3, 2, 2);
                    style.padding = new RectOffset(5, 5, 2, 5);
                    return style;
                }
            }
            
            public static GUIStyle miniBoldLabelCenter
            {
                get
                {
                    GUIStyle style = new(EditorStyles.miniBoldLabel);
                    style.alignment = TextAnchor.MiddleLeft;

                    if (labelColor.HasValue)
                        style.normal.textColor = labelColor.Value;

                    return style;
                }
            }
            public static GUIStyle LabelCenter
            {
                get
                {
                    GUIStyle style = new(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleLeft;

                    if (labelColor.HasValue)
                        style.normal.textColor = labelColor.Value;

                    return style;
                }
            }
            
            public static GUIStyle borderBoxStyle
            {
                get
                {
                    GUIStyle style = new();
                    style.margin = new RectOffset(3, 3, 2, 2);
                    style.padding = new RectOffset(5, 5, 5, 5);
                    return style;
                }
            }
        }
        
        public class BorderBoxScope : GUI.Scope
        {
            
            public BorderBoxScope(bool roundedBox = true)
            {
                BeginBorderLayout(roundedBox);
            }

            public BorderBoxScope(GUIContent title, float headerHeight = 18f, bool roundedBox = true)
            {
                BeginHeaderBorderLayout(title, headerHeight, roundedBox);
            }

            
            protected override void CloseScope()
            {
                EndBorderHeaderLayout();
            }
        }
        public class BorderBoxScopeExpand : GUI.Scope
        {
            public static Dictionary<string, bool> expandStates = new Dictionary<string, bool>();
            private string key;
            public Rect headerRect;
            public bool IsExpand => expandStates.TryGetValue(key,out bool expand) != null ? expand : true;
            

            public BorderBoxScopeExpand(GUIContent title, bool isExpand, float headerHeight = 18f, bool roundedBox = true)
            {
                key = title.text;
                if (!expandStates.ContainsKey(key))
                    expandStates[key] = isExpand;
                BeginExpandBorderLayout(title,out headerRect);
            }
            
            
            protected override void CloseScope()
            {
                
                if (expandStates.TryGetValue(key, out bool expand) != null)
                {
                    if(expand)
                        EndBorderHeaderLayout();
                }
                else EndBorderHeaderLayout();
            }
        }


        public static void DrawInspectorHeader(GUIContent title, Object script = null)
        {
            GUIStyle headerStyle = new(EditorStyles.boldLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            Rect rect = GUILayoutUtility.GetRect(1, 30);
            headerStyle.normal.textColor = Color.white;
            title.text = title.text.ToUpper();
            
            EditorGUI.LabelField(rect, title, headerStyle);
        }
        /// <summary>
        /// Begin a bordered vertical group;
        /// </summary>
        public static void BeginBorderLayout(bool roundedBox = true)
        {
            Rect drawingRect = EditorGUILayout.BeginVertical(Styles.borderBoxStyle);
            GUI.Box(drawingRect, GUIContent.none, new GUIStyle(roundedBox ? "HelpBox" : "Tooltip"));
        }
        
        /// <summary>
        /// Begin a expand bordered vertical group;
        /// </summary>
        public static void BeginExpandBorderLayout(GUIContent title,out Rect headerRect,float headerHeight = 18f,bool roundedBox = true)
        {
            bool foldoutResult = BorderBoxScopeExpand.expandStates[title.text];
            headerRect = EditorGUILayout.GetControlRect(false, headerHeight + 4f);
            Rect boxRect = headerRect;//foldoutResult ? headerRect : EditorGUILayout.BeginVertical(Styles.borderBoxHeaderStyle);
            //boxRect.yMin -= headerHeight + 6f;

            if (foldoutResult)
            {
                Rect drawingRect = EditorGUILayout.BeginVertical(Styles.borderBoxHeaderStyle);
                boxRect.yMax = drawingRect.yMax;
            }
            GUI.Box(boxRect, GUIContent.none, new GUIStyle(roundedBox ? "HelpBox" : "Tooltip"));
            BorderBoxScopeExpand.expandStates[title.text] = DrawFoldoutHeader(headerRect, title, BorderBoxScopeExpand.expandStates[title.text],true);
        }
        
        
        /// <summary>
        /// Begin a bordered vertical header group;
        /// </summary>
        public static Rect BeginHeaderBorderLayout(GUIContent title, float headerHeight = 18f, bool roundedBox = true)
        {
            Rect headerRect = EditorGUILayout.GetControlRect(false, headerHeight + 4f);
            Rect drawingRect = EditorGUILayout.BeginVertical(Styles.borderBoxHeaderStyle);
            drawingRect.yMin -= headerHeight + 6f;

            GUI.Box(drawingRect, GUIContent.none, new GUIStyle(roundedBox ? "HelpBox" : "Tooltip"));
            DrawHeader(headerRect, title);
            return headerRect;
        }
        /// <summary>
        /// End a bordered vertical group.
        /// </summary>
        public static void EndBorderHeaderLayout()
        {
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw header by specified rect.
        /// </summary>
        private static void DrawHeader(Rect headerRect, GUIContent title, GUIStyle labelStyle = null)
        {
            Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
            EditorGUI.DrawRect(headerRect, headerColor);

            Rect labelRect = new Rect(headerRect.x + 4f, headerRect.y - 1f, headerRect.width - 4f, headerRect.height);
            EditorGUI.LabelField(labelRect, title, labelStyle ?? Styles.miniBoldLabelCenter);
        }

        public static void DrawProperty(SerializedProperty property)
        {
            float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
            Rect headerRect = EditorGUILayout.GetControlRect(false, propertyHeight + 6f);
            
            Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0.25f);
            
            float verticalOffset = (headerRect.height - propertyHeight) / 2f;
            Rect propertyRect = new Rect(headerRect.x + 4, headerRect.y + verticalOffset, headerRect.width - 4, propertyHeight);
            
            // Draw background
            EditorGUI.DrawRect(headerRect, headerColor);
            
            EditorGUI.PropertyField(propertyRect, property);
        }

        public static void DrawComponent(object component,Type componentType)
        {
            if (component == null)
            {
                EditorGUILayout.LabelField("Component is null.");
                return;
            }
            
      
            Debug.LogError("Draw type " + componentType);
            var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

            foreach (var field in fields)
            {

                bool isHiddenInInspector = field.IsDefined(typeof(HideInInspector), false);
                if (isHiddenInInspector || field.DeclaringType != componentType) continue;
                
                bool isSerializable = field.IsPublic || field.IsDefined(typeof(SerializeField),false) ;
                if (!isSerializable) continue;

                object fieldValue = field.GetValue(component);
                
                DrawField(field, fieldValue, component);
            }
        }
        private static void DrawField(FieldInfo field, object fieldValue, object component)
        {
            // Verifică tipul câmpului și desenează corespunzător
            if (field.FieldType == typeof(int))
            {
                int newValue = EditorGUILayout.IntField(field.Name, (int)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType == typeof(float))
            {
                float newValue = EditorGUILayout.FloatField(field.Name, (float)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType == typeof(string))
            {
                string newValue = EditorGUILayout.TextField(field.Name, (string)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType == typeof(bool))
            {
                bool newValue = EditorGUILayout.Toggle(field.Name, (bool)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType.IsEnum)
            {
                object newValue = EditorGUILayout.EnumPopup(field.Name, (Enum)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType == typeof(Vector3))
            {
                Vector3 newValue = EditorGUILayout.Vector3Field(field.Name, (Vector3)fieldValue);
                field.SetValue(component, newValue);
            }
            else if (field.FieldType == typeof(Color))
            {
                Color newValue = EditorGUILayout.ColorField(field.Name, (Color)fieldValue);
                field.SetValue(component, newValue);
            }
            else
            {
                EditorGUILayout.LabelField(field.Name, $"(Unsupported type: {field.FieldType.Name})");
            }
        }
        public static PropertyCollection GetAllProperties(SerializedObject serializedObject)
        {
            PropertyCollection properties = new();

            SerializedProperty property = serializedObject.GetIterator();
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextSiblingProperty = property.Copy();

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    properties.Add(currentProperty.name, currentProperty.Copy());
                }
                while (currentProperty.NextVisible(false));
            }

            return properties;
        }

        public static PropertyCollection GetAllProperties(SerializedProperty classProperty)
        {
            PropertyCollection propertyCollection = new();
            var classChildrens = classProperty.GetVisibleChildrens();
            foreach (var child in classChildrens)
            {
                propertyCollection.Add(child.name,child.Copy());
            }
            
            return propertyCollection;
        }

        public static Rect DrawScriptableFoldout(SerializedProperty scriptableProperty,GUIContent title, ref bool expanded,ref bool toggle, float headerHeight = 18f)
        {
            SerializedObject serializedObject = scriptableProperty.propertyType == SerializedPropertyType.ObjectReference
                ? new SerializedObject(scriptableProperty.objectReferenceValue)
                : scriptableProperty.serializedObject;
            SerializedProperty iterator = serializedObject.GetIterator();
            
            bool hasChilds = HasIteratorChilds(iterator);
            
            if (BeginFoldoutBorderLayout(scriptableProperty, title, out Rect headerRect,ref toggle, headerHeight,
                    canExpand: hasChilds,true))
            {
                if (hasChilds)
                {
                    do
                    {
                        EditorGUI.BeginChangeCheck();
                        {
                            SerializedProperty child = serializedObject.FindProperty(iterator.name);
                            bool isArray = IsArray(child);

                            if (isArray) EditorGUI.indentLevel++;
                            {
                                EditorGUILayout.PropertyField(child, true);
                            }
                            if (isArray) EditorGUI.indentLevel--;
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    while (iterator.NextVisible(false));
                }

                EndBorderHeaderLayout();
            }

            return headerRect;
        }
        public static Rect DrawClassBorderFoldout(SerializedProperty classProperty, GUIContent title,
            float headerHeight = 18, bool roundedBox = true)
        {
            var classChildrens = classProperty.GetVisibleChildrens();
            bool dummyToggle = false;
            if (BeginFoldoutBorderLayout(classProperty, title, out Rect headerRect, ref dummyToggle,headerHeight, true,false,roundedBox))
            {
                foreach (var child in classChildrens)
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        bool isArray = IsArray(child);

                        if (isArray) EditorGUI.indentLevel++;
                        {
                            EditorGUILayout.PropertyField(child, true);
                        }
                        if (isArray) EditorGUI.indentLevel--;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        classProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            return headerRect;
        }
        
       
        
        public static bool HasIteratorChilds(SerializedProperty iterator)
        {
            return iterator != null && iterator.NextVisible(true) && iterator.NextVisible(false);
        }

        
        public static bool IsArray(SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }
        /// <summary>
        /// Begin a bordered vertical foldout group.
        /// </summary>
        public static bool BeginFoldoutBorderLayout(SerializedProperty foldoutProperty, GUIContent title, out Rect headerRect,ref bool toggle, float headerHeight = 18f, bool canExpand = true,bool useToggle = false, bool roundedBox = true)
        {
            headerRect = EditorGUILayout.GetControlRect(false, headerHeight + 4f);
            Rect boxRect = headerRect;
            bool foldoutResult = foldoutProperty.isExpanded && canExpand;

            if (foldoutResult)
            {
                Rect drawingRect = EditorGUILayout.BeginVertical(Styles.borderBoxHeaderStyle);
                boxRect.yMax = drawingRect.yMax;
            }

            GUI.Box(boxRect, GUIContent.none, new GUIStyle(roundedBox ? "HelpBox" : "Tooltip"));
            if(useToggle)
                foldoutProperty.isExpanded = DrawFoldoutHeaderToggle(headerRect, title,  ref toggle,foldoutProperty.isExpanded,canExpand,true);
            else foldoutProperty.isExpanded = DrawFoldoutHeader(headerRect, title, foldoutProperty.isExpanded,canExpand);
            return foldoutResult;
        }
        
        /// <summary>
        /// Draw foldout header by specified rect.
        /// </summary>
        public static bool DrawFoldoutHeader(Rect headerRect, GUIContent title, bool expanded,bool canExpand = true)
        {
            // Constants
            Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            // Draw header background
            EditorGUI.DrawRect(headerRect, headerColor);

            // Set up initial positions
            Rect foldoutRect = headerRect;
            foldoutRect.width = singleLineHeight;
            foldoutRect.x += 4f;
            
            // if can expand, draw foldout
            if (canExpand)
            {
                GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);
                foldoutRect = new Rect(headerRect.x + 4f, headerRect.y, singleLineHeight, headerRect.height);
            }
            
            // If Define and draw title label
            Rect labelRect = new Rect(foldoutRect.xMax, headerRect.y - 1f, headerRect.width - foldoutRect.xMax + 4f, headerRect.height);
            EditorGUI.LabelField(labelRect, title, Styles.miniBoldLabelCenter);

            // Handle mouse events for foldout interaction
            headerRect.xMax -= singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            
            // not register click event at de right part
            headerRect.width = 250;
            
            Event e = Event.current;
            if (canExpand && headerRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return expanded;
        }
        
        public static bool DrawFoldoutHeaderToggle(Rect headerRect, GUIContent title,ref bool toggle, bool expanded,bool canExpand = true,bool useTogglee = false)
        {
            // Constants
            Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            // Draw header background
            EditorGUI.DrawRect(headerRect, headerColor);

            // Set up initial positions
            Rect foldoutRect = headerRect;
            foldoutRect.width = singleLineHeight;
            foldoutRect.x += 4f;
            
            // if can expand, draw foldout
            if (canExpand)
            {
                GUI.Toggle(foldoutRect, expanded, GUIContent.none, EditorStyles.foldout);
                // = new Rect(headerRect.x + 4f, headerRect.y, singleLineHeight, headerRect.height);
            }
            foldoutRect.x += singleLineHeight;
            if (useTogglee)
            {
                Rect toggleRect = new Rect(foldoutRect.x, headerRect.y, singleLineHeight, headerRect.height);
                toggle = GUI.Toggle(toggleRect, toggle, new GUIContent("", "Enabled"), EditorStyles.toggle);
            }
            // If Define and draw title label
            Rect labelRect = new Rect(foldoutRect.xMax, headerRect.y - 1f, headerRect.width - foldoutRect.xMax + 4f, headerRect.height);
            EditorGUI.LabelField(labelRect, title, Styles.miniBoldLabelCenter);

            // Handle mouse events for foldout interaction
            headerRect.xMax -= singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Event e = Event.current;
            if (canExpand && headerRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
            {
                expanded = !expanded;
                e.Use();
            }

            return expanded;
        }

        
        /// <summary>
        /// Set custom icon size.
        /// </summary>
        public static Vector2 SetIconSize(float iconSize)
        {
            Vector2 prevIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(iconSize, iconSize));
            return prevIconSize;
        }
        
        /// <summary>
        /// Reset custom icon size.
        /// </summary>
        public static void ResetIconSize()
        {
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }
        
    }
}
