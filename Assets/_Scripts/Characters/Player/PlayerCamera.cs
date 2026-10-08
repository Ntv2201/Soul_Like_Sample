using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;

    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // the bigger the number, the longer it takes to reach to its desired position
    [SerializeField] private float upAndDownRotationSpeed = 200; 
    [SerializeField] private float leftAndRightRotationSpeed = 200; 
    [SerializeField] float minimumPivot = -30;
    [SerializeField] float maximumPivot = 60;
    [SerializeField] float cameraCollisionRadius = .2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; // used for the camera collision (move camera to this position upon collididng)
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; // values used for the camera collisions
    private float targetCameraZPosition; // values used for the camera collisions


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            HandleFollowTarget();

            HandleRotation();

            HandleCollision();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // rotate the camera left and right
        transform.rotation = Quaternion.Euler(0, leftAndRightLookAngle, 0);

        // rotate the camera up and down
        cameraPivotTransform.localRotation = Quaternion.Euler(upAndDownLookAngle, 0, 0);

    }

    private void HandleCollision()
    {
        targetCameraZPosition = cameraZPosition;

        // direction for collision check
        Vector3 direction = (cameraObject.transform.position - cameraPivotTransform.position).normalized;
        
        // check if there is an object infront of our desired direction
        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction,
                            out var hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            // if there is, we get our distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);

            // we then equate our target z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // limit the camera distance from collision to its target position when collides with 
        targetCameraZPosition = Mathf.Clamp(targetCameraZPosition, cameraZPosition, -cameraCollisionRadius);

        // make the camera move to its target position smoothly
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;

    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(cameraObject.transform.position, cameraCollisionRadius);
        Gizmos.color = Color.red;
    }

}
