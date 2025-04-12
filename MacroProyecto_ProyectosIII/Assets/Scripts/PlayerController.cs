using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Pausar juego
    public GameObject pausePanel;
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;
    private Animator capibaraAnimator;
    private SpriteRenderer capibaraSprites;

    public bool isPaused = false;

    // Movimiento jugador
    public Rigidbody rb;
    public float moveSpeed, jumpForce;

    private Vector2 moveInput;

    public LayerMask ground;
    public Transform groundPoint;
    private bool isGrounded;

    private InputAction moveAction;
    private InputAction jumpAction;

    // Animaciones jugador

    // Datos jugador
    public int health;
    public int score;

    void Start()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        capibaraAnimator = GetComponent<Animator>();
        capibaraSprites = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            PauseGame();
        }

        Movement();
        Jump();

    }

    private void Movement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // Activar o desactivar animación dependiendo si hay movimiento
        bool isMoving = move != Vector3.zero;
        capibaraAnimator.SetBool("Move", isMoving);

        isGrounded = Physics.Raycast(groundPoint.position, Vector3.down, 0.3f, ground);
        if (!capibaraSprites.flipX && moveInput.x < 0) capibaraSprites.flipX = true;
        else if (capibaraSprites.flipX && moveInput.x > 0) capibaraSprites.flipX = false;
    }
    private void Jump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.linearVelocity += new Vector3(0f, jumpForce, 0f);
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            playerInput.enabled = false;
            cinemachineBrain.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            FindFirstObjectByType<PlayerController>().isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            playerInput.enabled = true;
            cinemachineBrain.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;

            FindFirstObjectByType<PlayerController>().isPaused = false;
        }
    }
}
