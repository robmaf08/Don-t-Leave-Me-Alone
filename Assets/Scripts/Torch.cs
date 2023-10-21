using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Torch : MonoBehaviour, IDifficulty
{

    //Torch parameters 
    private const float AUTONOMY_EASY = 7;
    private const float AUTONOMY_MEDIUM = 5;
    private const float AUTONOMY_HARD = 3;
    [HideInInspector] public float autonomy;

    //Light Source Flashlight
    public Light lightSource;

    //Torch capacity
    public float minBatteryCapacity = 0f;
    public float maxBatteryCapacity = 100f;
    public float batteryCapacity;
    private float amountCapacityDrop = 1;
    public float timeForDecrease = 1f;

    [Header("Leds torch")]
    public GameObject ledTorchHigh;
    public GameObject ledTorchMedium;
    public GameObject ledTorchLow;
    public Image capacityBarUI;

    [Header("Color Capacity Torch")]
    public Color batteryFull;
    public float percentageFull = 100f;
    public Color batteryMedium;
    public float percentageMedium = 60f;
    public Color batteryLow;
    public float percentageLow = 40f;

    [Header("Audio Torch")]
    public AudioClip reloadTorch;

    private GameObject audioTorch;
    private bool canDrainBattery = true;
    private bool doOnce = true;
    private StarterAssets.StarterAssetsInputs _input;


    void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        audioTorch = new GameObject();
        audioTorch.AddComponent<AudioSource>();
        UpdateUI();
        if (!lightSource.enabled)
        {
            ledTorchLow.SetActive(true);
        }

        ChangeValuesByDifficulty();
    }

    public void ChangeValuesByDifficulty() 
    {
        if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
        {
            autonomy = AUTONOMY_EASY;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
        {
            autonomy = AUTONOMY_MEDIUM;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Hard)
        {
            autonomy = AUTONOMY_HARD;
        }
        amountCapacityDrop = maxBatteryCapacity / (autonomy * 60);
    }

    void Update() 
    {
        if (lightSource.enabled)
        {
            UpdateUI();
            SetLedColor();
            if (canDrainBattery)
                StartCoroutine(DrainBattery());
        }

        if (StateMachine.Instance != null && StateMachine.Instance.canEquipItem)
        {
            //Check if battery empty
            if (_input.turnTorch && !lightSource.enabled && batteryCapacity <= 0)
            {
                _input.Reset();
                if (doOnce)
                {
                    StartCoroutine(TurnOnWithEmptyBattery());
                    return;
                }
            }
            else if (_input.turnTorch && !lightSource.enabled && batteryCapacity > 0)
            {
                _input.turnTorch = false;
                if (audioTorch.GetComponent<AudioSource>() != null && audioTorch.GetComponent<AudioSource>().isPlaying)
                    audioTorch.GetComponent<AudioSource>().Stop();

                if (!lightSource.enabled && !audioTorch.GetComponent<AudioSource>().isPlaying)
                {
                    audioTorch.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Torch/TorchTurnOn"));
                    lightSource.enabled = true;
                    ledTorchHigh.SetActive(true);
                    ledTorchLow.SetActive(false);
                    ledTorchMedium.SetActive(false);
                }
                _input.Reset();
                return;
            }

            //Check if turning off lights
            if (_input.turnTorch && lightSource.enabled)
            {
                _input.turnTorch = false;
                PlayTurnOffSound();
                lightSource.enabled = false;
                ledTorchHigh.SetActive(false);
                ledTorchLow.SetActive(true);
                ledTorchMedium.SetActive(false);
                _input.Reset();
                return;
            }
        }
    }

    public void AddCapacity(float amount) 
    {
        if (reloadTorch != null)
            StartCoroutine(PlayAudioReload());

        batteryCapacity += amount;
        if (batteryCapacity > maxBatteryCapacity)
        {
            batteryCapacity = maxBatteryCapacity;
        }
        UpdateUI();
    }

    public void DecreaseCapacity(float amount)
    {
        batteryCapacity -= amount;
        if (batteryCapacity < minBatteryCapacity)
        {
            OnBatteryEmpty();
        }
        UpdateUI();
    }

    private void OnBatteryEmpty()
    {
        batteryCapacity = 0;
        lightSource.enabled = false;
        ledTorchHigh.SetActive(false);
        ledTorchMedium.SetActive(false);
        ledTorchLow.SetActive(true);
        PlayTurnOffSound();
    }

    private void PlayTurnOffSound() 
    {
        if (audioTorch.GetComponent<AudioSource>() != null && audioTorch.GetComponent<AudioSource>().isPlaying)
            audioTorch.GetComponent<AudioSource>().Stop();

        if (lightSource.enabled || !lightSource.enabled && !canDrainBattery)
            audioTorch.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Torch/TorchTurnOff"));
    }

    private void UpdateUI() 
    {
        if (capacityBarUI != null)
        {
            capacityBarUI.fillAmount = batteryCapacity / maxBatteryCapacity;

            if (batteryCapacity <= percentageLow)
            {
                capacityBarUI.color = batteryLow;
            }
            else if (batteryCapacity <= percentageMedium)
            {
                capacityBarUI.color = batteryMedium;
            }
            else
            {
                capacityBarUI.color = batteryFull;
            }
        }
    }

    private void SetLedColor() 
    {
        if (batteryCapacity <= percentageLow)
        {
            ledTorchMedium.SetActive(true);
            ledTorchHigh.SetActive(false);
            ledTorchLow.SetActive(false);
        }
        else if (batteryCapacity <= percentageMedium)
        {
            ledTorchMedium.SetActive(true);
            ledTorchHigh.SetActive(false);
            ledTorchLow.SetActive(false);
        }
        else
        {
            ledTorchMedium.SetActive(false);
            ledTorchHigh.SetActive(true);
            ledTorchLow.SetActive(false);
        }
    }

    private IEnumerator PlayAudioReload() 
    {
        GameObject audio = new GameObject();
        audio.AddComponent<AudioSource>();
        audio.GetComponent<AudioSource>().PlayOneShot(reloadTorch);
        yield return new WaitForSeconds(reloadTorch.length);
        Destroy(audio);
    }

    private IEnumerator DrainBattery()
    {
        float timer = (float)timeForDecrease;
        canDrainBattery = false;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        DecreaseCapacity(amountCapacityDrop);
        canDrainBattery = true;
    }

    /* This method will turn on the flashlight for a 
     * very small amount of time and turn it off instantly */
    private IEnumerator TurnOnWithEmptyBattery()
    {
        doOnce = false;
        ledTorchMedium.SetActive(true);
        lightSource.enabled = true;
        yield return new WaitForSeconds(0.3f);
        lightSource.enabled = false;
        ledTorchLow.SetActive(true);
        doOnce = true;
    }

    private void OnDisable()
    {
        lightSource.enabled = false;
    }
}
