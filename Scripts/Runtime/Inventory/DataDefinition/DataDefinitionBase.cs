using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace HatchStudio.Inventory
{
    public abstract class DataDefinitionBase : ScriptableObject
    {
        [SerializeField] [Tooltip("Unique id (auto generated)")]
        private int _id = -1;

        public int Id => _id;
        public abstract string Name { get; protected set; }
        public virtual string Description => string.Empty;
        public virtual Sprite Icon => null;

        
        #region Editor
        
        #if UNITY_EDITOR
        private bool _isDirty;
        [Conditional("UNITY_EDITOR")]
        protected void SetId(int id)
        {
            if (Application.isPlaying)
                return;

            _id = id;
            EditorUtility.SetDirty(this);
            _isDirty = true;
        }
        public bool IsDirty() => _isDirty;
        public void ClearDirty() => _isDirty = false;

        [Conditional("UNITY_EDITOR")]
        protected virtual void OnValidate() => _isDirty = true;

        [Conditional("UNITY_EDITOR")]
        public virtual void Reset() => Name = GetDefaultName();

        [Conditional("UNITY_EDITOR")]
        public virtual void FixIssues() { }

        private string GetDefaultName()
        {
            string defaultName = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(defaultName))
                return Name;

            int nameIndex = defaultName.LastIndexOf("/", StringComparison.Ordinal);

            defaultName = defaultName.Remove(0, nameIndex + 1);
            defaultName = defaultName.Remove(defaultName.IndexOf(".", StringComparison.Ordinal));

            if (defaultName.Contains("_"))
            {
                int index = defaultName.IndexOf('_');
                defaultName = defaultName.Remove(0, index + 1);
            }

            return defaultName;
        }
        #endif
        
        #endregion
    }
}