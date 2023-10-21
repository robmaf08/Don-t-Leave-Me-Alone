using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(PlayerHealth))]
public class TemperatureMonitor : MonoBehaviour, IDifficulty
{
    //Parameters for difficulty
    private const float DEFUALT_MIN_TEMPERATURE_SUFFER = -1f;
    private const float DEFUALT_MIN_TEMPERATURE_RESTORE = 13f;
    private const float TIME_SUFFER_BY_COLD_EASY = 100f;
    private const float TIME_SUFFER_BY_COLD_MEDIUM = 75f;
    private const float TIME_SUFFER_BY_COLD_HARD = 60f;
    private const float DAMAGE_SUFFER_BY_COLD_EASY = 2.5f;
    private const float DAMAGE_SUFFER_BY_COLD_MEDIUM = 4.5f;
    private const float DAMAGE_SUFFER_BY_COLD_HARD = 6f;

    [Header("Components")]
    [SerializeField] private GameObject temperatureMonitor;
    [SerializeField] private TextMeshPro temperatureMonitorText;
    [SerializeField] private AudioSource audioAlarm;
    [SerializeField] private GameObject ledAlarm;
    public GameObject uiIcon;

    [Header("Led colors")]
    public Material ledBad;
    public Material ledMedium;
    public Material ledGood;
    
    [Header("Default Cold Information")]
    public bool useDefaultValues = true;
    public int temperature = 8;
    public float timeDrop = 10f;
    public float damage = 1f;
    
    private bool _disposed = false;
    private PlayerHealth playerHealth;
    private StarterAssets.StarterAssetsInputs _input;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        _input = StarterAssets.StarterAssetsInputs.Instance;
    }

    private void Start()
    {
        temperatureMonitor.SetActive(false);
        ledAlarm.GetComponent<MeshRenderer>().material = ledMedium;
        StartCoroutine(SufferingCold());
        if (Difficulty.Instance != null)
            ChangeValuesByDifficulty();

        if (uiIcon != null)
            uiIcon.SetActive(false);
    }

    public void ChangeValuesByDifficulty()
    {
        /* if useDefaultValues is toggle then use default parameters */
        if (useDefaultValues)
        {
            if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
            {
                timeDrop = TIME_SUFFER_BY_COLD_EASY;
                damage = DAMAGE_SUFFER_BY_COLD_EASY;
            }
            else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
            {
                timeDrop = TIME_SUFFER_BY_COLD_MEDIUM;
                damage = DAMAGE_SUFFER_BY_COLD_MEDIUM;
            }
            else
            {
                timeDrop = TIME_SUFFER_BY_COLD_HARD;
                damage = DAMAGE_SUFFER_BY_COLD_HARD;
            }
        }
    }

    private void OnEnable()
    {
        SetTemperature(temperature);
        SetLedColor();
        enabled = true;
        _disposed = true;
        StartCoroutine(StartFlashing());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enabled = false;
        _disposed = false;
        StopCoroutine(StartFlashing());
    }

    void Update()
    {
        SetTemperature(temperature);
        PlayAudioAlarm();
    }

    public int GetTemperature()
    {
        return temperature;
    }

    public void SetTemperature(int temperature)
    {
        this.temperature = temperature;
        temperatureMonitorText.text = temperature.ToString() + "°C";
        SetLedColor();
    }

    private IEnumerator SufferingCold()
    {
        while (playerHealth.GetHealth() > 0)
        {
            if (StateMachine.Instance.IsInDialogue())
            {
                yield return new WaitWhile(() => StateMachine.Instance.IsInDialogue());
            }
            yield return new WaitForSecondsRealtime(timeDrop);
            
            if (!HotZone.isInHotZone)
                playerHealth.HitByCold(damage);
        }
        yield return new WaitForSeconds(temperature);
    }

    private void PlayAudioAlarm()
    {
        if (temperature <= DEFUALT_MIN_TEMPERATURE_SUFFER)
        {
            if (!audioAlarm.isPlaying && Time.timeScale > 0)
            {
                audioAlarm.enabled = true;
                audioAlarm.Play();

                if (!temperatureMonitor.activeSelf)
                    audioAlarm.volume = 0f;
 
            } else
            {
                if (temperatureMonitor.activeSelf)
                    audioAlarm.volume = Mathf.Lerp(audioAlarm.volume, 1f, 0.05f);
                else
                    audioAlarm.volume = Mathf.Lerp(audioAlarm.volume, 0.015f, 0.05f);
            }
        }
        else if (temperature >= DEFUALT_MIN_TEMPERATURE_SUFFER)
        {
            audioAlarm.volume = Mathf.Lerp(audioAlarm.volume, 0f, 0.05f);
        }
    }

    public void SetLedColor()
    {
        if (temperature <= DEFUALT_MIN_TEMPERATURE_SUFFER)
        {
            ledAlarm.GetComponent<MeshRenderer>().material = ledBad;
        } else if (temperature >= 0 && temperature < 15)
        {
            ledAlarm.GetComponent<MeshRenderer>().material = ledMedium;
        } else
        {
            ledAlarm.GetComponent<MeshRenderer>().material = ledGood;
        }
    }

    private IEnumerator StartFlashing()
    {
        while (_disposed)
        {
            if (!ledAlarm.GetComponent<MeshRenderer>().material.name.StartsWith(ledGood.name))
            {
                ledAlarm.SetActive(true);
                yield return new WaitForSeconds(1f);
                ledAlarm.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                ledAlarm.SetActive(true);
                yield return null;   
            }      
        }
    }

}
