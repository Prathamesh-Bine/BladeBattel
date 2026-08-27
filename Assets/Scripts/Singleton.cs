using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindFirstObjectByType<T>();
            }

            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }
    }
}