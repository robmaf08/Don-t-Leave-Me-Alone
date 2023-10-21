using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueChanger : MonoBehaviour
{

    [TextArea] public string[] texts;

    public TextMeshProUGUI diagText;

    int currentIndex = 0;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = texts[0];
        diagText.text = texts[0];
        SetFirstDialogue();
    }

    public void SetFirstDialogue()
    {
        StartCoroutine(FadeInDiag());
    }

    public void NextDialogue()
    {
        StartCoroutine(ChangeDialogue());
    }

    private IEnumerator ChangeDialogue()
    {
        yield return StartCoroutine(FadeOutDiag());
        yield return new WaitForSeconds(1.0f);
        currentIndex++;
        if (currentIndex < texts.Length)
        {
            GetComponent<TextMeshProUGUI>().text = texts[currentIndex];
            diagText.text = texts[currentIndex];
        }
        else
        {
            currentIndex = 0;
            StopCoroutine(ChangeDialogue());
        }
        yield return StartCoroutine(FadeInDiag());
    }

    /* This method allows to fade out text */
    private IEnumerator FadeOutDiag()
    {
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            diagText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        diagText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, 0f);
    }

    /* This method allows to fade in text */
    private IEnumerator FadeInDiag()
    {
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            diagText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        diagText.GetComponent<TextMeshProUGUI>().color = new Color(222f, 222f, 222f, 222f);
    }

}
