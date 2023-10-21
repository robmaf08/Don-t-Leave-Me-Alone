using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Slot : MonoBehaviour
{
    public bool isEmpty = true;

    public GameObject itemObject;

    [HideInInspector] public Vector3 scaleItem;

    public List<StoreItem> itemStore;

    public int quantity;

    public TextMeshProUGUI quantityText;

    public Image imageItem;

    public Button removeButton;

    public Sprite emptySlotImage;

    [SerializeField] private Transform dropItemSocket;

    private int currentIndex = 0;

    public void SetQuantityText()
    {
        if (quantityText != null && quantity > 0)
            quantityText.text = quantity.ToString();
        else
            quantityText.text = string.Empty;
    }

    public void SetImageItem(Sprite sprite)
    {
        imageItem.sprite = sprite;
    }

    public Sprite GetImage()
    {
        return imageItem.sprite;
    }

    public void DestroyItem()
    {
        if (quantity == 1)
        {
            isEmpty = true;
            quantity = 0;
            scaleItem = new Vector3(0, 0, 0);
            SetImageItem(emptySlotImage);
            removeButton.gameObject.SetActive(false);
        }
        else
        {
            quantity--;
        }
        GameObject item = new GameObject();
        item = itemStore[currentIndex].objectItem.gameObject;
        itemStore.RemoveAt(0);
        Destroy(item);
        SetQuantityText();
    }

    public void DropItem()
    {
        if (itemStore[0] != null)
        {
            try
            {
                //Debug.Log("Item Dropped: " + itemStore[0]);
            } finally
            {
                itemStore[0].objectItem.SetActive(true);
                itemStore[0].objectItem.transform.position = dropItemSocket.position;
                itemStore[0].objectItem.gameObject.transform.localScale = scaleItem;
                itemStore[0].objectItem.tag = "InteractableObject";
                if (itemStore[0].objectItem.GetComponent<Rigidbody>() != null)
                {
                    itemStore[0].objectItem.GetComponent<Rigidbody>().AddForce(new Vector3(3f, 3f, 3f));
                    itemStore[0].objectItem.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(2, 2, 2), ForceMode.Impulse);
                }
                RemoveItem();
            }
        }
    }

    public void RemoveItem()
    {
        // Debug.Log("Item removed: " + itemStore[currentIndex].objectItem.name);
        if (quantity == 1)
        {
            isEmpty = true;
            quantity = 0;
            SetImageItem(emptySlotImage);
            removeButton.gameObject.SetActive(false);
        }
        else
        {
            quantity--;
        }
        itemStore.RemoveAt(0);
        SetQuantityText();
    }

    public void Reset()
    {
        itemStore = new List<StoreItem>();
        quantity = 0;
        isEmpty = true;
        SetQuantityText();
        SetImageItem(emptySlotImage);
        scaleItem = new Vector3(0,0,0);
        removeButton.gameObject.SetActive(false);
    }
}
