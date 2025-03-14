using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace HatchStudio.Manager
{
    public abstract class UserOptions : ScriptableObject
    {
        private List<IOption> _options;
        private bool _isChanged;
        
        protected const string OPTIONS_MENU_PATH = "Hatch Studio/Options/";
        
        public bool IsChanged
        {
            get => _isChanged;
            private set
            {
                if (_isChanged == value)
                    return;
                
                _isChanged = value;
                Changed?.Invoke(value);
            }
        }
        
        /// <summary>
        /// Event raised when the options have changed.
        /// </summary>
        public event UnityAction<bool> Changed;
        
        /// <summary>
        /// Saves changes to file and applies them.
        /// </summary>
        public void SaveToFileAndApply()
        {
            if (!IsChanged)
                return;
            
            IsChanged = false;
            SaveToFile();
            Apply();
        }
        
        /// <summary>
        /// Loads changes from file and applies them.
        /// </summary>
        public void LoadFromFileAndApply()
        {
            if (!IsChanged)
                return;
            
            IsChanged = false;
            LoadFromFile();
            Apply();
        }

        /// <summary>
        /// Restores default options.
        /// </summary>
        public void RestoreDefaults()
        {
            var defaultInstance = UserOptionsUtility.LoadAssetInstance(GetType());
            var instanceOptions = defaultInstance._options ?? UserOptionsUtility.FindOptions(defaultInstance);
            
            for (int i = 0; i < _options.Count; i++)
                _options[i].BoxedValue = instanceOptions[i].BoxedValue;
        }
        /// <summary>
        /// Applies changes to the options.
        /// </summary>
        protected virtual void Apply() { }
        
        protected virtual void Awake()
        {
            _isChanged = false;
            
            _options = UserOptionsUtility.FindOptions(this);
            foreach (var option in _options)
                option.OptionChanged += OnOptionChanged;
            
            void OnOptionChanged() => IsChanged = true;
        }
        
        /// <summary>
        /// Saves the options to a file.
        /// </summary>
        protected void SaveToFile()
        {
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(UserOptionsUtility.GetSavePath(GetType()), json);
        }
        
        /// <summary>
        /// Loads the options from a file.
        /// </summary>
        protected void LoadFromFile()
        {
            string savePath = UserOptionsUtility.GetSavePath(GetType());
            if (!File.Exists(savePath))
                return;
            
            var json = File.ReadAllText(savePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
    
    public abstract class UserOptions<T> : UserOptions where T : UserOptions<T>
    {
        /// <summary>
        /// Gets the singleton instance of the user options.
        /// </summary>
        public static T Instance { get; private set; }

        /// <summary> Called when default options are loaded. </summary>
        protected virtual void OnDefaultLoaded() { }

        /// <summary>
        /// Creates the instance of user options.
        /// </summary>
        protected static void CreateInstance()
        {
            // Check if the save file exists
            bool saveFileExists = File.Exists(UserOptionsUtility.GetSavePath<T>());
            Instance = saveFileExists ? CreateInstanceFromFile() : CreateInstanceFromAsset();
            Instance.Apply();
        }

        /// <summary>
        /// Creates the instance of user options from a save file.
        /// </summary>
        /// <returns>The instance of user options loaded from file.</returns>
        private static T CreateInstanceFromFile()
        {
            var instance = Instance == null ? CreateInstance<T>() : Instance;
            instance.LoadFromFile();
            return instance;
        }

        /// <summary>
        /// Creates the instance of user options from an asset.
        /// </summary>
        /// <returns>The instance of user options instantiated from asset.</returns>
        private static T CreateInstanceFromAsset()
        {
            var asset = UserOptionsUtility.LoadAssetInstance<T>();
            var instance = asset != null ? Instantiate(asset) : ScriptableObject.CreateInstance<T>();
            
            instance.OnDefaultLoaded();
            
            return instance;
        }
    }
}