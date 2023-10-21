using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.EventSystems;

public class StateMachine : MonoBehaviour
{
    public static StateMachine Instance;
    [Header("Game Object Represents state machine")]
    [SerializeField] GameObject weaponCamera;
    public GameObject PlayerCharacter;
    public GameObject PlayerUI;
    public PauseMenuManager pauseMenu;
    public GameObject[] AlwaysSeeObj;
    public GameObject[] HudsObj;
    public GameObject Crosshair;

    private bool isInspecting = false;
    private bool isGamePaused = false;
    private bool isInDialogue = false;
    private bool isInventoryOpen = false;

    [HideInInspector] public bool canEquipItem = true;
    [HideInInspector] public bool canShoot = false;
    [HideInInspector] public bool isFighting = false;

    public void StopAllAudios() {
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>(true))
        {
            audio.Stop();
            if (audio.GetComponent<AudioBehaviour>() != null)
                audio.GetComponent<AudioBehaviour>().enabled = false;
        }
    }

    public bool IsInDialogue()
    {
        return isInDialogue;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }
    public bool IsInspecting()
    {
        return isInspecting;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowObjects(GameObject[] arrayObj)
    {
        if (!isInDialogue && !isInventoryOpen && !isInspecting)
        {
            for (int i = 0; i < arrayObj.Length; i++)
            {
                if (arrayObj[i].activeSelf)
                {
                    if (arrayObj[i].GetComponent<Canvas>() != null)
                    {
                        arrayObj[i].GetComponent<Canvas>().enabled = true;
                    }
                }

            }
        }
       
    }

    public void NotShowObjects(GameObject[] arrayObj)
    {
        for (int i = 0; i < arrayObj.Length; i++)
        {
            if (arrayObj[i] != null &&  arrayObj[i].activeSelf)
            {
                if (arrayObj[i].GetComponent<Canvas>() != null)
                {
                    arrayObj[i].GetComponent<Canvas>().enabled = false;
                }
            }
        }
    }
    
    public void DialogueSate(bool enable)
    {
        if (enable)
        {
            isInDialogue = true;
            NotShowObjects(HudsObj);
            PlayerCanMove(false);
            canEquipItem = false;
            weaponCamera.gameObject.SetActive(false);
            canShoot = false;
        } else
        {
            isInDialogue = false;
            PlayerCanMove(true);
            ShowObjects(HudsObj);
            Crosshair.SetActive(true);
            canEquipItem = true;
            weaponCamera.gameObject.SetActive(true);
            canShoot = true;
        }
   
    }

    private void EquipItemState(bool equipe)
    {
        canEquipItem = equipe;
    }

    public void IsLookingState(bool looking)
    {
        Crosshair.SetActive(!looking);
        PlayerCanMove(!looking);
        canEquipItem = !looking;
    }

    public void PauseMenuState(bool enable)
    {
        if (enable)
        {
            gameObject.SetActive(true);
            PlayerCanMove(false);
            NotShowObjects(HudsObj);
            Time.timeScale = 0f;
            isGamePaused = true;
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("ContinueButton"));
            canShoot = false;
            canEquipItem= false;
        } else
        {
            if (isInspecting || isInDialogue || isInventoryOpen)
            {
                PlayerCanMove(false);
                //NotShowObjects(HudsObj);
                ShowCursor(true);
                canEquipItem = false;
                canShoot = false;
            } else
            {
                PlayerCanMove(true);
                canShoot = true;
                canEquipItem = true;
                //ShowObjects(HudsObj);
            }
            Time.timeScale = 1f;
            ShowCursor(false);
            isGamePaused = false;
           
        }
    }

    public void InventoryMenuState(bool enable)
    {
       if (enable && !isInventoryOpen)
        {
            isInventoryOpen = true;
            PlayerCanMove(false);
            NotShowObjects(HudsObj);
            canEquipItem = false;
            canShoot = false;
        } else
        {
            canShoot = true;
            isInventoryOpen = false;
            canEquipItem = true;
            PlayerCanMove(true);
            ShowObjects(HudsObj);
            if (!weaponCamera.activeSelf)
                weaponCamera.SetActive(true);
        }
    }

    public void PlayerCanMove(bool canMove)
    {
        if (PlayerCharacter != null)
        {
            if (!PlayerCharacter.transform.parent.gameObject.activeSelf && canMove)
                PlayerCharacter.transform.parent.gameObject.SetActive(true);

            if (!canMove)
                PlayerCharacter.GetComponent<ThirdPersonController>().OnDisable();

            PlayerCharacter.GetComponent<ThirdPersonController>().enabled = canMove;
            PlayerCharacter.GetComponent<Animator>().enabled = canMove;
            PlayerCharacter.GetComponent<FootstepSoundPlayer>().enabled = canMove;
            canEquipItem = canMove;
            canShoot = canMove;
        }
    }

    public void InspectingState(bool inspect)
    {
        if (inspect)
        {
            isInspecting = true;
            PlayerCanMove(false);
            NotShowObjects(HudsObj);
            canEquipItem = false;
            weaponCamera.SetActive(false);
            canShoot = false;
        }
        else
        {
            isInspecting = false;
            if (!isInventoryOpen)
            {
                PlayerCanMove(true);
                ShowObjects(HudsObj);
                Crosshair.SetActive(true);
                canEquipItem = true;
                weaponCamera.SetActive(true);
            }
        }
    }

    public void TurnOffAllSound()
    {
         AudioSource[] audios = FindObjectsOfType<AudioSource>(true);  
         foreach (AudioSource audio in audios) 
            audio.Stop();
    }

    public void ShowCursor(bool show)
    {
        if (show)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }


}
