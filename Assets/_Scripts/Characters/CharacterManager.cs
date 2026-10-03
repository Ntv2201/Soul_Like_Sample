using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterManager : NetworkBehaviour
{
    public CharacterController characterController;
    CharacterNetworkManager characterNetworkManager;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }

    protected virtual void Update()
    {
        // this character is being controlled by us, the assign its network position to the position
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }

        // otherwise, assign its position here locally by the position of its network transform
        else
        {
            // position
            transform.position = Vector3.SmoothDamp(transform.position, 
                                                    characterNetworkManager.networkPosition.Value, 
                                                    ref characterNetworkManager.networkVelocity, 
                                                    characterNetworkManager.networkPositionSmoothTime);

            // rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                characterNetworkManager.networkRotation.Value, 
                                                characterNetworkManager.networkRotationSmoothTime);
        }
    }
}
