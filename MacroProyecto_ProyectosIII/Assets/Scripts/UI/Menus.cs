using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Invoke(nameof(PlayMusic), 0.1f);
        }

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

    public void GoToNewGame()
    {
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
        MapController.Instance.UnlockLevels();

        MusicManager.Instance.PlayMusic("ClaroPacifico");
        ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
