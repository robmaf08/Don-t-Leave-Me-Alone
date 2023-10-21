using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeathManager : MonoBehaviour
{
    
    [SerializeField] private PlayerHealth health;

    public GameObject firstButton;

    public GameObject UI_Death;

    private StarterAssets.StarterAssetsInputs _input;

    private void Start()
    {
        UI_Death.SetActive(false);
        _input = StarterAssets.StarterAssetsInputs.Instance;
    }

    public void InstantDie()
    {
        Die();
    }

    private void Die()
    {
        UI_Death.SetActive(true);
        Raycast.Instance.RaycastEnable(false);
        StateMachine.Instance.DialogueSate(true);
        StateMachine.Instance.ShowCursor(true);
        Time.timeScale = 0f;
        StartCoroutine(SetSelected(firstButton));   
    }

    private void Update()
    {
        if (UI_Death.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject == null
              && InputHandler.isGamepadConnected
              && (_input.move.y > 0 || _input.move.y < 0))
            {
                StartCoroutine(SetSelected(Button.allSelectablesArray[0].gameObject));
            }
        }
        
        if (health.GetHealth() == 0 && !UI_Death.activeSelf)
        {
            Die();
        }
    }

    private void StopAllAudios() 
    {
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>(true))
            audio.Stop();
    }

    private IEnumerator SetSelected(GameObject selected)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(selected);
    }

}
