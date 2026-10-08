using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }


    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting)
        {
            verticalAmount = 2;
        }

        character.animator.SetFloat(horizontal, horizontalAmount, .1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmount, .1f, Time.deltaTime);
    }

    public virtual void PlayTargetAnimation(
        string targetAnimation, bool isPerformingAction, 
        bool applyRootMotion = true, bool canRotate = false, 
        bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        // can be used to stop character from attempting other action
        // (for example: if u get damaged and begin performing a damage action, this flag will turn true of you're stunned)
        character.isPerformingAction = isPerformingAction;
        character.canMove = canMove;
        character.canRotate = canRotate;

        // tell the server/host we played an animation and play that to present animation to everyone
        character.characterNetworkManager.NotifyTheServerActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

}
