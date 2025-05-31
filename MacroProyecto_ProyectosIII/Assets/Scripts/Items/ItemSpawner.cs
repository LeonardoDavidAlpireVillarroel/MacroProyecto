using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject itemPrefab;
    public float intervaloSpawn = 3f;
    public Transform puntoDeSpawn;

    [Header("Efecto Pop-Up")]
    public float popupScale = 1.5f;
    public float popupDuration = 0.3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnItem), 0f, intervaloSpawn);
    }

    void SpawnItem()
    {
        Vector3 posicion = puntoDeSpawn != null ? puntoDeSpawn.position : transform.position;

        GameObject nuevoItem = Instantiate(itemPrefab, posicion, Quaternion.identity);
        StartCoroutine(PopUpEffect(nuevoItem.transform));
    }

    System.Collections.IEnumerator PopUpEffect(Transform itemTransform)
    {
        Vector3 originalScale = itemTransform.localScale;
        Vector3 targetScale = originalScale * popupScale;

        float elapsed = 0f;

        while (elapsed < popupDuration)
        {
            float t = elapsed / popupDuration;
            itemTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        itemTransform.localScale = originalScale;
    }
}
