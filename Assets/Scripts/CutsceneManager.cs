using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayableDirector cutscene;
    [SerializeField] private GameObject cutsceneComponents;
    public PlayerInput playerInput;
    public Image backgroundCloseCutscene;
    

    [Header("Properties")]
    public bool startOnSceneLoad;
    public bool canSkip = false;
    public KeyCode skipKey;

    public UnityEvent BeforeCutsceneStarts;
    public UnityEvent OnCutsceneComplete;

    private StarterAssets.StarterAssetsInputs _input;

    public void StartCutscene()
    {
        if (BeforeCutsceneStarts != null)
            BeforeCutsceneStarts.Invoke();

        /* Disabling the user interaction and setting the
         * state machine in Dialogue State*/
        playerInput.enabled = false;
        StateMachine.Instance.DialogueSate(true);
        Raycast.Instance.RaycastEnable(false);
        if (cutscene != null && !cutscene.gameObject.activeSelf)
        {
            cutscene.gameObject.SetActive(true);
            if (cutsceneComponents != null && !cutsceneComponents.activeSelf)
            {
                cutsceneComponents.SetActive(true);
            }
        }
        cutscene.Play();
        StartCoroutine(OpenCutscene());
        playerCamera.enabled = false; 
        playerCamera.GetComponent<AudioListener>().enabled = false;
        enabled = true;
    }

    public void StopCutscene()
    {
        enabled = false;
        cutscene.Stop();
        StartCoroutine(CloseCutscene());
    }

    public void PauseCutscene()
    {
        cutscene.Pause();
        enabled = false;
    }

    private void IsTimelineFinished()
    {
        if (cutscene != null && !(cutscene.state == PlayState.Playing))
        {
            StopCutscene();
        }
    }

    private IEnumerator OpenCutscene()
    {
        /* A black background will increasly decrase opacity 
         * until is completely transparent*/
        backgroundCloseCutscene.color = Color.black;
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            backgroundCloseCutscene.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.025f);
        }
        backgroundCloseCutscene.color = new Color(0,0,0,0);
    }

    private IEnumerator CloseCutscene()
    {
        if (!backgroundCloseCutscene.gameObject.activeSelf)
            backgroundCloseCutscene.color = new Color(0,0,0,0);

        /* A black background will increasly increase opacity 
         * until is completely back */
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            backgroundCloseCutscene.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }

        backgroundCloseCutscene.color = Color.black;
        cutscene.gameObject.SetActive(false);
        cutsceneComponents.SetActive(false);
        
        //Activating Main Camera
        playerCamera.enabled = true;
        playerCamera.GetComponent<AudioListener>().enabled = true;

        yield return new WaitForSeconds(1.5f);
        StateMachine.Instance.DialogueSate(false);
        backgroundCloseCutscene.color = new Color(0, 0, 0, 1);
      
        /* The black blackground will decrease until opacity is zero */
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            backgroundCloseCutscene.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        backgroundCloseCutscene.color = new Color(0, 0, 0, 0);
        backgroundCloseCutscene.gameObject.SetActive(false);

        /* Invoke custom events after cutscene is completed */
        if (OnCutsceneComplete != null)
            OnCutsceneComplete.Invoke();

        playerInput.enabled = true;
        Raycast.Instance.RaycastEnable(true);
    }


    private void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        if (cutscene != null && startOnSceneLoad)
        {
            StartCutscene();
            enabled = true;
        } else
        {
            cutscene.gameObject.SetActive(false);
            cutsceneComponents.gameObject.SetActive(false);
            enabled = false;
        }
    }

    private void Update() 
    {
        if (cutscene != null && cutscene.state == PlayState.Playing)
        {
            if ((Input.GetKeyDown(skipKey) || (InputHandler.isGamepadConnected && Gamepad.current.buttonSouth.isPressed))
                && canSkip
                && !StateMachine.Instance.IsGamePaused())
            {
                _input.Reset();
                StopCutscene();
                return;
            }
        }
        IsTimelineFinished();
    }

}
