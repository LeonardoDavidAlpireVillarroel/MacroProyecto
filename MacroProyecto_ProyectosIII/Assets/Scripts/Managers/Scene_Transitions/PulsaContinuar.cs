using UnityEngine;
using UnityEngine.InputSystem;

public class PulsaContinuar : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame == true || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
        {
            MusicManager.Instance.PlayMusic("MainMenu", 0.5f);
            ScenesManager.Instance.LoadScene("MainMenu", "CrossFade");
        }
    }
}
