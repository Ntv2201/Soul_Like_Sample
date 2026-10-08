using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        
    }

    protected override void Update()
    {
        base.Update();

        // if we do not own this object, we do not control or edit it
        if(!IsOwner) 
            return;

        // handle all character movement
        playerLocomotionManager.HandleAllMovement();

    }

    protected override void LateUpdate()
    {
        if(!IsOwner)
            return;
        
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // if this player object owned by this client
        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
        }
    }
}
