using UnityEngine;

public class Food : StoreItem
{
    [Header("Components")]
    [SerializeField] private PlayerHealth playerHealth;
    public AudioSource eatSound;
    public AudioClip eatSoundClip;

    [Header("Properties")]
    public float healthGain;
    public float resistanceGain;

    private void Start() 
    {
        descriptionItem = descriptionItem + "\nAumento salute: " 
            + healthGain.ToString() + "%\n" 
            + "Aumento Resistenza: "
            + resistanceGain.ToString() + "%."; 
    }

    public override string UseItem() 
    {
        base.UseItem(); 
        string message = string.Empty;
        if (playerHealth.GetHealth() < playerHealth.maxHealth)
        {
            playerHealth.RestoreHealth(healthGain);
            playerHealth.RestoreResistance(resistanceGain);
            if (eatSound != null && eatSoundClip != null)
                eatSound.PlayOneShot(eatSoundClip);

            Destroy(gameObject);
        }
        else
        {
            message = "La vita è già al massimo";
        }
        return message;
    }

}
