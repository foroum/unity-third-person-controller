using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigitBody;

    public bool isSprinting;
    public float walkSpeed = 2;
    public float runSpeed = 5;
    public float sprintSpeed = 7; // added sprinting and walking
                                     // in order to blend the animations better
    public float rotationSpeed = 15;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigitBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
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


        Vector3 movementVelocity = moveDirection;
        playerRigitBody.velocity = movementVelocity;
    }

    public void HandleRotation()
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
}
