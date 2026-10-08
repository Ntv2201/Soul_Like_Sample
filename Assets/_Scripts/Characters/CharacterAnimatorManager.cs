using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    float vertical;
    float horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }


    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        character.animator.SetFloat("Horizontal", horizontalValue, .1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", verticalValue, .1f, Time.deltaTime);
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
