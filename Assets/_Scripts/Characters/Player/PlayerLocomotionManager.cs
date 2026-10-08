using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{

    PlayerManager player;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header("Movement Settings")]
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 7.5f;
    [SerializeField] float rotationSpeed = 15;

    [Header("Dodge")]
    private Vector3 rollDirection;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement; 
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            // if not locked on, pass the move amount
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            // if locked on, pass the hori and vert
        }
    }

    public void HandleAllMovement()
    {
        // ground movement
        HandleGroundMovement();
        HandleRotation();
    }
    
    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        // clamp the movements
    }

    private void HandleGroundMovement()
    {
        if(!player.canMove)
            return;
            
        GetMovementValues();

        // move direction is based on camera facing direction
        moveDirection = (PlayerCamera.instance.transform.forward * verticalMovement) + (PlayerCamera.instance.transform.right * horizontalMovement);
        
        moveDirection.y = 0;
        moveDirection.Normalize();

        // if sprinting
        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        // if not sprinting
        else
        {
            if(PlayerInputManager.instance.moveAmount > 0.5f)
        {
                // move at a running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
            else if(PlayerInputManager.instance.moveAmount > 0)
            {
                // move at at walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        
    }

    private void HandleRotation()
    {
        if(!player.canRotate) 
            return;

        targetRotationDirection = Vector3.zero;
        targetRotationDirection =  (PlayerCamera.instance.cameraObject.transform.forward * verticalMovement) + 
                                    (PlayerCamera.instance.cameraObject.transform.right * horizontalMovement);
    
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if(targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;

    }

    public void AttempToPerformDodge()
    {
        if(player.isPerformingAction) return;

        if(PlayerInputManager.instance.moveAmount > 0)
        {
            Vector3 cameraForward = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            Vector3 cameraRight = PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;

            rollDirection = (cameraForward + cameraRight).normalized;

            rollDirection.y = 0;

            rollDirection.Normalize();
            
            player.transform.rotation = Quaternion.LookRotation(rollDirection);

            // perform roll animation
            player.playerAnimatorManager.PlayTargetAnimation("roll_forward_01", true, true);
        }

        else
        {
            // perfrom backstep animation
            player.playerAnimatorManager.PlayTargetAnimation("back_step_01", true, true);
        }

    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // if we're moving, sprinting is true
        if(moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        // otherwise, sprinting is false
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        //

    }

}
