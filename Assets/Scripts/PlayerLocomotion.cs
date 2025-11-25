using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRB;

    [Header("Falling Setting")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastHeightOffset = 0.5f;
    public LayerMask groundLayer;
    public float maxDistance;

    [Header("Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Settings")]
    public float walkSpeed = 2;
    public float runSpeed = 5;
    public float sprintSpeed = 7; // added sprinting and walking
                                     // in order to blend the animations better
    public float rotationSpeed = 15;

    [Header("Jump Settings")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;

    //[Header("Ground Snapping")]
    //public float maxStepHeight = 0.5f;      // how high a step we can snap to
    //public float groundSnapSpeed = 20f;     // how fast we blend to ground when moving

    // adding double jump theory
    //public int maxJumps = 2;
    //private int jumpCount = 0;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRB = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
    }

    private bool OnSlope()
    {
        // start ray a tiny bit above the player position so it doesn't start inside the ground
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out slopeHit, 1.5f, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // angle > 0 so we don't treat flat ground as a slope
            return angle > 0f && angle <= maxSlopeAngle;
        }

        return false;
    }

    private void HandleMovement()
    {
        // base moveDirection from camera
        moveDirection = cameraObject.forward * inputManager.verticalInput; // movement input
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize(); // takes direction and keeps direction the same (for this scenario), so it's consistent
        moveDirection.y = 0;
        // need to edit so speed is ewuals to how much the joystic(ig for controller only) is moving -- aka an en llio walk, else halfway run, else full sprint?
        // idea not be able to sprint all the tume, add like a "stamina bar"

        // speed type
        if (isSprinting) // added boolean to support the "stamina bar" idea
        {
            moveDirection = moveDirection * sprintSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkSpeed;
            }
        }


        //Vector3 movementVelocity = moveDirection;
        //playerRigitBody.velocity = movementVelocity;

        // bellow what i have been using before adding idea for slop movement
        /* Vector3 movementVelocity = moveDirection;
        movementVelocity.y = playerRB.velocity.y; // keep vertical motion
        playerRB.velocity = new Vector3(movementVelocity.x, playerRB.velocity.y,   // keep gravity momentum
        movementVelocity.z); */

        // NEW FOR SLOPE HANDLING TEST project onto slope plane if grounded & on slope
        Vector3 finalMove = moveDirection;

        if (isGrounded && OnSlope())
        {
            // project our desired movement onto the slope surface
            finalMove = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;

            // keep our speed magnitude
            finalMove *= moveDirection.magnitude;
        }

        // 4. apply velocity (only x/z, keep existing y for jump/fall)
        Vector3 movementVelocity = finalMove;
        movementVelocity.y = playerRB.velocity.y;

        playerRB.velocity = new Vector3(
            movementVelocity.x,
            playerRB.velocity.y,   // vertical handled by jump/fall
            movementVelocity.z
        );


    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        // targetDirection.Normalize();
        targetDirection.y = 0f; // just like the above in theory so far
        // prevents LookRotation on zero vector
        if (targetDirection.sqrMagnitude < 0.001f)
        {
            return;
        }
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection); // we're gonna look towards our target direction basicially
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // reminder for myself: use again deltaTime when we want movement or
                                                                                                                          // any other constant variable change to the same speed,
                                                                                                                          // no matter the frame rate (cuz frame rate will always differ)

        transform.rotation = playerRotation;
    }

    // debugging
    //private void HandleFallingAndLanding()
    //{
    //    RaycastHit hit;
    //    Vector3 rayCastOrigin = transform.position;
    //    rayCastOrigin.y += raycastHeightOffset;

    //    bool wasGrounded = isGrounded;

    //    // 1. Ground check
    //    if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
    //    {
    //        isGrounded = true;
    //        inAirTimer = 0f;
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //        inAirTimer += Time.deltaTime;
    //    }

    //    // 2. Just started falling this frame (but not if we jump!)
    //    if (wasGrounded && !isGrounded && !isJumping)
    //    {
    //        animatorManager.PlayTargetAnimation("Falling", false); // NOT interacting
    //    }
    //    // 3. Just landed this frame
    //    else if (!wasGrounded && isGrounded)
    //    {
    //        animatorManager.PlayTargetAnimation("Land", false); // NOT interacting
    //    }

    //    // 4. OPTIONAL: slightly stronger gravity while in air
    //    if (!isGrounded)
    //    {
    //        playerRB.AddForce(Vector3.down * fallingVelocity * Time.deltaTime, ForceMode.Acceleration);
    //    }
    //}
    // best one yet ^^
    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        // Vector3 targetPos;
        rayCastOrigin.y += raycastHeightOffset;
        //targetPos = transform.position;

        bool wasGrounded = isGrounded;

        // 1. Ground check
        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            Vector3 rayCastHitPoint = hit.point;
            // targetPos.y = rayCastHitPoint.y;
            isGrounded = true;
            inAirTimer = 0f;

            // Just landed this frame
            if (!wasGrounded)
            {
                // we finished a jump or a fall
                isJumping = false;
                animatorManager.animator.SetBool("isJumping", false);

                animatorManager.PlayTargetAnimation("Land", false);
            }
        }
        else
        {
            isGrounded = false;
            inAirTimer += Time.deltaTime;

            // Walked off a ledge (not jumping) -> start falling immediately
            if (wasGrounded && !isJumping)
            {
                animatorManager.PlayTargetAnimation("Falling", false);
            }

            // If we *are* jumping, swap to Falling once we start going down
            if (isJumping && playerRB.velocity.y <= 0f)
            {
                isJumping = false;
                animatorManager.animator.SetBool("isJumping", false);
                animatorManager.PlayTargetAnimation("Falling", false);
            }

            // Extra gravity
            playerRB.AddForce(Vector3.down * fallingVelocity * Time.deltaTime, ForceMode.Acceleration);
        }

        //if (isGrounded && !isJumping)
        //{
        //    if (playerManager.isInteracting || inputManager.moveAmount > 0)
        //    {
        //        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / 0.1f);
        //    }
        //    else
        //    {
        //        transform.position = targetPos;
        //    }
        //}
    }


    public void HandleJump()
    {
        if (!isGrounded) return;  // can't jump in air

        isJumping = true;
        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump", false);

        float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 playerVelocity = moveDirection;
        playerVelocity.y = jumpingVelocity;
        playerRB.velocity = playerVelocity;
    }
}
