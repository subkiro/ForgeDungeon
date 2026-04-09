using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static readonly T s_Instance = new T();
    public static T Instance
    {
        get
        {
            return s_Instance;
        }
    }
}

public class SingletonObj<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }


    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
          //  DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


public class SingletonObjOpposite<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {          
            return instance;
        }
    }


    public virtual void Awake()
    {
           instance = this as T;   
            
    }
}

public class SingletonObjTraditional<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }


    public virtual void Awake()
    {
        if (instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this as T; 
        DontDestroyOnLoad(gameObject);

    }
}