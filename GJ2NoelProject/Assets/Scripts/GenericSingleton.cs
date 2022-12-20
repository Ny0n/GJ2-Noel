using UnityEngine;

public abstract class GenericSingleton<T> : MonoBehaviour where T : GenericSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // if instance is already set
            if (_instance != null)
            {
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
            
            // find the singleton
            var objs = FindObjectsOfType<T>();
            
            // if found, set the instance
            if (objs.Length > 0)
            {
                var instance = objs[0];
                _instance = instance;
                DontDestroyOnLoad(instance.gameObject);
            }
            else // else create a new GameObject then set it to the instance
            {
                var go = new GameObject
                {
                    name = typeof(T).Name
                };
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }
}