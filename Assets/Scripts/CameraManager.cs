using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform targetTransform;  // the object the camera will follow
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;
    private Vector3 offset;

    public float lookAngle; // up and down look
    public float pivotAngle; // left and right look

    private void Awake()
    {
        // If you want to assign via inspector, only do this if null
        if (targetTransform == null)
        {
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }

        // Save initial offset between camera rig and target
        offset = transform.position - targetTransform.position;
    }

    public void FollowTarget()
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

    public void RotateCamera()
    {
        //lookAngle = lookAngle + (mouseXinput * cameraLookSpeed);
        //pivotAngle = pivotAngle + (mouseYinput * cameraPivotSpeed);

    }
}
