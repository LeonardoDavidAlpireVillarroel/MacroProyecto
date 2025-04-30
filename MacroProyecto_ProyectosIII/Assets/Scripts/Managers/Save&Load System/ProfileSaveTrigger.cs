using UnityEngine;

public class ProfileSaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ProfileStorage.StorePlayerProfile(other.gameObject);
    }
}
