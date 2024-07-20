using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindFirstObjectByType(typeof(T));

                Debug.Log("An instance of " + typeof(T) + " created.");
            }
            if (_instance == null)
            {
                Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }
}
