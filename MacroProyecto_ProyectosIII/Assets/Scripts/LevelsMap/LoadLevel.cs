using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void ReintentarNivelActual()
    {
        Time.timeScale = 1f;
        string nombreNivel = SceneManager.GetActiveScene().name;
        ScenesManager.Instance.LoadScene(nombreNivel, "CrossFade");
        MusicManager.Instance.PlayMusic(nombreNivel);
    }

    public void ReturnLago()
    {
        GameManager.Instance.ResumeGame();
    }

    public void LoadLago()
    {
        GameManager.Instance.RestoreBackupState();

        if (MapController.Instance.unlockLevel < 2)
        {
            MapController.Instance.unlockLevel = 2;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("ClaroPacifico");
        ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");
    }

    public void LoadLevel1()
    {
        GameManager.Instance.BackupCurrentState();

        if (MapController.Instance.unlockLevel < 3)
        {
            MapController.Instance.unlockLevel = 3;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level1");
        ScenesManager.Instance.LoadScene("Level1", "CrossFade");
    }

    public void LoadLevel2()
    {
        GameManager.Instance.BackupCurrentState();

        if (MapController.Instance.unlockLevel < 4)
        {
            MapController.Instance.unlockLevel = 4;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level2");
        ScenesManager.Instance.LoadScene("Level2", "CrossFade");
    }
    public void LoadLevel3()
    {
        GameManager.Instance.BackupCurrentState();

        if (MapController.Instance.unlockLevel < 5)
        {
            MapController.Instance.unlockLevel = 5;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level3");
        ScenesManager.Instance.LoadScene("Level3", "CrossFade");
    }
    public void LoadLevel4()
    {
        GameManager.Instance.BackupCurrentState();

        if (MapController.Instance.unlockLevel < 5)
        {
            MapController.Instance.unlockLevel = 5;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Level4");
        ScenesManager.Instance.LoadScene("Level4", "CrossFade");
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.BackupCurrentState();

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("MainMenu");
        ScenesManager.Instance.LoadScene("MainMenu", "CrossFade");
    }

    public void LoadCredits()
    {
        GameManager.Instance.RestoreBackupState();

        if (MapController.Instance.unlockLevel < 3)
        {
            MapController.Instance.unlockLevel = 3;
        }

        MapController.Instance.UnlockLevels();
        Time.timeScale = 1;
        MusicManager.Instance.PlayMusic("Credits");
        ScenesManager.Instance.LoadScene("Credits", "CrossFade");
    }
}
