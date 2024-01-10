using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;

    private bool alive = true;

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else
            {
                T[] managers = FindObjectsOfType<T>();
                if (managers != null)
                {
                    if (managers.Length == 1)
                    {
                        instance = managers[0];
                        DontDestroyOnLoad(instance);
                        return instance;
                    }
                    else
                    {
                        if (managers.Length > 1)
                        {
                            Debug.LogError($"Have more that one {typeof(T).Name} in scene. " +
                                            "But this is Singleton! Check project.");
                            for (int i = 0; i < managers.Length; ++i)
                            {
                                T manager = managers[i];
                                Destroy(manager.gameObject);
                            }
                        }
                    }
                }
                //create 
                GameObject go = new GameObject(typeof(T).Name, typeof(T));
                instance = go.GetComponent<T>();
                instance.Initialization();
                DontDestroyOnLoad(instance.gameObject);
                return instance;
            }
        }

        //Can be initialized externally
        set
        {
            instance = value as T;
        }
    }

    /// <summary>
    /// Check flag if need work from OnDestroy or OnApplicationExit
    /// </summary>
    public static bool IsAlive
    {
        get
        {
            if (instance == null)
                return false;
            return instance.alive;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this as T;
            Initialization();
        }
        else
        {
            Debug.LogError($"Have more that one {typeof(T).Name} in scene. " +
                            "But this is Singleton! Check project.");
            DestroyImmediate(this);
        }
    }

    protected void OnDestroy() { alive = false; }

    protected void OnApplicationQuit() { alive = false; }

    protected virtual void Initialization() { }
}