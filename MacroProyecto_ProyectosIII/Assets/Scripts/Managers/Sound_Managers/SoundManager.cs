using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] 
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SetVolume(savedVolume);
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip !=  null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfx2DSource.volume = volume;

        PlayerPrefs.SetFloat("SoundVolume", volume);
        PlayerPrefs.Save();
    }
}
