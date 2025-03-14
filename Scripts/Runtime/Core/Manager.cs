using System.Linq;
using UnityEngine;

namespace HatchStudio.Manager
{
    [DefaultExecutionOrder(ExecutionOrderConstants.SCRIPTABLE_SINGLETON)]
    public abstract class Manager : ScriptableObject
    {
        private static GameObject s_ManagersRoot;
        protected const string MANAGERS_MENU_PATH = "Hatch Studio/Managers/";
        protected const string MANAGERS_PATH = "Managers/";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reload()
        {
            if (s_ManagersRoot != null)
                DestroyImmediate(s_ManagersRoot);
        }

        protected static U CreateRuntimeObject<U>(string objName = "RuntimeObject") where U : MonoBehaviour
        {
            var managersRoot = GetManagersRoot();
            var runtimeObject = new GameObject(objName).AddComponent<U>();
            runtimeObject.transform.parent = managersRoot.transform;
            runtimeObject.gameObject.name = objName;

            return runtimeObject;
        }
        
        protected static GameObject GetManagersRoot()
        {
            if (s_ManagersRoot == null)
            {
                s_ManagersRoot = new GameObject("Managers")
                {
                    tag = TagConstants.GAME_CONTROLLER
                };

                DontDestroyOnLoad(s_ManagersRoot);
            }

            return s_ManagersRoot;
        }
    }

    public abstract class Manager<T> : Manager where T : Manager<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual void OnInitialized(){}
        
        protected static void LoadOrCreateInstance()
        {
            if (Instance == null)
            {
                Instance = LoadInstance();
                if (Instance == null)
                    Instance = CreateInstance<T>();
            }

            Instance.OnInitialized();
        }
        
        protected static void CreateInstance()
        {
            if (Instance == null)
                Instance = CreateInstance<T>();
            
            Instance.OnInitialized();
        }
        
        private static T LoadInstance()
        {
            T instance;
            
            string path = MANAGERS_PATH + typeof(T).Name;
            instance = Resources.Load<T>(path);

            if (instance == null)
            {
                var managers = Resources.LoadAll<T>(MANAGERS_PATH);
                instance = managers.FirstOrDefault();
            }

            return instance;
        }
    }
}