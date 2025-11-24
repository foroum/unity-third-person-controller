using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    int horizontal;
    int vertical;

    [Header("Added For Debugging")]
    Rigidbody rb;
    PlayerManager playerManager;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        // for falling
        rb = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
    }

    // for falling
    private void OnAnimatorMove()
    {
        if (playerManager == null || rb == null)
            return;

        // Only use root motion when interacting (e.g. attack, roll)
        if (!playerManager.isInteracting)
            return;

        float delta = Time.deltaTime;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0; // ignore vertical from animation

        Vector3 velocity = deltaPosition / delta;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f); // crossfade animation
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // animation snapping
        float snappedHorizontal, snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else { 
            snappedHorizontal= 0;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        } // gia na kamnei matching me to tree pou ekamame

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime); // 0.1f damp time = blend time so it doesn't look sudden
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime); // changed to snappedHorizontal/Vertical so when the character stops
    }
}
