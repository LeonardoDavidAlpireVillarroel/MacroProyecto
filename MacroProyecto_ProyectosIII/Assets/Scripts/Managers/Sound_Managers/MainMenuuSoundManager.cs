using UnityEngine;

public class MainMenuuSoundManager : MonoBehaviour
{
    void Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
        Destroy(gameObject);
    }
}
