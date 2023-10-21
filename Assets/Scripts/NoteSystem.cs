using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteSystem : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameObject noteUI;
    [SerializeField] private GameObject noteExamineUI;
    [SerializeField] private TextMeshProUGUI nameNote;
    [SerializeField] private TextMeshProUGUI descriptionNote;
    [SerializeField] private Image imageNote;
    [SerializeField] private Image imageExamineBackground;

    private Note inspectedNote;
    private bool onInspect = false;
    private bool onExamine = false;
    private StarterAssets.StarterAssetsInputs _input;

    private void Start()
    {
        enabled = false;
        _input = StarterAssets.StarterAssetsInputs.Instance;
    }

    public void InspectNote(Note note)
    {
        _input.Reset();

        //Set note to examine
        inspectedNote = note;
        inspectedNote.image = note.image;
        inspectedNote.name = note.name;
        inspectedNote.description = note.description;
        onInspect = true;

        //Set name note
        if (note.nameItem != null && note.nameItem != string.Empty)
        {
            nameNote.text = note.nameItem;
        } else
        {
            nameNote.text = "Nota";
        }
    
        note.gameObject.tag = "Untagged";
        
        //Activate UI & Set note information
        StateMachine.Instance.InspectingState(true);
        noteUI.SetActive(true);
        imageNote.sprite = inspectedNote.image;
        descriptionNote.text = inspectedNote.description;

        //Disable viewpoint if any
        if (note.gameObject.GetComponentInChildren<Viewpoint>() != null)
        {
            note.gameObject.GetComponentInChildren<Viewpoint>().OnDisable();
        }

        OnInspectNote();
        enabled = true;
    }

    void Update()
    {
        if (onInspect)
        {
            if (_input.jump || Input.GetKeyDown(KeyCode.E))
            {
                if (!onExamine)
                {
                    onExamine = true;
                    StartCoroutine(FadeInText());
                }
                _input.Reset();
            } 

            if (_input.backButton)
            {
                if (onExamine)
                {
                    //Debug.Log("Note system examine still not closed.");
                    onExamine = false;
                    StopAllCoroutines();
                    StartCoroutine(FadeOutText());
                } else
                {
                    //Debug.Log("Note system examine closed.");
                    noteUI.SetActive(false);
                    noteExamineUI.SetActive(false);
                    StateMachine.Instance.InspectingState(false);
                    enabled = false;
                }
                _input.Reset();
            }

            if (StateMachine.Instance != null && StateMachine.Instance.IsInventoryOpen())
            {
                if (_input.inventory)
                    _input.inventory = false;
            }
        }
    }

    private void OnDisable()
    {
        StateMachine.Instance.InspectingState(false);   
        if (inspectedNote != null)
        {
            inspectedNote.gameObject.tag = "InteractableObject";
            inspectedNote = gameObject.AddComponent<Note>(); 
            nameNote.text = string.Empty;   
        }
    }

    private void OnInspectNote()
    {
        //Play note sound
        GameObject soundPageFlip = new GameObject();
        soundPageFlip.AddComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("Audio/UI/PageFlip");
        soundPageFlip.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(soundPageFlip.gameObject, clip.length);
    }

    private IEnumerator FadeOutText()
    {
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            imageExamineBackground.color = new Color(0, 0, 0, i);
            descriptionNote.color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        noteExamineUI.SetActive(false);
    }

    private IEnumerator FadeInText()
    {
        noteExamineUI.SetActive(true);
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            imageExamineBackground.color = new Color(0, 0, 0, i);
            descriptionNote.color = new Color(222f, 222f, 222f, i);
            yield return new WaitForSeconds(0.01f);
        }
        StateMachine.Instance.InspectingState(true);

    }

}
