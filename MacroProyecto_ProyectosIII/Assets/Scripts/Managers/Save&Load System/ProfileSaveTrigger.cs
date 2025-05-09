using UnityEngine;

public class ProfileSaveTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        ProfileStorage.StorePlayerProfile(other.gameObject, gameManager);
    }
}
