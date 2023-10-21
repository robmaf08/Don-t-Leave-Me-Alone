using UnityEngine;
using UnityEngine.UI;


public class StoreItem : MonoBehaviour
{
    public enum itemType
    {
        Food,
        Drink,
        Battery,
        Note,
        Key
    }

    [Header("Item information")]
    public itemType typeItem;
    public string nameItem;   
    public GameObject objectItem;
    public Sprite imageItem;
    [TextArea] public string descriptionItem;

    [Header("Pickup Audio Components")]
    public AudioSource audioSourceItem;
    public AudioClip audioItemPickup;

    public virtual string UseItem() { return string.Empty; }

    public void PickupItem()
    {
        if (audioSourceItem != null && audioItemPickup != null)
        {
            audioSourceItem.PlayOneShot(audioItemPickup);
            gameObject.SetActive(false);
        }
    }

}
