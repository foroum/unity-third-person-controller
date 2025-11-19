using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigitBody;

    public float movementSpeed = 7;
    public float rotationSpeed = 15;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigitBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; // movement input
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize(); // takes direction and keeps direction the same (for this scenario), so it's consistent
        moveDirection.y = 0;
        moveDirection = moveDirection * movementSpeed;

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
