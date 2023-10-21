using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour 
{

    public AudioSource HoverSound;

    [Header("For changing Background Images")]
    public Canvas MainMenu;
    public RawImage Background;
    public TextMeshProUGUI PressAnyKeyBtn;

    [Header("Events after press any key")]
    public UnityEvent KeyPressed;
    public UnityEvent BeforeKeyPressed;

    [Header("Main Menu Buttons")]
    public GameObject[] ButtonsMainMenu;

    [Header("Before Start")]
    [SerializeField] private Animator PressAnyKeyText;
    [SerializeField] private Animator Logo;

    [Header("UI Components")]
    public GameObject uiMainMenu;
    public GameObject uiSetting;
    public GameObject uiPanelSetting;
    public GameObject uiChooseDifficulty;
    public GameObject uiControls;
    public GameObject uiQuit;
    public TextMeshProUGUI gamepadConnected;
    private GameObject currentUIOpen;

    private GameObject lastButtonSelected;
    private bool isSettingMenuOpen = false;
    public static GameObject currentPanelActive;
    private StarterAssets.StarterAssetsInputs _input;
    private Vector3 startingMousePos;

    void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        MainMenu.enabled = false;
        PressAnyKeyBtn.enabled = true;
        enabled = false;
        StartCoroutine(WaitForKeyPress());
    }

    private void Update()
    {
        startingMousePos = Input.mousePosition;

        if (EventSystem.current.currentSelectedGameObject == null
            && InputHandler.isGamepadConnected
            && (_input.move.y > 0 || _input.move.y < 0)
            && !isSettingMenuOpen)
        {
            SetObjectSelected(ButtonsMainMenu[0]);
        } else if (InputHandler.isGamepadConnected 
            && Cursor.visible
            && (_input.move.y > 0 || _input.move.y < 0))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if ((Input.GetAxis("Mouse X") != 0 && !Cursor.visible)
            || (!InputHandler.isGamepadConnected && !Cursor.visible))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (isSettingMenuOpen)
        {
            if (!InputHandler.isGamepadConnected)
            {
                gamepadConnected.text = "Non connesso";
                uiControls.GetComponentInChildren<Dropdown>().value = 0;
            } else
            {
                gamepadConnected.text = "Connesso";
            }

            if (_input.backButton)
            {
                if (currentUIOpen != null)
                {
                    currentUIOpen.SetActive(false);
                    currentUIOpen = null;
                }
                EnableButtons();
                _input.Reset();
            }

            /* If player is using gamepad and no button is currently selected, 
             * if any analog movement is performed then the first avaiable button 
             * will be selected*/
            if (EventSystem.current.currentSelectedGameObject == null
                && InputHandler.isGamepadConnected
                && (_input.move.y > 0 || _input.move.y < 0))
            {
                StartCoroutine(SetSelected(Button.allSelectablesArray[0].gameObject));
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    public void SetDifficultyEasy() 
    {
        Difficulty.Instance.SetDifficulty(LevelDifficulty.Difficulty.Easy);
    }

    public void SetDifficultyMedium() 
    {
        Difficulty.Instance.SetDifficulty(LevelDifficulty.Difficulty.Medium);
    }

    public void SetDifficultyHard()
    {
        Difficulty.Instance.SetDifficulty(LevelDifficulty.Difficulty.Hard);
    }

    public void PlayHover() 
    {
        HoverSound.Play();
    }

    public void QuitGame()
    {
        Debug.Log("Game Closed.");
        InputHandler.OnApplicationClose();
        Application.Quit();
    }

    public void OpenUI(GameObject ui) 
    {
        ui.SetActive(true);
        currentUIOpen = ui;
        OpenSettingUI();
    }

    private void OpenSettingUI()
    {
        isSettingMenuOpen = true;
        DisableButtons();
    }

    public void CloseAllUI()
    {
        if (isSettingMenuOpen)
        {
            uiQuit.SetActive(false);
            uiSetting.SetActive(false);
        }
    }

    private IEnumerator WaitForKeyPress() 
    {
        yield return new WaitWhile(() => !Input.anyKeyDown);
        Time.timeScale = 1f;
        StartCoroutine(Any_Key_Pressed_Animation());
    }

    IEnumerator Any_Key_Pressed_Animation() 
    {

        //Play animation of UI Componets after key pressed
        Logo.Play("Logo");
        PressAnyKeyText.Rebind();
        PressAnyKeyText.Play("Any_Key_Pressed");
        GameObject.Find("OpeningSound").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2f);

        //Enable UI Main Menu
        PressAnyKeyBtn.gameObject.SetActive(false);
        MainMenu.enabled = true;
        KeyPressed.Invoke();

        //Select Play Button as first button selected
        SetObjectSelected(ButtonsMainMenu[0]);
        enabled = true;

        if (!ControlSchemeManager.instance.gamepadConnected)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void DisableButtons(GameObject firstButtonPanel) 
    {
        /* Store last button selected so that when user exit from the new ui opened
         * the button pressed will be selected again*/
        lastButtonSelected = EventSystem.current.currentSelectedGameObject;
        uiMainMenu.SetActive(false);
    }

    public void DisableButtons() 
    {
        /* Store last button selected so that when user exit from the new ui opened
         * the button pressed will be selected again*/
        lastButtonSelected = EventSystem.current.currentSelectedGameObject;
        uiMainMenu.SetActive(false);
    }

    public void EnableButtons()
    {
        uiMainMenu.SetActive(true);
        /* Set the last button that was selected and pressed */
        StartCoroutine(SetSelected(lastButtonSelected));
    }

    public void SetObjectSelected(GameObject selected)
    {
        StartCoroutine(SetSelected(selected));
    }

    public static IEnumerator SetSelected(GameObject selected)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(selected);
    }

}
