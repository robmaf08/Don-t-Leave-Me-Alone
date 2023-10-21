using System.Collections;
using UnityEngine;
using TMPro;

public class Keypad : MonoBehaviour
{
    //Default strings
    private const string DEFAULT_KEYCODE = "1234";
    private const string DEFAULT_KEYCODE_TEXT = "Insert Code";
    private const string DEFAULT_KEYCODE_WRONG = "Error code";
    private const string DEFAULT_KEYCODE_CORRECT = "Unlocked";

    [Header("Buttons Keypad")]
    public GameObject[] buttons;
    public bool isEnabled = true;

    [Header("Keypad LEDS")]
    public GameObject ledCode;
    private Material ledCodeMaterial;
    public TextMeshPro ledCodeText;
    public Material ledCodeWrong;
    public GameObject ledRed;
    public GameObject ledOrange;
    public GameObject ledGreen;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip audioButtonPressed;
    public AudioClip audioCorrectKey;
    public AudioClip audioWrongKey;

    public string keyCode = DEFAULT_KEYCODE;
    public bool openAfter = false;
    private string keyCodeTyped = string.Empty;

    [SerializeField] private Door[] door;

    void Start()
    {
        ledRed.SetActive(true);
        ledOrange.SetActive(false);
        ledGreen.SetActive(false);
        ledCodeText.text = DEFAULT_KEYCODE_TEXT;
        ledCodeMaterial = ledCode.GetComponent<MeshRenderer>().material;
        EnableKeypad(isEnabled);
    }

    public void EnableKeypad(bool enable)
    {
        string tag = string.Empty;
        if (enable)
            tag = "InteractableObject";
        else
            tag = "Untagged";

        foreach (GameObject button in buttons)
            button.tag = tag;
    }

    public void SetButtonPressed(KeypadButton pressed)
    {
        if (pressed != null)
        {
            bool flag = true;
            foreach (Door door in door)
            {
                if (!door.isLocked)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                StopAllCoroutines();
                ledCode.GetComponent<MeshRenderer>().material = ledCodeMaterial;
                keyCodeTyped += pressed.number.ToString();
                ledCodeText.text = keyCodeTyped;
                StartCoroutine(ButtonPressed());
            }
        }
    }

    public void DeleteCode()
    {
        if (keyCodeTyped != null
            && keyCodeTyped != string.Empty)
        {
            keyCodeTyped = keyCodeTyped.Substring(0, keyCodeTyped.Length - 1);
            ledCodeText.text = keyCodeTyped;
            StartCoroutine(ButtonPressed());
        }
    }

    public void InsertCode()
    {
        /* If the code insered is correct then unlock the door */
        if (keyCodeTyped != string.Empty
            && keyCodeTyped == keyCode)
        {
            audioSource.PlayOneShot(audioCorrectKey);
            foreach (Door door in door)
            {
                if (door.isLocked)
                {
                    door.isLocked = false;
                }
            }

            foreach (Door door in door)
            {
                if (!door.isLocked)
                {
                    if (openAfter)
                        door.Open();
                }
            }

            ledRed.SetActive(false);
            ledOrange.SetActive(false);
            ledGreen.SetActive(true);
            ledCodeText.text = DEFAULT_KEYCODE_CORRECT;
            tag = "Untagged";
        }
        else
        {
            keyCodeTyped = string.Empty;
            ledCode.GetComponent<MeshRenderer>().material = ledCodeWrong;
            ledCodeText.text = DEFAULT_KEYCODE_WRONG;
            audioSource.PlayOneShot(audioWrongKey);
        }
    }

    private IEnumerator ButtonPressed()
    {
        audioSource.PlayOneShot(audioButtonPressed);
        ledOrange.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ledOrange.SetActive(false);
    }

}
