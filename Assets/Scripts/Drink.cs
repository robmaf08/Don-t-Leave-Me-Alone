using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : StoreItem
{
    [Header("Components")]
    public PlayerHealth playerHealth; 
    public UI_InventorySystem inventorySystem;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip drinkAudio;

    [Header("Properties")]
    public float staminaGain = 10f;
    public float timeStopStaminaDrop = 0f;

    public override string UseItem()
    {
        base.UseItem();
        string message = string.Empty;
        if (playerHealth.GetResistance() > 0 
            && playerHealth.GetResistance() < playerHealth.maxResistance)
        {
            playerHealth.RestoreResistance(staminaGain);
            audioSource.PlayOneShot(drinkAudio);
            Destroy(gameObject, 1f);
        } else
        {
            message = "La resistenza è già al massimo!";
        }
        return message;
    }

}
