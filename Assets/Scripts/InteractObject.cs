using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InteractObject : MonoBehaviour
{

    public enum InteractType
    {
        ExamineOnly,
        ExamineAndUse,
        UseOnly
    }

    public InteractType itemTypeInteraction;

    public UnityEvent OnInteractEvent;

    public UnityEvent OnExamineEvent;

    public bool interactOnce = false;

    public string useCommandText = Raycast.DEFAULT_USE_COMMAND_TEXT;

    public string examineCommandText = Raycast.DEFAULT_EXAMINE_COMMAND_TEXT;

    [System.Serializable]
    public class OnKeyPressedEvent
    {
        public KeyCode keyCode;
        public UnityEvent onKeyPressed;
    }

    public virtual void OnHitCollision ()
    {
        /* When the Raycast hit the object, it will check the type
         * of interaction and shows the right commands */
        if (itemTypeInteraction == InteractObject.InteractType.ExamineOnly)
        {
            Raycast.ShowCommandUse(useCommandText, false);
            Raycast.ShowCommandExamine(examineCommandText, true);
        }
        if (itemTypeInteraction == InteractObject.InteractType.ExamineAndUse)
        {
            Raycast.ShowCommandUse(useCommandText, true);
            Raycast.ShowCommandExamine(examineCommandText, true);
        }
        if (itemTypeInteraction == InteractObject.InteractType.UseOnly)
        {
            Raycast.ShowCommandUse(useCommandText, true);
            Raycast.ShowCommandExamine(examineCommandText, false);
        }
    }

    public virtual void OnNotHitCollision()
    {
        Raycast.ShowCommandUse(useCommandText, false);
        Raycast.ShowCommandExamine(examineCommandText, false);
    }

    public virtual void OnInteract()
    {  
        OnInteractEvent.Invoke();
        if (interactOnce)
        {
            NotInteractableAnymore();
            OnDisable();
        }
    }

    public virtual void OnExamine()
    {
        if (itemTypeInteraction == InteractObject.InteractType.ExamineOnly 
            || itemTypeInteraction == InteractObject.InteractType.ExamineAndUse)
        {
            OnExamineEvent.Invoke();
            NotInteractableAnymore();
            OnNotHitCollision();
        }
    }

    public void NotInteractableAnymore()
    {
        //tag = "Untagged";
        OnNotHitCollision();
        if (interactOnce)
        {
            tag = "Untagged";
            enabled = false;
            OnNotHitCollision();
        }

        if (GetComponentInChildren<Viewpoint>() != null)
        {
            GetComponentInChildren<Viewpoint>().OnDisable();
        }  
    }

    public void IsInteractable()
    {
        if (interactOnce)
        {
            tag = "Untagged";
            enabled = false;
        } else
        {
            tag = "InteractableObject";
            enabled = true;
        }
        
        if (GetComponentInChildren<Viewpoint>() != null)
        {
            GetComponentInChildren<Viewpoint>().enabled = true;
            tag = "InteractableObject";
            enabled = true;
        }
    }

    public virtual void OnDisable()
    {
        tag = "Untagged";
        OnNotHitCollision();

    }

    public void SetUseCommandText(string text)
    {
        if (text != string.Empty)
        {
            useCommandText = text;
        } else
        {
            useCommandText = string.Empty;
        }
    }

    public void SetExamineCommandText(string text)
    {
        if (text != string.Empty)
        {
            examineCommandText = text;
        }
        else
        {
            examineCommandText = string.Empty;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 3.5f);
    }

}
