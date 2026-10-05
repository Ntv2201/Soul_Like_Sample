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
    [SerializeField] private float upAndDownRotateSpeed = 200; 
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
        // locked on, force the rotation toward target

        // else rotate regularly

        // normal rotation

        // rotate left and right based on horizontal movement
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        // rotate up and down based on vertical movement
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotateSpeed) * Time.deltaTime;

        // clamp the up and down look angle between a max and a min value
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);


        // rotate this onject left and right
        transform.rotation = Quaternion.Euler(0, leftAndRightLookAngle, 0);

        // rotate thi pivot game object up and down
        cameraPivotTransform.localRotation = Quaternion.Euler(upAndDownLookAngle, 0, 0);
        
    }

    private void HandleCollision()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // limit the distance so the camera can't get through the charater (avoid see inside us)
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

}
