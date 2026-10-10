using System;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    private float staminaRegenTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenDelay = 2f;
    [SerializeField] float staminaRegenAmount = 1f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;
        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        // only owner can edit their work variables
        if(!character.IsOwner) return;

        // no rengen stamina while using it
        if(character.characterNetworkManager.isSprinting.Value) return;

        if (character.isPerformingAction) return;

        staminaRegenTimer += Time.deltaTime;

        if(staminaRegenTimer >= staminaRegenDelay)
        {
            if(character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if(staminaTickTimer >= 0.1f)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                }
            }
        }
        
    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // only reset the regen if the action used stamina
        // no reset if the stamina is already regenerating
        if(currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenTimer = 0;
        }
    }

}
