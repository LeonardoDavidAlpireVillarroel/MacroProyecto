using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;

    private SpriteRenderer capibaraSprites;
    private FruitShoot fruitShoot;

    [Header("Movement Settings")]
    public float runSpeed = 5f;
    private bool isMoving = false;

    public float jumpSpeed = 5f;
    public float doubleJumpSpeed = 4f;
    private bool canDoubleJump;

    public bool isDashing = false;

    [Header("References")]
    public Vector2 moveInput { get; private set; }
    public Vector2 lastMoveDirection { get; private set; }

    private Animator capibaraAnimator;
    private Rigidbody rb;
    private bool jumpPressed;

    //Input Actions
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] private InputAction jumpAction;
    [HideInInspector] public InputAction interactAction;

    // Shoot/Aim Inputs
    [HideInInspector] public InputAction aimAction;
    [HideInInspector] public InputAction shootAction;
    [HideInInspector] public InputAction pointerPositionAction;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Impulsos por gameObjects")]
    private bool isBeingPushed = false;
    private float pushTimer = 0f;
    private float pushDuration = 0.5f;

    [Header("Water Settings")]
    public LayerMask waterLayer;
    public float waterFloatForce = 5f;
    private bool isInWater;    

    void Awake()
    {
        if (Instance  == null)
        {
            Instance = this;
        }

        rb = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();
        playerInput.defaultActionMap = "Player";

        capibaraSprites = GetComponentInChildren<SpriteRenderer>();
        capibaraAnimator = GetComponentInChildren<Animator>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        interactAction = playerInput.actions["Interact"];

        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        pointerPositionAction = playerInput.actions["PointerPosition"];
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (jumpAction.triggered)
            jumpPressed = true;

        if (Input.GetMouseButtonDown(0))
        {
            capibaraAnimator.SetBool("IsShooting", true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            capibaraAnimator.SetBool("IsShooting", false);
        }
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

    public void MovePlayer()
    {
        if (isDashing) return;

        if (isBeingPushed)
        {
            pushTimer -= Time.fixedDeltaTime;
            if (pushTimer <= 0)
            {
                isBeingPushed = false;
            }
            else
            {
                // Mientras está siendo empujado, no modificar velocidad horizontal
                return;
            }
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            isMoving = true;
            lastMoveDirection = moveInput;

            Vector3 targetVelocity = moveDirection * runSpeed;
            Vector3 currentVelocity = rb.linearVelocity;

            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

            if (targetVelocity.magnitude > horizontalVelocity.magnitude)
            {
                rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, currentVelocity.z);
            }
        }
        else
        {
            isMoving = false;
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
            if (horizontalVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = new Vector3(0, currentVelocity.y, 0);
            }
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
        if (isDashing) return;

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 movement = moveDirection * (runSpeed * 0.5f);
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            isMoving = true;
            lastMoveDirection = moveInput;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                capibaraAnimator.SetFloat("LookDirection", moveInput.x > 0 ? 0f : 1f);
            }
            else
            {
                capibaraAnimator.SetFloat("LookDirection", moveInput.y > 0 ? 2f : 3f);
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            isMoving = false;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                capibaraAnimator.SetFloat("LookDirection", moveInput.x > 0 ? 0f : 1f);
            }
            else
            {
                capibaraAnimator.SetFloat("LookDirection", moveInput.y > 0 ? 2f : 3f);
            }
        }

        UpdateVisualDirection();

        if (!jumpPressed)
        {
            rb.AddForce(Vector3.up * waterFloatForce, ForceMode.Acceleration);
        }
        capibaraAnimator.SetBool("IsSwimming", true);
        capibaraAnimator.SetBool("IsRun", isMoving);
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

    public void ApplyPush(Vector3 pushForce)
    {
        rb.linearVelocity = pushForce;
        isBeingPushed = true;
        pushTimer = pushDuration;
    }
}
