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
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; // movement input
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize(); // takes direction and keeps direction the same (for this scenario), so it's consistent
        moveDirection.y = 0;
        // need to edit so speed is ewuals to how much the joystic(ig for controller only) is moving -- aka an en llio walk, else halfway run, else full sprint?
        // idea not be able to sprint all the tume, add like a "stamina bar"

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
        Vector3 movementVelocity = moveDirection;
        movementVelocity.y = playerRB.velocity.y; // keep vertical motion
        // playerRigidBody.velocity = movementVelocity; TEST ISSUE WITH FALL ANIM AND FALL PHYSICS
        playerRB.velocity = new Vector3(movementVelocity.x, playerRB.velocity.y,   // keep gravity momentum
        movementVelocity.z);

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
    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += raycastHeightOffset;

        bool wasGrounded = isGrounded;

        // 1. Ground check
        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            isGrounded = true;
            inAirTimer = 0f;
        }
        else
        {
            isGrounded = false;
            inAirTimer += Time.deltaTime;
        }

        // 2. Just started falling this frame (but not if we jump!)
        if (wasGrounded && !isGrounded && !isJumping)
        {
            animatorManager.PlayTargetAnimation("Falling", false); // NOT interacting
        }
        // 3. Just landed this frame
        else if (!wasGrounded && isGrounded)
        {
            animatorManager.PlayTargetAnimation("Land", false); // NOT interacting
        }

        // 4. OPTIONAL: slightly stronger gravity while in air
        if (!isGrounded)
        {
            playerRB.AddForce(Vector3.down * fallingVelocity * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    //private void HandleFallingAndLanding()
    //{
    //    RaycastHit hit;
    //    Vector3 rayCastOrigin = transform.position;
    //    rayCastOrigin.y = rayCastOrigin.y + raycastHeightOffset;

    //    bool wasGrounded = isGrounded;

    //    // ✅ Proper ground check with maxDistance + layer
    //    if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
    //    {
    //        isGrounded = true;
    //        inAirTimer = 0;

    //        if (!wasGrounded && !playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Land", true);
    //        }
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //        inAirTimer += Time.deltaTime;

    //        // We just left the ground → start falling anim
    //        if (wasGrounded && !playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Falling", true);
    //        }

    //        // ✅ Only apply downward force (no forward)
    //        playerRigidBody.AddForce(
    //            Vector3.down * fallingVelocity * inAirTimer,
    //            ForceMode.Acceleration
    //        );
    //    }
    //}
    //private void HandleFallingAndLanding()
    //{
    //    RaycastHit hit;
    //    Vector3 rayCastOrigin = transform.position; // at the feet of the player ?
    //    rayCastOrigin.y = rayCastOrigin.y + raycastHeightOffset;

    //    if (!isGrounded)
    //    {
    //        if (!playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Falling", true);
    //        }

    //        inAirTimer = inAirTimer + Time.deltaTime;
    //        playerRigidBody.AddForce(transform.forward * leapingVelocity);
    //        playerRigidBody.AddForce(Vector3.down * fallingVelocity * inAirTimer); // added in air timer so the longer it's in the air, the faster it falls
    //    }

    //    if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
    //    {
    //        if (!isGrounded && !playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Land", true);
    //        }

    //        inAirTimer = 0; // to reset the timer
    //        isGrounded = true;
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //    }
    //}

    //private void HandleFallingLanding()
    //{
    //    RaycastHit hit;
    //    Vector3 rayCastOrigin = transform.position;
    //    rayCastOrigin.y += raycastHeightOffset;

    //    bool wasGrounded = isGrounded;

    //    // Check for ground just below the player
    //    if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.6f, groundLayer))
    //    {
    //        isGrounded = true;
    //        inAirTimer = 0f;

    //        // Just landed this frame
    //        if (!wasGrounded && playerRigitBody.velocity.y < -0.1f)
    //        {
    //            animatorManager.PlayTargetAnimation("Land", false); // NOT interacting
    //        }
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //        inAirTimer += Time.deltaTime;

    //        // Just left the ground and are moving down → start fall anim
    //        if (wasGrounded && playerRigitBody.velocity.y < -0.1f)
    //        {
    //            animatorManager.PlayTargetAnimation("Fall", false); // NOT interacting
    //        }

    //        // (Optional) extra gravity if you want snappier fall:
    //        // playerRigitBody.AddForce(Vector3.down * fallingSpeed * Time.deltaTime, ForceMode.Acceleration);
    //    }
    //}

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
