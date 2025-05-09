using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileList : MonoBehaviour
{
    public Transform profileHolder;
    public GameObject profileUIBoxPrefab;

    private void Start()
    {
        var index = ProfileStorage.GetProfileIndex();

        foreach (var profileName in index.ProfileFileNames)
        {
            var go = Instantiate(this.profileUIBoxPrefab);
            var uibox = go.GetComponent<ProfileBoxUI>();

            ProfileStorage.LoadProfile(profileName);
            uibox.nameLabel.text = ProfileStorage.s_currentProfile.name;

            uibox.loadButton.onClick.AddListener(() => {                
                ProfileStorage.LoadProfile(profileName);

                if (MapController.Instance != null)
                {
                    MapController.Instance.UnlockLevels();
                }
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.PlayMusic("ClaroPacifico");
                }
                if (ScenesManager.Instance != null)
                {
                    ScenesManager.Instance.LoadScene("ClaroPacifico", "CrossFade");
                }
            });

            uibox.deleteButton.onClick.AddListener(() => {
                ProfileStorage.DeleteProfile(profileName);
                Destroy(go);
            });

            go.transform.SetParent(this.profileHolder, false);
        }
    }
}
