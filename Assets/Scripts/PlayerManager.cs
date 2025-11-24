using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basically this script will be the script used to run all functionalities we create for player (as script name states)
public class PlayerManager : MonoBehaviour
{
    Animator animator;
    AnimatorManager animatorManager;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animatorManager = GetComponent<AnimatorManager>(); // for debug
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

        isInteracting = animator.GetBool("isInteracting"); // checking to isInteracting
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
