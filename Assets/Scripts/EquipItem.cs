using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour
{

    [System.Serializable]
    public class PlayerItem
    {
        public Animator item;
        public AnimationClip clipEquip;
        public AnimationClip clipNotEquip;
        public GameObject itemUI;
        public bool equipped = false;
    }

    public PlayerItem[] items;
    private int currentIndex = -1;
    private bool isRemovingItem = false;
    private bool isChanging = false;

    public void CanEquip(bool canEquip) {
        StateMachine.Instance.canEquipItem = canEquip;
    }

    private void Start() 
    {  
        NotEquipAnything();
    }

    private void Update()
    {

        if (!isRemovingItem)
        {
            if (items.Length > 1)
            {
                
                if (StateMachine.Instance.canEquipItem)
                {
                    float scroll = Input.GetAxis("Mouse ScrollWheel");
                    if (scroll < 0 || StarterAssets.StarterAssetsInputs.Instance.changeItem)
                    {
                        StarterAssets.StarterAssetsInputs.Instance.changeItem = false;

                        if (!isChanging)
                        {
                            StopAllCoroutines();
                            if (currentIndex == -1)
                            {
                                ChangeItem();
                                return;
                            }

                            if (currentIndex >= 0)
                            {
                                ChangeItem();
                                return;
                            }
                        }
                    }
                }
            }
        } 
    }

    private void ChangeItem() 
    {
        int lastIndex = currentIndex;
        currentIndex++;
        
        /* If no other weapon can be selected, 
            then the first one will be equiped */
        if (currentIndex > items.Length - 1)
            currentIndex = 0;

        /* Unequip previous item */
        if (lastIndex >= 0)
            StartCoroutine(NotEquipItemAtIndex(lastIndex));

        /* Equip new item */
        StartCoroutine(EquipItemAtIndex(currentIndex));

    }

    private IEnumerator NotEquipItemAtIndex(int index)
    {
        isChanging = true;
        isRemovingItem = true;
        if (items[index] != null
             && items[index].clipNotEquip != null && items[index].clipEquip != null)
        {
            items[index].item.Play(items[index].clipNotEquip.name);
            yield return new WaitForSeconds(items[index].clipNotEquip.length);
        }
        yield return null;

        /* Disabling ui of item current equipped if any */
        if (items[index].itemUI != null)
            items[index].itemUI.SetActive(false);

        items[index].item.gameObject.SetActive(false);
        isRemovingItem = false;
    }

    private IEnumerator EquipItemAtIndex(int index)
    {
        /* If player tries to equip another item while is removing another
         then we must wait until animation is over */
        while (isRemovingItem)
        {
            yield return null;
        }

        items[index].item.gameObject.SetActive(true);
        if (items[index].clipEquip != null)
        {
            items[index].item.Play(items[index].clipEquip.name);
            yield return new WaitForSeconds(items[index].clipEquip.length);
        }

        /* Setting ui of item current equipped if any */
        if (items[index].itemUI != null)
            items[index].itemUI.SetActive(true);

        isChanging = false;

    }

    public void NotEquipAnything() 
    {
        StopAllCoroutines();
        foreach (PlayerItem item in items)
        {
            item.item.gameObject.SetActive(false);
            if (item.itemUI != null)
                item.itemUI.SetActive(false);

            isChanging = false;
        }
        //StartCoroutine(NotEquipItemAtIndex(currentIndex));  
    }

    public void EquipLastItem() 
    {
        StopAllCoroutines();
        StartCoroutine(EquipItemAtIndex(currentIndex));
    }

}


