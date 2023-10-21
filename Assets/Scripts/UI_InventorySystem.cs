using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UI_InventorySystem : MonoBehaviour
{
    public GameObject uiInventory;

    [Header("Item Slot Information")]
    public List<UI_Slot> slots;
    [SerializeField] private TextMeshProUGUI nameItemText;
    [SerializeField] private TextMeshProUGUI descriptionItemText;
    public TextMeshProUGUI slotNumberUsedText;

    [Header("Use Item Slot Information")]
    [SerializeField] private TextMeshProUGUI useItemText;
    [SerializeField] private TextMeshProUGUI useSlotMessageText;

    [Header("Note inspection")]
    [SerializeField] private NoteSystem noteSystem;

    [Header("UI Item Added")]
    [SerializeField] private GameObject UI_ItemAdded;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI itemNameAdded;

    [Header("Audios")]
    public AudioSource dropItemSound;

    public Animator pauseMenuAnimator;
    public Image imageClose;
    public EventSystem eventSystem;
    private bool isInspectingNote = false;
    private int maxSlots = 14;
    private int slotUsed = 0;
    private StarterAssets.StarterAssetsInputs _input;

    private void Start()
    {
        uiInventory.SetActive(false);
        SetSlotsUI();
        nameItemText.transform.parent.gameObject.SetActive(false);
        UI_ItemAdded.SetActive(false);
        _input = StarterAssets.StarterAssetsInputs.Instance;
    }

    public void AddItem(StoreItem item)
    {
        StopAllCoroutines();
        foreach (UI_Slot slot in slots)
        {
            if (slot != null)
            {
                //Check if same object is already in inventory
                if (!slot.isEmpty)
                {
                    foreach (StoreItem itemList in slot.itemStore)
                    {
                        if (itemList.typeItem == item.typeItem && itemList.imageItem == item.imageItem)
                        {
                            slot.itemStore.Add(item);
                            slot.quantity++;
                            slot.scaleItem = item.gameObject.transform.localScale;
                            slot.SetQuantityText();
                            slot.removeButton.gameObject.SetActive(true);
                            item.PickupItem();
                            // Show UI Item Added
                            StartCoroutine(ShowPopupItemAdded(item.nameItem));
                            return;
                        }
                    }
                }

                /* If the object is not already in the inventory we check if there's a free slot 
                         to containt it*/
                if (slot.isEmpty)
                {
                    slot.isEmpty = false;
                    slot.itemStore.Add(item);
                    slot.quantity++;
                    slot.SetQuantityText();
                    slot.scaleItem = item.gameObject.transform.localScale;
                    slot.SetImageItem(item.imageItem);
                    ShowItemInfo(slot);
                    item.PickupItem();
                    slot.removeButton.gameObject.SetActive(true);
                    // Show UI Item Added
                    UpdateSlotsNumber();
                    StartCoroutine(ShowPopupItemAdded(item.nameItem));
                    return;
                }
            }
        }
    }

    public bool SearchItem(StoreItem item)
    {
        int j = 0;
        bool found = false;
        for (int i = 0; i < slots.Count; i++)
        {
            j = 0;
            for (j = 0; j < slots[i].itemStore.Count; j++)
            {
                if (slots[i].itemStore[j] == item)
                {
                    found = true;
                    break;
                }
            }
        }
        return found;
    }

    public bool SearchItem(GameObject item) 
    {
        int j = 0;
        bool found = false;
        for (int i = 0; i < slots.Count; i++)
        {
            j = 0;
            for (j = 0; j < slots[i].itemStore.Count; j++)
            {
                if (slots[i].itemStore[j].objectItem == item)
                {
                    found = true;
                    break;
                }
            }
        }
        return found;
    }

    public void RemoveItem(UI_Slot slot)
    {
        slot.RemoveItem();
        SortItems();
    }

    public void DropItemUI(UI_Slot slot) 
    {
        dropItemSound.Play();
        slot.DropItem();
        StartCoroutine(Sort());
        //SortItems();
    }

    public void UseSlot(UI_Slot slot) 
    {
        //Debug.Log("Button clicked");
        if (slot != null && !slot.isEmpty)
        {
            StoreItem item = slot.itemStore[0];
            string messageUse = string.Empty;
            if (item.typeItem == StoreItem.itemType.Food)
            {
                Food food = (Food)item;
                messageUse = food.UseItem();
                if (messageUse == string.Empty) slot.DestroyItem();
            }
            else if (item.typeItem == StoreItem.itemType.Drink)
            {
                Drink drink = (Drink)item;
                messageUse = drink.UseItem();
                if (messageUse == string.Empty) slot.DestroyItem();
            }
            else if (item.typeItem == StoreItem.itemType.Battery)
            {
                Battery battery = (Battery)item;
                messageUse = battery.UseItem();
                if (messageUse == string.Empty) slot.DestroyItem();
            }
            else if (item.typeItem == StoreItem.itemType.Note)
            {
                //Debug.Log("Inspecting note: " + item.name);
                Note note = (Note)item;
                noteSystem.InspectNote(note);
                uiInventory.gameObject.SetActive(false);
                isInspectingNote = true;
            }

            //Display message cannot use item
            if (messageUse != string.Empty)
            {
                //StopAllCoroutines();
                StartCoroutine(FadeInDiag(messageUse));
            }
            SortItems();
        }
    }

    public void ShowItemInfo(UI_Slot slot)
    {
        if (!slot.isEmpty)
        {
            nameItemText.text = slot.itemStore[0].nameItem;
            descriptionItemText.text = slot.itemStore[0].descriptionItem;
        }
    }

    private void UpdateSlotsNumber() 
    {
        slotUsed = 0;
        foreach (UI_Slot slot in slots)
        {
            if (!slot.isEmpty)
                slotUsed++;
            else
                break;
        }
        slotNumberUsedText.text = slotUsed.ToString() + "/" + maxSlots.ToString();
    }

    public void NotShowItemInfo(UI_Slot slot) 
    {
        nameItemText.text = string.Empty;
        descriptionItemText.text = string.Empty;
    }

    private IEnumerator Sort() {
        yield return new WaitForSeconds(0.09f);
        SortItems();
        UpdateSlotsNumber();
    }

    private void SortItems() 
    {
        List<UI_Slot> slotsTemp = new List<UI_Slot>();
        //selection sort
        for (int i = 0; i < slots.Count; i++)
        {
            for (int j = i + 1; j < slots.Count; j++)
            {
                if (slots[i].isEmpty && !slots[j].isEmpty)
                {
                    slots[i].isEmpty = false;
                    slots[i].itemStore = slots[j].itemStore;
                    slots[i].quantity = slots[j].quantity;
                    slots[i].scaleItem = slots[j].scaleItem;
                    slots[i].SetQuantityText();
                    slots[i].SetImageItem(slots[j].GetImage());
                    slots[i].removeButton.gameObject.SetActive(true);
                    //Reset slot at index j
                    slots[j].Reset();
                }
            }
        }
    }

    private void SetSlotsUI() 
    {
        foreach (UI_Slot slot in slots)
        {
            slot.SetQuantityText();
        }
    }

    private void Update() 
    {

        if (_input.inventory 
            && !StateMachine.Instance.IsInspecting() 
            && !StateMachine.Instance.IsInDialogue())
        {
            if (uiInventory.activeSelf)
            {
                OnInventoryClose();
                _input.jump = false;
            }
            else
            {
                if (!StateMachine.Instance.IsGamePaused())
                {
                    StopAllCoroutines();
                    if (UI_ItemAdded.activeSelf) UI_ItemAdded.SetActive(false);

                    uiInventory.SetActive(true);
                    OnInventoryOpen();
                    StateMachine.Instance.InventoryMenuState(true);
                    StateMachine.Instance.ShowCursor(true);
                    Raycast.Instance.RaycastEnable(false);
                    GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(slots[0].gameObject);
                }
            }
            _input.Reset();
        }


        //Check if inspecting note
        if (isInspectingNote)
        {
            if (!StateMachine.Instance.IsInspecting())
            {
                uiInventory.SetActive(true);
                isInspectingNote = false;
            }
        }

        if (uiInventory.activeSelf)
        {
            /*If player is using gamepad and no button is selected, when the analog moves
            it selects the first slot*/
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (InputHandler.isGamepadConnected)
                {
                    if (_input.move.x > 0 || _input.move.x < 0 
                        || (_input.move.y > 0) || _input.move.y < 0)
                    {
                        StartCoroutine(First());
                        _input.Reset();
                    }
                }
            }

            /* If player is using gamepad, we check if remove button action is perfomed*/
            if (InputHandler.isGamepadConnected && Gamepad.current != null 
                && Gamepad.current.rightTrigger.isPressed
                && uiInventory.activeSelf)
            {
                UI_Slot currentSlot = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<UI_Slot>();
                if (currentSlot != null && !currentSlot.isEmpty)
                    DropItemUI(currentSlot);

                _input.Reset();
                Input.ResetInputAxes();
            }
        }

    }

    private void OnInventoryOpen()
    {
        StartCoroutine(PlaySoundInventory(true));
        if (!slots[0].isEmpty)
        {
            nameItemText.transform.parent.gameObject.SetActive(true);
            nameItemText.text = slots[0].itemStore[0].nameItem;
            descriptionItemText.text = slots[0].itemStore[0].descriptionItem;
        }
        StartCoroutine(ShowInventory(true));
    }

    private void OnInventoryClose()
    {
        StartCoroutine(PlaySoundInventory(false));
        if (!slots[0].isEmpty)
        {
            nameItemText.transform.parent.gameObject.SetActive(false);
            nameItemText.text = string.Empty;
            descriptionItemText.text = string.Empty;
        }
        useSlotMessageText.text = string.Empty;
        StartCoroutine(First());
        StartCoroutine(ShowInventory(false));
        Raycast.Instance.RaycastEnable(true);
    }

    private IEnumerator PlaySoundInventory(bool open) 
    {
        GameObject audio = new GameObject();
        audio.AddComponent<AudioSource>();
        AudioClip clip;
        if (open)
            clip = Resources.Load<AudioClip>("Audio/Inventory/InventoryOpen");
        else
            clip = Resources.Load<AudioClip>("Audio/Inventory/InventoryClose");

        audio.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(audio, clip.length); 
        yield return new WaitForSeconds(clip.length);
    }
    private IEnumerator FadeOutDiag()
    {
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            useSlotMessageText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        useSlotMessageText.text = string.Empty;
    }

    private IEnumerator FadeInDiag(string message)
    {
        useSlotMessageText.text = message;
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            useSlotMessageText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        useSlotMessageText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, 222f);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeOutDiag());
    }

    private IEnumerator ShowPopupItemAdded(string nameItem)
    {
        UI_ItemAdded.SetActive(true);
        itemNameAdded.text = nameItem;
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
                text.color = new Color(text.color.r, text.color.g, text.color.b, i);

            background.fillAmount = i;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
                text.color = new Color(text.color.r, text.color.g, text.color.b, i);

            background.fillAmount = i;
            yield return new WaitForSeconds(0.01f);
        }
        itemNameAdded.text = string.Empty;
        UI_ItemAdded.SetActive(false);
    }

    private IEnumerator ShowInventory(bool show) 
    {
        if (show)
        {
            pauseMenuAnimator.Play("OpenInventory");
            StartCoroutine(First());
            yield return new WaitForSeconds(1f);
        }
        else
        {
            for (float i = 0f; i <= 1f; i += 0.05f)
            {
                imageClose.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return new WaitForSeconds(0.01f);
            }

            imageClose.GetComponent<Image>().color = Color.black;
            uiInventory.SetActive(false);
            StateMachine.Instance.InventoryMenuState(false);
            StateMachine.Instance.ShowCursor(false);
            for (float i = 1f; i >= 0f; i -= 0.05f)
            {
                imageClose.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private IEnumerator First() 
    {
        eventSystem.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        eventSystem.SetSelectedGameObject(slots[0].imageItem.gameObject);
    }

}