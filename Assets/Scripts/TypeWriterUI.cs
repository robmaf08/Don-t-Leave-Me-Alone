using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using StarterAssets;

public class TypeWriterUI : MonoBehaviour
{
    [SerializeField] private float typeWriterSpeed = 50f;

    [SerializeField] public CharacterController player;

    public bool disableTextAfterReading = false;

    private bool IsPlaying = false;

    public float waitSecondAfterSence;
    
    public Animator gradientBackground;
    
    public AnimationClip gradientBackgroundOpening;

    public AnimationClip gradientBackgroundClosing;



    public bool GetIsPlaying()
    {
        return IsPlaying;
    }

    public void SetIsPlaying(bool play)
    {
        IsPlaying = play;
    }

    public void Run(string[] textTotType, TMP_Text textLabel)
    {
        StopAllCoroutines();
        CancelInvoke();
        IsPlaying = true;
        if (!textLabel.gameObject.activeSelf)
        {
            textLabel.gameObject.SetActive(true);
            textLabel.transform.parent.gameObject.SetActive(true);
        }

        //Debug.Log("Before calling coroutine");
        StartCoroutine(routine: TypeText(textTotType, textLabel));
    }
    
    public IEnumerator TypeText(string[] textTotType, TMP_Text textLabel)
    {
        //Debug.Log("Entered calling coroutine");
        if (gradientBackground != null)
        {
            gradientBackground.gameObject.SetActive(true);
            gradientBackground.Play("Gradient_Background");
        }

        int currentIndex = 0;
        textLabel.text = textTotType[currentIndex];
        IsPlaying = true;

        if (GetComponent<AudioSource>() != null)
        {
            if (GetComponent<AudioSource>().clip != null)
                GetComponent<AudioSource>().Play();
        }

        float time = 0;
        int charIndex = 0;
        string text = string.Empty;
        for (int i = 0; i < textTotType.Length; i++)
        {
            text = textTotType[i];
            time = 0;
            charIndex = 0;

            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();

            while (charIndex < text.Length)
            {
                time += Time.deltaTime * typeWriterSpeed;
                charIndex = Mathf.FloorToInt(time);
                charIndex = Mathf.Clamp(value: charIndex, min: 0, max: text.Length);
                textLabel.text = text.Substring(startIndex: 0, length: charIndex);
                yield return null;
            }
            GetComponent<AudioSource>().Pause(); 
            waitSecondAfterSence = waitSecondAfterSence >= 0f ? waitSecondAfterSence: 1f;
            yield return new WaitForSeconds(waitSecondAfterSence);
        }

        if (GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().Stop();
        }

        yield return new WaitForSeconds(2f);
       
        IsPlaying = false;
        textLabel.text = string.Empty;

        if (gradientBackground != null)
        {
            if (gradientBackgroundClosing != null)
                StartCoroutine(GradientBackgroundAnimation());
        }

    }

    private IEnumerator GradientBackgroundAnimation()
    {
        gradientBackground.Play("Gradient_Background_Closing");
        yield return new WaitForSeconds(gradientBackgroundClosing.length);
        gradientBackground.Rebind();
        gradientBackground.gameObject.SetActive(false);
    }

}

