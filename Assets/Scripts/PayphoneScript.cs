using UnityEngine;
using System.Collections;
using TMPro;
using StarterAssets;

public class PayphoneScript : LookObject
{

    [TextArea] public string[] phrasesBeforeInteract;
    [TextArea] public string[] hasNoMoney;
    public CutsceneManager cutscene;
    [SerializeField] private GameObject InsertCoin;
    [SerializeField] private UI_InventorySystem inventory;
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private GameObject coins;
    public GameObject atm;

    public bool CoinInsered;
    private bool doOnce = true;

    private void Start()
    {
        CoinInsered = false;
        enabled = true;
        atm.tag = "Untagged";
    }

    public void SetCoinInsered(bool insered)
    {
        CoinInsered = insered;
    }

    public override void OnLook()
    {
        base.Look();
        //Find money in inventory
        bool hasMoney = inventory.SearchItem(coins);
        if (!hasMoney)
        {
            atm.tag = "InteractableObject";
            StartCoroutine(InteractDialogue(phrasesBeforeInteract));
            //StartCoroutine(Look(phrasesBeforeInteract));
        } else
        {
            StartCoroutine(InteractDialogue(phrases));
            atm.tag = "Untagged";
        }
        if (gameObject.GetComponent<CameraLookObj>() != null)
        {
            gameObject.GetComponent<CameraLookObj>().ChangeCamera();
        }
    }

    public void Interact()
    {
        //Find money in inventory
        Input.ResetInputAxes();
        bool hasMoney = inventory.SearchItem(coins);
        if (!hasMoney)
        {
            atm.tag = "InteractableObject";
            if (gameObject.GetComponent<CameraLookObj>() != null)
            {
                gameObject.GetComponent<CameraLookObj>().ChangeCamera();
            }
           StartCoroutine(InteractDialogue(hasNoMoney));
        } else
        {
            atm.tag = "Untagged";
            cutscene.StartCutscene();
        }
        
    }

    private IEnumerator InteractDialogue(string[] text)
    {
        yield return StartCoroutine(Look(text));
        if (doOnce)
        {
            goalManager.NextGoal();
            doOnce = false;
        }
    }


}
