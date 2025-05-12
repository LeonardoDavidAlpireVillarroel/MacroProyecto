using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class Dash : MonoBehaviour
{
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction dashAction;
    private PlayerController playerController;

    private Animator capibaraAnimator;

    private bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        capibaraAnimator = GetComponent<Animator>();
        dashAction = playerInput.actions["Dash"];
    }

    private void Update()
    {
        if (dashAction.WasPerformedThisFrame() && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        playerController.isDashing = true;
        capibaraAnimator.SetBool("IsDashing", true);

        Vector3 direction = new Vector3(playerController.moveInput.x, 0, playerController.moveInput.y).normalized;

        if (direction == Vector3.zero)
        {
            direction = new Vector3(playerController.lastMoveDirection.x, 0, playerController.lastMoveDirection.y).normalized;
        }

        Vector3 originalVelocity = rb.linearVelocity;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            rb.linearVelocity = direction * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector3(0, originalVelocity.y, 0);
        playerController.isDashing = false;
        capibaraAnimator.SetBool("IsDashing", false);
    }
}
