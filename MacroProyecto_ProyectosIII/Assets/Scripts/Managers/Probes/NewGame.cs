using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NewGame : MonoBehaviour
{
    public TMP_InputField profileInput;

    public void Generate()
    {
        string profileName = this.profileInput.text;
        ProfileStorage.CreateNewGame(profileName);

        MusicManager.Instance.PlayMusic("PacificLake");
        ScenesManager.Instance.LoadScene("PacificLake", "CrossFade");
    }
}
