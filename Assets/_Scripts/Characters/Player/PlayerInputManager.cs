using System.ComponentModel;
using System.Security;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{

    public static PlayerInputManager instance;
    public PlayerManager player;

    PlayerControls playerControls;

    [Header("Player Movement Inputs")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Inputs")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Player Input Actions")]
    [SerializeField] bool dodgeInput = false;

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
        
        // when scence changes, run this logic
        SceneManager.activeSceneChanged +=  OnSceneChanged;

        instance.enabled = false;
    }

    private void Update()
    {
        HandleAllInput();
    }

    private void HandleAllInput()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // if loading into world scene, enable players controls
        if(newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }

        // otherwise, we must be at the main menu, disable players controls
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;
        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // if destroy this object, unsub from this event
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // if we minimize or lower the window, stop adjustin inputs
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    // Movement
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // return the abs number
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // clamp the values, so they're 0, 0.5 or 1 (optional)  
        if(moveAmount <= 0.5f && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if(moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        // why pass 0?, cuz we only want non-strafe movement
        // we use the horizontal when we are strafing or locked on
        if(player == null) return;

        // if we are not locked on, only use the move amout
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);

        // if we are locked on, pass the hori movement as well

    }

    private void HandleCameraMovementInput()
    {
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }

    // Action
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            
            // future note: return (do nothing) if menu ui is open
            
            // perform a dodge;
            player.playerLocomotionManager.AttempToPerformDodge();
        }
    }

}
