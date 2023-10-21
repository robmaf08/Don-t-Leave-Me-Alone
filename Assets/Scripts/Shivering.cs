using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shivering : MonoBehaviour
{
    public static Shivering instance;

    private void Awake () 
    {
        instance = this;
    }

    [Header("Shivering Audio Components")]
    public AudioSource audioSource;
    public AudioClip shiveringNormal;
    public AudioClip shiveringColdZone;

    private bool isInColdZone = false;

    private void Start()
    {
        audioSource.clip = shiveringNormal;
        audioSource.loop = true; 
    }

    public void StartShivering() 
    {
        audioSource.clip = shiveringNormal;
        audioSource.Play();
    }

    public void EnterColdZone(bool enter)
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                if (enter)
                {
                    //currentTime = audioSource.time;
                    audioSource.clip = shiveringColdZone;
                    //audioSource.volume = 0.68f;
                    audioSource.loop = true;
                    audioSource.Play();
                    SwitchHotToColdZone();
                } else
                {
                    audioSource.clip = shiveringNormal;
                    //audioSource.time = currentTime;
                    audioSource.volume = 0.85f;
                    audioSource.Play();
                }
            }
        }
    }

    public void EnterHotZone(bool enter) 
    {
        StopAllCoroutines();
        if (enter && audioSource != null)
        {
            StartCoroutine(SwitchColdToHotZone());
        } else
        {
            StartCoroutine(SwitchHotToColdZone());
        }
    } 

    private IEnumerator SwitchColdToHotZone() 
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0.010f, 0.0075f);
            
            if (audioSource.volume < 0.2f)
                audioSource.Pause();

            yield return null;
        }
    }

    private IEnumerator SwitchHotToColdZone()
    {
        float volume = 1;   
        if (isInColdZone)
            volume = 0.5f;

        if (!audioSource.isPlaying)
            audioSource.Play();

        while (audioSource.volume < volume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, volume, 0.0095f);
            yield return null;
        }
    }

}
