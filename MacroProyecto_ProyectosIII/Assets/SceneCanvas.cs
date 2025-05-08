using UnityEngine;

public class SceneCanvas : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
