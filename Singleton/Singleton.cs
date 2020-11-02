using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    var singleton = new GameObject("[SINGLETON]" + typeof(T));
                    _instance = singleton.AddComponent<T>();
                    if(_instance.GetComponent<IDestroyOnLoad>() != null) DontDestroyOnLoad(singleton);
                }
            }

            return _instance;
        }
    }
}