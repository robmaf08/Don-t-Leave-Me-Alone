using UnityEngine;
using TMPro;

public class LightSwitching : MonoBehaviour
{
    
    [Header("Lights Components")]
    [SerializeField] private Light[] lights;
    [SerializeField] private LightProbeGroup[] lightProbeGroup;

    [Header("Audio Components")]
    [SerializeField] private AudioSource audioLightSwitch;
    [SerializeField] private AudioClip audioClip;
    
    //State Light/s
    public bool turnOn = true;
    
    private void Start()
    {
        SetTurnOnText();
        TurnOn(turnOn);
    }

    public void TurnOnLights()
    {
        PlayAudioTurnOn();
        if (turnOn)
        {
            TurnOn(false);
        } else
        {
            TurnOn(true);
        }
        SetTurnOnText();
    }

    private void SetTurnOnText()
    {
        if (GetComponent<InteractObject>() != null)
        {
            bool canSetText = true;
            if (Blackout.instance != null && !Blackout.instance.isBlackoutOn)
                canSetText = false;

            if (canSetText)
            {
                if (turnOn)
                    GetComponent<InteractObject>().SetUseCommandText("Spegni");
                else
                    GetComponent<InteractObject>().SetUseCommandText("Accendi");
            }
        }
    }

    public void TurnOn(bool turnOn)
    {
        bool canTurn = true;
        if (Blackout.instance != null && Blackout.instance.isBlackoutOn)
            canTurn = false;
 
        if (canTurn)
        {
            foreach (Light light in lights)
                light.gameObject.SetActive(turnOn);

            this.turnOn = turnOn;
        }
    }

    private void PlayAudioTurnOn()
    {
        audioLightSwitch.PlayOneShot(audioClip);
    }

}
