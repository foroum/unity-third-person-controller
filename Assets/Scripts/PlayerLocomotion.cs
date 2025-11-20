using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigitBody;

    public float inAirTimer;
    public float leapingSpeed;
    public float fallingSpeed;
    public float raycastHeightOffset = 0.5f;
    public LayerMask groundLayer;
    public float maxDistance = 1;


    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    public float walkSpeed = 2;
    public float runSpeed = 5;
    public float sprintSpeed = 7; // added sprinting and walking
                                     // in order to blend the animations better
    public float rotationSpeed = 15;

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
        playerRigitBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        // HandleFallingLandingSimple();
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;
        // debugging

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (isJumping)
        {
            return;
        }
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
        movementVelocity.y = playerRigitBody.velocity.y; // keep vertical motion
        playerRigitBody.velocity = movementVelocity;

    }

    private void HandleRotation()
    {
        if (isJumping)
        {
            return;
        }
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


    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + raycastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Fall", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigitBody.AddForce(transform.forward * leapingSpeed);
            playerRigitBody.AddForce(Vector3.down * fallingSpeed * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, groundLayer)) // added 0.6f
        // if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting) // was (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            inAirTimer = 0;
            isGrounded = true;
            // jumpCount = 0; // reseting counter when char touches ground
            // playerManager.isInteracting = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJump()
    {
        if (isGrounded)
        {
            //// first jump must start from the ground
            //if (!isGrounded && jumpCount == 0)
            //    return;

            //// already used all jumps?
            //if (jumpCount >= maxJumps)
            //    return;

            //jumpCount++;

            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigitBody.velocity = playerVelocity;

            // isGrounded = false; // added for double j
        }
    }

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


    //private void HandleFallingLandingSimple()
    //{
    //    RaycastHit hit;
    //    Vector3 raycastOrigin = transform.position;
    //    raycastOrigin.y += raycastOffset;

    //    bool wasGrounded = isGrounded;

    //    // Check for ground just below us
    //    if (Physics.SphereCast(raycastOrigin, 0.2f, Vector3.down, out hit, 0.6f, ground))
    //    {
    //        isGrounded = true;

    //        // Just landed this frame
    //        if (!wasGrounded && !playerManager.isInteracting && playerRigitBody.velocity.y < -0.1f)
    //        {
    //            animatorManager.PlayTargetAnimation("Land", false);
    //        }
    //    }
    //    else
    //    {
    //        isGrounded = false;

    //        // We are falling (going down)
    //        if (!playerManager.isInteracting && playerRigitBody.velocity.y < -0.1f)
    //        {
    //            animatorManager.PlayTargetAnimation("Fall", false);
    //        }
    //    }
    //}

}
