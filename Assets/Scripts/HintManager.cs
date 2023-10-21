using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintManager : MonoBehaviour
{
 
    [SerializeField] private List<Hint> hintList;
    public AudioSource audioSource;
    public AudioClip clip;
    public float timeEachHint = 3f;
    private bool isCoroutineExecuting = false;
    private GameObject currentHint;
    private int delay = 0;

    public void AddHint(Hint hint)
    {
        this.hintList.Add(hint);
    }

    public void ClearList() 
    {
        StopAllCoroutines(); 
        isCoroutineExecuting = false; 
        hintList.Clear();
        if (currentHint != null)
        {
            currentHint.SetActive(false);
            currentHint = null;
        }
    }

    public void ShowHint(Hint hint) 
    {
        ClearList();
        if (!hint.showed)
            StartCoroutine(ShowSingleHint(hint));
    }

    public void ShowHints(List<Hint> hints)
    {
        if (!isCoroutineExecuting)
            StartCoroutine(ShowMultipleHints(hints));
    }

    public void ShowAllHints()
    {
        StopAllCoroutines();
        isCoroutineExecuting = false;
        if (!isCoroutineExecuting)
            StartCoroutine(ShowMultipleHints(this.hintList));
    }

    public void ShowAllHints(int delay)
    {
        this.delay = delay;
        StopAllCoroutines();
        isCoroutineExecuting = false;
        if (!isCoroutineExecuting)
            StartCoroutine(ShowMultipleHints(this.hintList));
    }

    private IEnumerator ShowMultipleHints(List<Hint> hintList)
    {
        isCoroutineExecuting = true;
        yield return new WaitForSeconds(delay); 
        for (int i = 0; i < hintList.Count; i++)
        {
            PlayHintSound();
            hintList[i].hintPrefab.SetActive(true); 
            currentHint = hintList[i].gameObject;
            yield return StartCoroutine(FadeInHint(hintList[i]));
            yield return new WaitForSeconds(timeEachHint);
            yield return StartCoroutine(FadeOutHint(hintList[i]));
            hintList[i].hintPrefab.SetActive(false);
        }
        isCoroutineExecuting = false;
        ClearList();
    }

    private IEnumerator ShowSingleHint(Hint hint) 
    {
        isCoroutineExecuting = true;
        hint.hintPrefab.SetActive(true);
        hint.showed = true;
        currentHint = hint.gameObject;
        PlayHintSound();
        yield return StartCoroutine(FadeInHint(hint));
        yield return new WaitForSeconds(timeEachHint);
        yield return StartCoroutine(FadeOutHint(hint));
        hint.hintPrefab.SetActive(false);   
        isCoroutineExecuting = false;
    }

    private IEnumerator FadeOutHint(Hint hint)
    {
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            hint.textHintObj.color = new Color(222f, 222f, 222f, i);
            if (hint.imageHint != null)
            {
                hint.imageHint.color = new Color(222f, 222f, 222f, i);
            }
            yield return new WaitForSeconds(0.01f);
        }
        
    }

    private IEnumerator FadeInHint(Hint hint)
    {
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            hint.textHintObj.color = new Color(222f, 222f, 222f, i);
            if (hint.imageHint != null)
            {
                hint.imageHint.color = new Color(222f, 222f, 222f, i);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void PlayHintSound() 
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

}
