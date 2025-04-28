using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;

    private SpriteRenderer capibaraSprites;

    [Header("Movement Settings")]
    public float runSpeed = 5f;
    private Vector2 lastMoveDirection = new Vector2(1, 0);
    private bool isMoving = false;
    public float jumpSpeed = 5f;
    public float doubleJumpSpeed = 4f;
    private bool canDoubleJump;

    [Header("References")]
    private Animator capibaraAnimator;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpPressed;

    private InputAction moveAction;
    private InputAction jumpAction;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Water Settings")]
    public LayerMask waterLayer;
    public float waterFloatForce = 5f;
    private bool isInWater;

    //Player Stats
    public int health;
    public int score;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        capibaraSprites = GetComponentInChildren<SpriteRenderer>();
        capibaraAnimator = GetComponentInChildren<Animator>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.triggered)
            jumpPressed = true;
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (isInWater)
        {
            SwimPlayer();
        }
        else
        {
            MovePlayer();
        }

        HandleJumpLogic();

        UpdateAnimations();
        jumpPressed = false;
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        isInWater = Physics.CheckSphere(groundCheck.position, groundCheckRadius, waterLayer);
    }

    void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            isMoving = true;
            lastMoveDirection = moveInput; // Actualizamos última dirección

            Vector3 movement = moveDirection * runSpeed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            isMoving = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        UpdateVisualDirection();

        capibaraAnimator.SetBool("IsRun", isMoving);
    }

    void UpdateVisualDirection()
    {
        if (capibaraSprites == null)
            return;

        Vector2 dir = isMoving ? moveInput : lastMoveDirection;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            capibaraSprites.flipX = dir.x < 0;
            capibaraAnimator.SetFloat("LookDirection", dir.x > 0 ? 0f : 1f);
        }
        else
        {
            capibaraSprites.flipX = false;
            capibaraAnimator.SetFloat("LookDirection", dir.y > 0 ? 2f : 3f);
        }
    }

    void HandleJumpLogic()
    {
        if (isGrounded || isInWater)
        {
            if (jumpPressed)
            {
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);
                    canDoubleJump = true;

                    capibaraAnimator.SetBool("IsJump", true);
                    capibaraAnimator.SetBool("DoubleJump", false);
                }
                else if (isInWater)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);
                    capibaraAnimator.SetBool("IsJump", true);
                    capibaraAnimator.SetBool("DoubleJump", false);
                }
            }
        }
        else
        {
            if (jumpPressed && canDoubleJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, doubleJumpSpeed, rb.linearVelocity.z);
                canDoubleJump = false;

                capibaraAnimator.SetBool("DoubleJump", true);
                capibaraAnimator.SetBool("IsJump", false);
            }
        }
    }

    void SwimPlayer()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 movement = moveDirection * runSpeed * 0.5f;

            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            capibaraSprites.flipX = moveInput.x < 0;
            capibaraAnimator.SetBool("IsRun", true);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            capibaraAnimator.SetBool("IsRun", false);
        }

        if (!jumpPressed)
        {
            rb.AddForce(Vector3.up * waterFloatForce, ForceMode.Acceleration);
        }

        capibaraAnimator.SetBool("IsSwimming", true);
        capibaraAnimator.SetBool("IsJump", false);
        capibaraAnimator.SetBool("DoubleJump", false);
        capibaraAnimator.SetBool("Falling", false);
    }

    void UpdateAnimations()
    {
        if (isInWater)
            return;

        float verticalVelocity = rb.linearVelocity.y;

        capibaraAnimator.SetBool("IsSwimming", false);

        if (isGrounded)
        {
            capibaraAnimator.SetBool("IsJump", false);
            capibaraAnimator.SetBool("DoubleJump", false);
            capibaraAnimator.SetBool("Falling", false);
        }
        else
        {
            if (verticalVelocity > 0.1f)
            {
                capibaraAnimator.SetBool("IsJump", true);
                capibaraAnimator.SetBool("DoubleJump", !canDoubleJump);
                capibaraAnimator.SetBool("Falling", false);
            }
            else if (verticalVelocity < -0.1f)
            {
                capibaraAnimator.SetBool("IsJump", false);
                capibaraAnimator.SetBool("DoubleJump", false);
                capibaraAnimator.SetBool("Falling", true);
            }
        }
    }
}
