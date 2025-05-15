using UnityEngine;

namespace Core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                }

                if (_instance == null)
                {
                    _instance = Create();
                }
            
                return _instance;
            }
        }
    
        protected static bool Quitting;

        protected static T Create()
        {
            var obj = new GameObject(typeof(T).ToString());
            // DontDestroyOnLoad(obj);
            return obj.AddComponent<T>();
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
            }
        
            if(_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            DontDestroyOnLoad(gameObject);
        }
    
        protected void OnDestroy()
        {
            if (_instance == this)
            {
                Quitting = true;
            }
        }
    }
}