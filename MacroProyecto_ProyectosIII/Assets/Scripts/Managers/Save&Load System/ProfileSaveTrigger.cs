using UnityEngine;

public class ProfileSaveTrigger : MonoBehaviour
{
    public int levelToUnlock = 2;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;

        if (other.CompareTag("Player"))
        {
            var level = LevelController.Instance;

            if (level != null && !level.HasCollectedAllRequiredItems())
            {
                return;
            }

            alreadyTriggered = true;

            var profile = ProfileStorage.s_currentProfile;

            if (profile != null && profile.unlockedLevelCount < levelToUnlock)
            {
                profile.unlockedLevelCount = levelToUnlock;
            }

            ProfileStorage.StorePlayerProfile(other.gameObject, GameManager.Instance);
        }
    }
}
