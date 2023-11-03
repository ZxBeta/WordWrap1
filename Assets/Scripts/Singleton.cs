using UnityEngine;

namespace LS {
    /** Assume that the object, prefab or class will be initialized in the first scene. Never creates a new object. */
    //FOR WORD-WRAP -> removed dontdestroyonload functionality
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		
        private static T _instance;
        private static object _lock = new object();

        /** Used it in destructor if you don't want to create new instance. */
        public static bool HasInstance {
            get {
                if (applicationIsQuitting) {
                    return false;
                }
                return _instance != null;
            }
        }

        public static T Instance {
            get {
                #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    //Debug.Log(typeof(T) + " [Singleton] is being invoked in editor mode." +  StackTraceUtility.ExtractStackTrace ());
                    if (_instance == null) {
                        _instance = (T)FindObjectOfType(typeof(T));
                    }
                    if (_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " [Singleton][Editor]";
                        Debug.Log(typeof(T) + " [Singleton] instance created.");
                    }
                    return _instance;
                }
                #endif
				
                if (applicationIsQuitting) {
                    Debug.LogError(typeof(T) + " [Singleton] is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor.");
                    return null;
                }
				
                lock (_lock) {
                    if (_instance == null) {
                        _instance = (T)FindObjectOfType(typeof(T));
                    }
                    if (_instance == null && !applicationIsQuitting) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " [Singleton]";
                        //Debug.Log(typeof(T) + " [Singleton] instance created.");
                        //DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }

        /** You must call base Awake. Never override Awake, as it will not be called on first reference */
        protected void Awake () {
            if (_instance && _instance != this) {
                Debug.LogWarning(typeof(T) + " [Singleton] Destroying double instance: " + this.gameObject.name);
                Destroy(this.gameObject);
            } else {
                _instance = this as T;
                //DontDestroyOnLoad(this.gameObject);
            }
        }

        protected bool IsMe () {
            return _instance == this;
        }

        public void OnApplicationPause (bool value) {
            if (value) {
                Save();
            } else {
                RefreshPause();
            }
        }

        private static bool applicationIsQuitting = false;

        public void OnApplicationQuit () {           
            if (!applicationIsQuitting) {
                AppIsQuiting();
                applicationIsQuitting = true;
                Save();
            }
        }

        /** Use this if you want to perform something when app is quiting.*/
        protected virtual void AppIsQuiting () {
        }

        protected virtual void Save () {
			
        }

        protected virtual void RefreshPause () {
			
        }
    }
}