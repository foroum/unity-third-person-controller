using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basically this script will be the script used to run all functionalities we create for player (as script name states)
public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    Animator animator;

    public bool isInteracting;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        // cameraManager.FollowTarget();
        cameraManager.HandleAllCameraMovement();
        isInteracting = animator.GetBool("isInteracting");
    }
}
