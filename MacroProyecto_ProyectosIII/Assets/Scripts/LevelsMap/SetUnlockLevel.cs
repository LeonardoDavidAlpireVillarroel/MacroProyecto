using UnityEngine;
using UnityEngine.SceneManagement;

public class SetUnlockLevel : MonoBehaviour
{
    [Header("Nivel a desbloquear")]
    public int unlockLevel = 3;

    [Header("Win Panel")]
    public GameObject winPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ProfileStorage.s_currentProfile != null && unlockLevel > ProfileStorage.s_currentProfile.unlockedLevelCount)
            {
                ProfileStorage.s_currentProfile.unlockedLevelCount = unlockLevel;

                ProfileStorage.StorePlayerProfile(other.gameObject);
            }

            UnlockLevelData.sharedUnlockLevel = unlockLevel;

            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }
        }
    }
}
