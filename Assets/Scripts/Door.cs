using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Door : MonoBehaviour
{
    [Header("Door Components")]
    public Animator doorAnimator;
    public AnimationClip doorOpenClip;
    public AnimationClip doorCloseClip;
    private UI_InventorySystem inventorySystem;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip openingAudioClip;
    public AudioClip closingAudioClip;
    public AudioClip unlockingSoundClip;
    public AudioClip lockedSoundClip;

    [Header("Check if door need key")]
    public bool needKey = false;
    public bool isLocked = false;
    public GameObject keyToUnlock;

    private bool isAnimating = false;
    [HideInInspector] public bool opened = false;

    void Start()
    {
        if (GameObject.Find("UI_InventorySystem"))
            inventorySystem = GameObject.Find("UI_InventorySystem").GetComponent<UI_InventorySystem>();
        
        if (needKey)
        {
            if (GetComponent<InteractObject>() != null)
            {
                GetComponent<InteractObject>().SetUseCommandText("Sblocca");
            }
        }

    }

    public void Open()
    {
        //Try to unlock the door
        if (needKey && isLocked)
        {
            UnlockDoor();
        } else
        {
            if (!isAnimating && !isLocked)
            {
                if (opened)
                {
                    StartCoroutine(OpenDoor(false));
                    if (GetComponent<InteractObject>() != null)
                    {
                        GetComponent<InteractObject>().SetUseCommandText("Apri");
                    }
                }
                else
                {
                    StartCoroutine(OpenDoor(true));
                    if (GetComponent<InteractObject>() != null)
                    {
                        GetComponent<InteractObject>().SetUseCommandText("Chiudi");
                    }
                }
                opened = !opened;
            }
        }
    }

    private void UnlockDoor()
    {
        if (needKey)
        {
            if (isLocked)
            {
                //Search in the inventory if key is found
                if (inventorySystem != null)
                {
                    bool hasKey = inventorySystem.SearchItem(keyToUnlock);
                    /* If the key is on inventory 
                        then unlock the door*/
                    if (hasKey)
                    {
                        isLocked = false;
                        StartCoroutine(Unlock());
                    }
                    else
                    {
                        StartCoroutine(Locked());
                    }
                } else
                {
                    StartCoroutine(Locked());
                }
            }  
        }
    }

    private IEnumerator Unlock()
    {
        /* A new audiosource is created to reproduce the
         * door's unlock sound */
        GameObject audioSource = new GameObject();
        audioSource.AddComponent<AudioSource>();
        audioSource.GetComponent<AudioSource>().PlayOneShot(unlockingSoundClip);
        if (GetComponent<InteractObject>() != null)
        {
            GetComponent<InteractObject>().SetUseCommandText("Apri");
        }
        yield return new WaitForSeconds(unlockingSoundClip.length); 
        Input.ResetInputAxes();
        Destroy(audioSource);
    }

    private IEnumerator Locked()
    {
        GameObject audioSource = new GameObject();
        audioSource.AddComponent<AudioSource>();
        audioSource.GetComponent<AudioSource>().PlayOneShot(lockedSoundClip);
        yield return new WaitForSeconds(lockedSoundClip.length);
        Destroy(audioSource);   
    }

    private IEnumerator OpenDoor(bool open)
    {
        isAnimating = true;
        tag = "Untagged";
        float lenghtAnimation;
        if (open)
        {
            doorAnimator.Play(doorOpenClip.name);
            lenghtAnimation = doorOpenClip.length;
        }
        else
        {
            doorAnimator.Play(doorCloseClip.name);
            lenghtAnimation = doorCloseClip.length;
        }

        if (open)
        {
            if (audioSource != null && openingAudioClip != null)
                audioSource.PlayOneShot(openingAudioClip);
        }
        else
        {
            if (audioSource != null && closingAudioClip != null)
                audioSource.PlayOneShot(closingAudioClip);
        }
        yield return new WaitForSecondsRealtime(lenghtAnimation + 0.5f);
        isAnimating = false;
        tag = "InteractableObject";
    }
}
