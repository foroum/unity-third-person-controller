using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform;  // the object the camera will follow
    public Transform cameraPivot; // the object the camera uses to pivot
    public Transform cameraTransform; // transform of actual camera obj in scene
    public LayerMask collisionLayers; // layers we want camera to collide with
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraCollisionR;
    public float cameraFollowSpeed = 0.2f;
    public float cameraCollisionOffset = 0.2f;
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
        // if you want to assign via inspector, only do this if null
        if (targetTransform == null)
        {
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }
        cameraTransform = Camera.main.transform;
        // save initial offset between camera rig and target
        offset = transform.position - targetTransform.position;
        defaultPosition = cameraTransform.localPosition.z;
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
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle + (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionR, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
            {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = targetPosition - (distance - cameraCollisionOffset);
        }
    }
}
