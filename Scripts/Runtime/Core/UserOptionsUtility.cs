using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HatchStudio.Manager
{
    public static class UserOptionsUtility
    {
        private const string SAVE_FILE_EXTENSION = "json";
        private const string OPTIONS_ASSET_PATH = "Options/";
        
        /// <summary>
        /// Gets the save path for the specified user options type.
        /// </summary>
        /// <typeparam name="T">Type of user options.</typeparam>
        /// <returns>The save path for the specified user options type.</returns>
        public static string GetSavePath<T>() where T : UserOptions =>
            GetSavePath(typeof(T));
        
        /// <summary>
        /// Gets the save path for the specified settings type.
        /// </summary>
        /// <param name="settingsType">Type of settings.</param>
        /// <returns>The save path for the specified settings type.</returns>
        public static string GetSavePath(Type settingsType) =>
            $"{GetSaveDirectoryPath()}/{settingsType}.{SAVE_FILE_EXTENSION}";
        
        /// <summary>
        /// Gets the directory path for saving user options.
        /// </summary>
        /// <returns>The directory path for saving user options.</returns>
        private static string GetSaveDirectoryPath()
        {
            string savePath = Application.persistentDataPath + "/Options";

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            return savePath;
        }
        
        /// <summary>
        /// Finds all options fields within the specified user options instance.
        /// </summary>
        /// <param name="instance">User options instance.</param>
        /// <returns>A list of option fields within the specified user options instance.</returns>
        public static List<IOption> FindOptions(UserOptions instance)
        {
            var options = new List<IOption>();
            var fields = instance.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .OrderBy(t => t.MetadataToken);

            foreach (var field in fields)
            {
                if (typeof(IOption).IsAssignableFrom(field.FieldType))
                {
                    IOption option = (IOption)field.GetValue(instance);
                    options.Add(option);
                }
            }

            return options;
        }
        
        /// <summary>
        /// Loads an instance of the specified user options type from assets.
        /// </summary>
        /// <typeparam name="T">Type of user options.</typeparam>
        /// <returns>An instance of the specified user options type.</returns>
        public static T LoadAssetInstance<T>() where T : UserOptions<T>
        {
            T instance;

            string path = $"{OPTIONS_ASSET_PATH}{typeof(T).Name}";
            instance = Resources.Load<T>(path);
            
            if (instance == null)
                instance = ScriptableObject.CreateInstance<T>();
            
            return instance;
        }
        
        /// <summary>
        /// Loads an instance of the specified user options type from assets.
        /// </summary>
        /// <param name="type">Type of user options.</param>
        /// <returns>An instance of the specified user options type.</returns>
        public static UserOptions LoadAssetInstance(Type type)
        {
            UserOptions instance;
            
            string path = OPTIONS_ASSET_PATH;
            if (string.IsNullOrEmpty(type.Name))
            {
                var managers = Resources.LoadAll(path, type);
                instance = managers.Cast<UserOptions>().FirstOrDefault();
            }
            else
            {
                path += type.Name;
                instance = Resources.Load(path, type) as UserOptions;
            }

            if (instance == null)
                instance = (UserOptions)ScriptableObject.CreateInstance(type);
            
            return instance;
        }
    }
}