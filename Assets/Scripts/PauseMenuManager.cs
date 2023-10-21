using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{

    [SerializeField] GameObject pauseMenuUI;
    public GameObject pauseMenuAnimation;

    [Header("Components for animation")]
    public AnimationClip animationOpening;
    public AnimationClip animationClosing;
    [SerializeField] private Image imageBackground;

    [Header("Pause Menu Buttons")]
    public GameObject[] ButtonsMainMenu;
    public static GameObject currentPanelActive;
    private GameObject lastButtonSelected;

    [Header("UI Components")]
    public GameObject uiControlSetting;
    public TextMeshProUGUI gamepadConnectedText;

    [Header("Audio Components")]
    public AudioSource BackgroundAudio;
    [SerializeField] private AudioSource audioPauseMenu;
    [SerializeField] private AudioClip openingPauseMenu;
    [SerializeField] private AudioClip idlePauseMenu;
    [SerializeField] private AudioClip exitPauseMenu;

    [Header("Show Current Goal Components")]
    [SerializeField] GoalManager goalManager;
    public TextMeshProUGUI currentGoalText;

    private bool isSettingMenuOpen = false;
    private StarterAssets.StarterAssetsInputs _input;
    private Vector3 startingMousePos;

    void Start()
    {
        _input = StarterAssetsInputs.Instance;
        pauseMenuUI.SetActive(false);
        currentGoalText.text = string.Empty;
    }

    private void Update()
    {
        if (startingMousePos != null 
            && startingMousePos != Input.mousePosition 
            && !Cursor.visible
            && pauseMenuUI.activeSelf)
        {
            StateMachine.Instance.ShowCursor(true);
        }

        if (!InputHandler.isGamepadConnected)
        {
            gamepadConnectedText.text = "Non connesso";
            uiControlSetting.GetComponentInChildren<Dropdown>().value = 0;
        }
        else
        {
            gamepadConnectedText.text = "Connesso";
        }

        if (_input.pauseGame)
        {
            _input.Reset();
            if (!pauseMenuUI.activeSelf)
            {
                OpenPauseMenu();
            } else
            {
                ContinueGame();
            }
        } else
        {
            if (pauseMenuUI.activeSelf)
            {
                if (InputHandler.isGamepadConnected 
                    && _input.backButton
                    && !isSettingMenuOpen)
                {
                    ContinueGame();
                    _input.Reset();
                }
            }
        }

        if (pauseMenuUI.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject == null
                 && InputHandler.isGamepadConnected
                 && (_input.move.y > 0 || _input.move.y < 0))
            {
                StartCoroutine(SetSelected(ButtonsMainMenu[0]));
            }
        }

        if (isSettingMenuOpen)
        {
            if ((InputHandler.isGamepadConnected
                && _input.backButton)
                || Input.GetKeyDown(KeyCode.Mouse1))
            {
                uiControlSetting.SetActive(false);
                isSettingMenuOpen = false;
                _input.Reset();
                Input.ResetInputAxes();
                EnableButtons();
                StartCoroutine(SetSelected(ButtonsMainMenu[1]));
            }

            if (!InputHandler.isGamepadConnected)
            {
                ControlSchemeManager.instance.ChangeControlScheme();
                uiControlSetting.GetComponentInChildren<Dropdown>().value = 0;
            }

        }

        if (!InputHandler.isGamepadConnected)
        {
            ControlSchemeManager.instance.ChangeToKeyboardIcon();
            uiControlSetting.GetComponentInChildren<Dropdown>().value = 0;
        }

    }

    private void OnDisable()
    {
        foreach (GameObject btn in ButtonsMainMenu)
        {
            btn.GetComponent<Button>().interactable = false;
        }
        isSettingMenuOpen = false;
        uiControlSetting.SetActive(false);
    }

    private void OnEnable()
    {
        foreach (GameObject btn in ButtonsMainMenu)
        {
            btn.GetComponent<Button>().interactable = true;
        }
        currentPanelActive = null;
    }

    public void OpenControlSetting()
    {
        uiControlSetting.SetActive(true);
        isSettingMenuOpen = true;
        DisableButtons();
        uiControlSetting.GetComponentInChildren<Dropdown>().value = (int)InputHandler.currentInputScheme;
        StartCoroutine(SetSelected(uiControlSetting.GetComponentInChildren<Dropdown>().gameObject));
        if (InputHandler.isGamepadConnected)
            gamepadConnectedText.text = "Connesso";
        else
            gamepadConnectedText.text = "Non connesso";
    }

    public  IEnumerator SetSelected(GameObject selected) 
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(selected);
    }

    public void DisableButtons()
    {
        foreach (GameObject btn in ButtonsMainMenu)
        {
            btn.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach (GameObject btn in ButtonsMainMenu)
        {
            btn.GetComponent<Button>().interactable = true;
        }
    }

    public void ContinueGame()
    {
        StateMachine.Instance.PauseMenuState(false);
        _input.Reset();
        StartCoroutine(ResetEventSystem());
        if (isSettingMenuOpen)
        {
            isSettingMenuOpen = false;
            uiControlSetting.SetActive(false);
            StartCoroutine(ResetEventSystem());
        }

        audioPauseMenu.clip = exitPauseMenu;
        audioPauseMenu.PlayOneShot(audioPauseMenu.clip);
        StartCoroutine(WaitAnimationClosing());
    }

    IEnumerator WaitAnimationOpening()
    {
        pauseMenuAnimation.GetComponent<Animator>().Play(animationOpening.name);
        yield return new WaitForSeconds(animationOpening.length - 0.5f);
    }

    IEnumerator WaitAnimationClosing()
    {
        pauseMenuAnimation.GetComponent<Animator>().Play(animationClosing.name);
        yield return new WaitForSeconds(animationClosing.length - 0.5f);
        if (!StateMachine.Instance.IsInventoryOpen())
            StateMachine.Instance.ShowObjects(StateMachine.Instance.HudsObj);

        if (Cursor.visible)
            StateMachine.Instance.ShowCursor(false);

        pauseMenuUI.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        _input.Reset();
        startingMousePos = Input.mousePosition;
        
        EnableButtons();
        pauseMenuUI.SetActive(true);
        imageBackground.enabled = true;
        enabled = true;
        StateMachine.Instance.PauseMenuState(true);
        StopAllCoroutines();
        StartCoroutine(WaitAnimationOpening());
        audioPauseMenu.enabled = true;
        audioPauseMenu.clip = openingPauseMenu;
        audioPauseMenu.PlayOneShot(audioPauseMenu.clip);
        BackgroundAudio.Pause();
        audioPauseMenu.clip = idlePauseMenu;
        audioPauseMenu.Play();

        //Show current goal
        currentGoalText.transform.parent.gameObject.SetActive(true);
        currentGoalText.text = goalManager.GetCurrentGoal().nameGoal;

        /* if Gamepad is connected the cursor will not show */
        if (InputHandler.isGamepadConnected && Gamepad.current != null)
        {
            StateMachine.Instance.ShowCursor(false);
        } else
        {
            StateMachine.Instance.ShowCursor(true);
        } 
    }

    private IEnumerator ResetEventSystem() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.firstSelectedGameObject = null; 
    }


    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        InputHandler.OnApplicationClose();
        Application.Quit();
    }
}
