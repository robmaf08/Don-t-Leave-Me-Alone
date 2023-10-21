using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookObject : MonoBehaviour
{

    [TextArea] public string[] phrases;
    public TextMeshProUGUI lookText;
    public bool stopPlayerWhenLook = false;

    public virtual void OnLook() 
    {
        StartCoroutine(Look());
    }

    public virtual void OnLook(string[] phrases) { }


    public IEnumerator Look() 
    {
        
        if (stopPlayerWhenLook)
        {
            //Player is looking object so stop him.
            StateMachine.Instance.InspectingState(true);
            Raycast.Instance.RaycastEnable(false);
        }

        lookText.transform.parent.gameObject.SetActive(true);
        lookText.gameObject.SetActive(true);

        lookText.GetComponent<TypeWriterUI>().Run(phrases, lookText);
        yield return new WaitWhile(() => lookText.GetComponent<TypeWriterUI>().GetIsPlaying() == true);

        if (stopPlayerWhenLook)
        {
            //Close look text
            StateMachine.Instance.InspectingState(false);
            Raycast.Instance.RaycastEnable(true);
        }

        lookText.transform.parent.gameObject.SetActive(false);

    }

    public IEnumerator Look(string[] phrases)
    {

        if (stopPlayerWhenLook)
        {
            //Player is looking object so stop him.
            StateMachine.Instance.DialogueSate(true);
            Raycast.Instance.RaycastEnable(false);
        }

        lookText.transform.parent.gameObject.SetActive(true);
        lookText.gameObject.SetActive(true);

        lookText.GetComponent<TypeWriterUI>().Run(phrases, lookText);
        yield return new WaitWhile(() => lookText.GetComponent<TypeWriterUI>().GetIsPlaying() == true);

        if (stopPlayerWhenLook)
        {
            //Close look text
            StateMachine.Instance.DialogueSate(false);
            Raycast.Instance.RaycastEnable(true);
        }
        lookText.transform.parent.gameObject.SetActive(false);
    }

    public static IEnumerator LookOnly (TextMeshProUGUI TextGUI, string LookDescription) 
    {
        StateMachine.Instance.InspectingState(true);
        TextGUI.transform.parent.gameObject.SetActive(true);
        TextGUI.gameObject.SetActive(true);
        yield return new WaitWhile(() => TextGUI.GetComponent<TypeWriterUI>().GetIsPlaying() == true);
        StateMachine.Instance.InspectingState(false);
    }


}
