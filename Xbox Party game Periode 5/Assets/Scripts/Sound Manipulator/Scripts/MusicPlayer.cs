using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer m_instance;

    private void OnEnable()
    {
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (m_instance) { DestroyImmediate(gameObject); }
    }
}