using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void ReturnLago()
    {
        MapController.Instance.ClosePanel();
    }

    public void LoadLago()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("ClaroPacifico");
        ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");
    }

    public void LoadLevel1()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level1");
        ScenesManager.Instance.LoadScene("Level1", "CrossFade");
    }

    public void LoadLevel2()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level2");
        ScenesManager.Instance.LoadScene("Level2", "CrossFade");
    }
    public void LoadLevel3()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level3");
        ScenesManager.Instance.LoadScene("Level3", "CrossFade");
    }
    public void LoadLevel4()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level4");
        ScenesManager.Instance.LoadScene("Level4", "CrossFade");
    }

    public void LoadMainMenu()
    {
        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("MainMenu");
        ScenesManager.Instance.LoadScene("MainMenu", "CrossFade");
    }
}
