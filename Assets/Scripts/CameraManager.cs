using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform;  // the object the camera will follow
    public Transform cameraPivot; // the object the camera uses to pivot
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;
    private Vector3 offset;

    public float lookAngle; // up and down look
    public float pivotAngle; // left and right look
    public float minPivotAngle = -35;
    public float maxPivotAngle = 35;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        // If you want to assign via inspector, only do this if null
        if (targetTransform == null)
        {
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }

        // Save initial offset between camera rig and target
        offset = transform.position - targetTransform.position;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }

    private void FollowTarget()
    {
        //Vector3 targetPosition = Vector3.SmoothDamp
        //    (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        //transform.position = targetPosition; // basically with this the position of the camera
        // will follow after each frame

        Vector3 targetPosition = targetTransform.position + offset;

        targetPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref cameraFollowVelocity,
            cameraFollowSpeed
        );

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle + (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
}
