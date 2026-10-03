using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        //
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
}
