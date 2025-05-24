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
        GameObject newFruit = Instantiate(fruitPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = newFruit.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = newFruit.AddComponent<Rigidbody>();
        }

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
    }

}
