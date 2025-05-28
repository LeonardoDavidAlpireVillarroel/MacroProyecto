using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitShoot : MonoBehaviour
{
    public GameObject fruitPrefab;
    public float shootSpeed = 10f;
    public float fruitLifetime = 5f;
    public GameObject arrowInstance;
    public Transform spawnPoint;

    private float unpausedCooldownTime = 0.1f;
    private float unpausedTimer = 0f;

    public bool isAiming { get; private set; }
    public bool hasShot { get; set; }

    private Vector3 currentAimDirection = Vector3.forward;

    [SerializeField] private PlayerController playerController;

    [Header("UI sin munición")]
    public CanvasGroup noAmmoPanel;
    public float noAmmoFadeDuration = 0.5f;
    public float noAmmoDisplayTime = 2f;

    void Start()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(false);
        }

        playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        unpausedTimer = unpausedCooldownTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        SnakeBossAI boss = other.GetComponent<SnakeBossAI>();
        if (boss != null)
        {
            boss.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (arrowInstance == null) return;

        if (unpausedTimer > 0f)
        {
            unpausedTimer -= Time.unscaledDeltaTime;
            return;
        }

        isAiming = playerController.aimAction.ReadValue<float>() > 0.5f;

        if (isAiming)
        {
            if (!arrowInstance.activeSelf)
                arrowInstance.SetActive(true);

            UpdateArrowDirection();

            if (playerController.shootAction.WasPressedThisFrame())
            {
                ShootFruit();
            }
        }

        else
        {
            if (arrowInstance.activeSelf)
                arrowInstance.SetActive(false);
        }
    }


    void UpdateArrowDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0)); 

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            hitPoint.y = transform.position.y; 

            Vector3 direction = (hitPoint - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                arrowInstance.transform.forward = direction;
                currentAimDirection = direction;
            }
        }
    }


    public Vector3 GetAimDirection()
    {
        return currentAimDirection;
    }

    void ShootFruit()
    {
        bool tieneMunicion = false;
        int indexMunicion = -1;
        int idMunicion = -1;

        for (int i = 0; i < Inventory.Instance.inventory.Count; i++)
        {
            var item = Inventory.Instance.inventory[i];
            if (item.id == -1 || item.cantidadItems <= 0) continue;

            var itemData = Inventory.Instance.data.ObjectsDataBase[item.id];

            if (item.id == 3 || itemData.clase == ItemsDataBase.Clase.Municion)
            {
                tieneMunicion = true;
                indexMunicion = i;
                idMunicion = item.id;
                break;
            }
        }

        if (!tieneMunicion)
        {
            StartCoroutine(ShowNoAmmoPanel());
            return;
        }

        GameObject newFruit = Instantiate(fruitPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = newFruit.GetComponent<Rigidbody>();
        if (rb == null) rb = newFruit.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearVelocity = currentAimDirection * shootSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider fruitCollider = newFruit.GetComponent<Collider>();
            Collider playerCollider = player.GetComponent<Collider>();

            if (fruitCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(fruitCollider, playerCollider);
            }
        }

        Destroy(newFruit, fruitLifetime);
        hasShot = true;

        var currentItem = Inventory.Instance.inventory[indexMunicion];
        int nuevaCantidad = currentItem.cantidadItems - 1;

        if (nuevaCantidad <= 0)
        {
            Inventory.Instance.inventory[indexMunicion] = new ObjectInventoryID(-1, 0);
        }
        else
        {
            Inventory.Instance.inventory[indexMunicion] = new ObjectInventoryID(idMunicion, nuevaCantidad);
        }

        Inventory.Instance.InventoryUpdate();
        GameManager.Instance.SaveGame();
    }

    IEnumerator ShowNoAmmoPanel()
    {
        if (noAmmoPanel == null) yield break;

        noAmmoPanel.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < noAmmoFadeDuration)
        {
            elapsed += Time.deltaTime;
            noAmmoPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / noAmmoFadeDuration);
            yield return null;
        }

        noAmmoPanel.alpha = 1f;

        yield return new WaitForSeconds(noAmmoDisplayTime);

        elapsed = 0f;
        while (elapsed < noAmmoFadeDuration)
        {
            elapsed += Time.deltaTime;
            noAmmoPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / noAmmoFadeDuration);
            yield return null;
        }

        noAmmoPanel.alpha = 0f;
        noAmmoPanel.gameObject.SetActive(false);
    }
}
