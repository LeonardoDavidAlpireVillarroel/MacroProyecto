using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    void Start()
    {
        if (musicVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.value = savedVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            sfxVolumeSlider.value = savedVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    public void GoToTutorial()
    {
        MusicManager.Instance.PlayMusic("Tutorial");
        ScenesManager.Instance.LoadScene("Tutorial", "CrossFade");
    }

    public void ExitTutorial()
    {
        GameManager.Instance.isTutorialScene = false;
        MusicManager.Instance.PlayMusic("OtherMenus");
        ScenesManager.Instance.LoadScene("NewGame", "CrossFade");
    }

    public void GoToNewGame()
    {
        ProfileStorage.s_currentProfile = null;
        MusicManager.Instance.PlayMusic("OtherMenus");
        ScenesManager.Instance.LoadScene("NewGame", "CrossFade");
    }
    public void GoToSavedGames()
    {
        MusicManager.Instance.PlayMusic("OtherMenus");
        ScenesManager.Instance.LoadScene("LoadSaveGames", "CrossFade");
    }

    public void PlayClaroPacificoScene()
    {
        GameManager.Instance.OnGameOverConfirm();
    }

    public void GoClaroExitGame()
    {
        Time.timeScale = 1;
        if (ScenesManager.Instance != null)
            ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("ClaroPacifico");
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        if (ScenesManager.Instance != null)
            ScenesManager.Instance.LoadScene("MainMenu", "CrossFade");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlaySFX(string soundName)
    {
        SoundManager.Instance?.PlaySound2D(soundName);
    }

    void PlayMusic()
    {
        MusicManager.Instance?.PlayMusic("MainMenu");
    }

    public void SetMusicVolume(float volume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
    }

    public void SetSfxVolume(float volume)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(volume);
        }
    }
}
