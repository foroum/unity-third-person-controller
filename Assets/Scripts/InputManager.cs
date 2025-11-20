using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   // <- new Input System

public class InputManager : MonoBehaviour
{
    // reference to the generated input actions class (from our .inputactions asset)
    private PlayerControls playerControls;

    // reference to PlayerLocomotion
    PlayerLocomotion locomotion;

    // reference to your AnimatorManager (for updating blend tree)
    private AnimatorManager animatorManager;

    public Vector2 movementInput;      // raw input (x,y) from WASD / stick
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;           // 0..1 (how strong the movement is)
    public bool sprintButtonPressed;
    public bool jumpButtonPressed;

    public Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;

    private void Awake()
    {
        // If you have an AnimatorManager on the same GameObject:
        animatorManager = GetComponent<AnimatorManager>();
        locomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Subscribe to Movement action
            playerControls.PlayerMovement.Movement.performed += ctx =>
                movementInput = ctx.ReadValue<Vector2>();

            playerControls.PlayerMovement.Movement.canceled += ctx =>
                movementInput = Vector2.zero;

            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += i => sprintButtonPressed = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintButtonPressed = false;

            playerControls.PlayerActions.Jump.performed += i => jumpButtonPressed = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpButtonPressed = false;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    // Call this from your PlayerManager / Update loop
    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintInput();
        HandleJumpInput();
        //HandleActionInput
    }

    private void HandleMovementInput()
    {
        // Convert Vector2 into separate axes
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // for camera
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        // How strong is the movement? (used for animations)
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        // Update animator, if you have one
        if (animatorManager != null)
        {
            // Example: first param could be forward, second strafe
            animatorManager.UpdateAnimatorValues(0, moveAmount, sprintButtonPressed);
        }
    }

    private void HandleSprintInput()
    {
        if (moveAmount > 0.5f && sprintButtonPressed)
        {
            locomotion.isSprinting = true;
        }
        else
        {
            locomotion.isSprinting = false;
        }
    }
    private void HandleJumpInput()
    {
        if (jumpButtonPressed)
        {
            jumpButtonPressed = false;
            locomotion.HandleJump();
        }
    }
}
