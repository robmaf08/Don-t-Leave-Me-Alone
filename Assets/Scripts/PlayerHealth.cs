using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

public class PlayerHealth : MonoBehaviour, IDifficulty
{
    //Defualt drop e restore amount based on difficulty
    private const float DEFAULT_DROP_STAMINA_EASY = 0.05f;
    private const float DEFAULT_REST_STAMINA_EASY = 2f;
    private const float DEFAULT_DROP_STAMINA_MEDIUM = 0.06f;
    private const float DEFAULT_REST_STAMINA_MEDIUM = 0.08f;
    private const float DEFAULT_DROP_STAMINA_HARD = 0.098f;
    private const float DEFAULT_REST_STAMINA_HARD = 1f;

    [Header("Player Health parameters")]
    [Tooltip("Uncheck useDefaultParameters to use specific values. " +
        "If checked default values based on difficulty will be used")]
    public bool useDefaultParameters = true;
    [SerializeField] private ThirdPersonController player;
    public float health = 100;
    public float resistance = 100;
    public float maxHealth = 300f;
    public float maxResistance = 300f;
    public float chipSpeed = 2f;

    [Header("UI Components")]
    public GameObject staminaUIObj;
    public Image staminaUI;
    public Image healthUI;
    public Image hitByColdUI;
    [SerializeField] private Animator animatorStaminaUI;
    [SerializeField] private AnimationClip showStaminaClip;
    [SerializeField] private AnimationClip notShowStaminaClip;

    //Drop e restore amount for arMIN
    [HideInInspector] public float dropStaminaAmount;
    [HideInInspector] public float restoreStaminaAmount;
    [HideInInspector] public bool isIncreasing = false;
    [HideInInspector] public bool isDecreasing = false;
    [HideInInspector] public bool showOnce = true;
    [HideInInspector] public bool doOnce = true;
    [HideInInspector] static public bool superSpeedActivated = false;
    [HideInInspector] static public bool invincibiltyActivated = false;
 
    //Audio components for running and rest sounds
    private GameObject soundRun;
    private GameObject soundRest;

    [Header("Player Hurt Sound Effect")]
    public AudioSource playerHurtSource;
    public AudioClip[] hurtSounds;

    private float lastHealth;

    void Start()
    {
        soundRun = new GameObject();
        soundRest = new GameObject();
        soundRun.AddComponent<AudioSource>();
        soundRun.AddComponent<AudioBehaviour>();
        soundRest.AddComponent<AudioSource>();
        soundRest.AddComponent<AudioBehaviour>();
        soundRest.GetComponent<AudioSource>().loop = true;
        if (staminaUI != null)
            staminaUI.transform.parent.gameObject.SetActive(false);
        //health = maxHealth;
        lastHealth = maxHealth;
        resistance = maxResistance; 
        UpdateHealthUI();
        superSpeedActivated = false;
        invincibiltyActivated=false;
        ChangeValuesByDifficulty();
    }

    public void ChangeValuesByDifficulty()
    {
        if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
        {
            dropStaminaAmount = DEFAULT_DROP_STAMINA_EASY;
            restoreStaminaAmount = DEFAULT_REST_STAMINA_EASY;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
        {
            dropStaminaAmount = DEFAULT_DROP_STAMINA_MEDIUM;
            restoreStaminaAmount = DEFAULT_REST_STAMINA_MEDIUM;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Hard)
        {
            dropStaminaAmount = DEFAULT_DROP_STAMINA_HARD;
            restoreStaminaAmount = DEFAULT_REST_STAMINA_HARD;
        }
    }

    public float GetDropStaminaAmount()
    {
        return dropStaminaAmount;
    }

    public float GetRestoreStaminaAmount()
    {
        return restoreStaminaAmount;
    }

    public float GetHealth() 
    {
        return health;
    }

    public float GetResistance()
    {
        return resistance;
    }

    void Update()
    {
        if (player.enabled && Time.timeScale == 1f)
        {
            UpdateStaminaUI();
            UpdateHealthUI();
            PlayHeartBeat();

            if (resistance >= maxResistance 
                && !isIncreasing 
                && player.GetCurrentSpeed() == 0
                && doOnce)
            {
                StartCoroutine(ShowStamina(false));
            } 

            if (resistance < maxResistance)
            {
                if (showOnce)
                    StartCoroutine(ShowStamina(true));
            }

            if (resistance >= 85f)
            {
                if (soundRest.GetComponent<AudioSource>().isPlaying)
                {
                    soundRest.GetComponent<AudioSource>().Stop();
                    soundRest.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Player/BreathStop"));
                }
            }
            if (health <= 50f && health < lastHealth)
            {
                lastHealth = health;
            }
        }
    }
 
    public void UpdateStaminaUI()
    {
        if (staminaUI != null)
        {
            if (!isIncreasing)
                staminaUI.fillAmount = resistance / maxResistance;
            else
                staminaUI.fillAmount = resistance / maxResistance;
        }
    }

    public void UpdateHealthUI() 
    {
        if (healthUI != null)  healthUI.fillAmount = health / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!invincibiltyActivated)
        {
            health -= damage;
            if (health < 0)
                health = 0;

            //Play audio player hurt casually
            playerHurtSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length - 1)]);
        }
    }
    
    public void HitByCold(float damage) 
    {
        if (!invincibiltyActivated)
        {
            health -= damage;
            if (health < 0)
                health = 0;

            //Play audio player hurt casually
            if (hurtSounds.Length > 0)
                playerHurtSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length - 1)]);

            StopAllCoroutines();
            StartCoroutine(ColdHit());
        }
   
    }

    private IEnumerator ColdHit() 
    {
        if (hitByColdUI != null)
        {
            hitByColdUI.gameObject.SetActive(true);

            for (float i = 0f; i <= 1f; i += 0.05f)
            {
                hitByColdUI.color = new Color(hitByColdUI.color.r, hitByColdUI.color.g, hitByColdUI.color.b, i);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(1f);
            for (float i = 1f; i >= 0f; i -= 0.02f)
            {
                hitByColdUI.color = new Color(hitByColdUI.color.r, hitByColdUI.color.g, hitByColdUI.color.b, i);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForEndOfFrame();
            hitByColdUI.gameObject.SetActive(false);
        }
    }

    public void ReduceResistance(float reduce)
    {
        if (!superSpeedActivated)
        {
            if (soundRest.GetComponent<AudioSource>().isPlaying)
                soundRest.GetComponent<AudioSource>().Stop();

            resistance -= reduce;
            isIncreasing = false;
            isDecreasing = true;
            if (resistance <= 0)
            {
                resistance = 0;
            }
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void RestoreResistance(float healAmount)
    {
        if (!soundRest.GetComponent<AudioSource>().isPlaying && resistance < 80f)
        {
            soundRest.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Player/BreathRest"));
            soundRest.GetComponent<AudioSource>().loop = true;
        }
        resistance += healAmount;
        isIncreasing = true;
        isDecreasing = false;
        if (resistance >= maxResistance)
        {
            resistance = maxResistance;
            isIncreasing = false;
        }
    }

    private IEnumerator ShowStamina(bool show)
    {
        if (staminaUI != null)
        {
            Image parent = staminaUI.transform.parent.GetComponent<Image>();
            if (show)
            {
                showOnce = false;
                doOnce = true;
                staminaUI.transform.parent.gameObject.SetActive(true);
                for (float i = 0f; i <= 1f; i += 0.05f)
                {
                    staminaUI.color = new Color(staminaUI.color.r, staminaUI.color.g, staminaUI.color.b, i);
                    staminaUI.transform.parent.gameObject.GetComponent<Image>().color = 
                        new Color(parent.color.r, parent.color.g, parent.color.b, i);
                    yield return new WaitForSeconds(0.01f);
                }
                staminaUI.color = new Color(staminaUI.color.r, staminaUI.color.g, staminaUI.color.b, 255);
                staminaUI.transform.parent.gameObject.GetComponent<Image>().color = new Color(parent.color.r, parent.color.g, parent.color.b, 255);

            }
            else
            {
                for (float i = 1f; i >= 0f; i -= 0.05f)
                {
                    staminaUI.color = new Color(staminaUI.color.r, staminaUI.color.g, staminaUI.color.b, i);
                    staminaUI.transform.parent.gameObject.GetComponent<Image>().color = 
                        new Color(parent.color.r, parent.color.g, parent.color.b, i);
                    yield return new WaitForSeconds(0.01f);
                }
                staminaUI.color = new Color(staminaUI.color.r, staminaUI.color.g, staminaUI.color.b, 0);
                staminaUI.transform.parent.gameObject.GetComponent<Image>().color = 
                        new Color(parent.color.r, parent.color.g, parent.color.b, 0);

                staminaUI.transform.parent.gameObject.SetActive(false);
                showOnce = true;
                doOnce = false;
            }
        }
    }

    private void PlayHeartBeat()
    {
        if (resistance <= 90f
                && !soundRun.GetComponent<AudioSource>().isPlaying
                && !soundRest.GetComponent<AudioSource>().isPlaying)
        {
            if (soundRun != null && !soundRun.GetComponent<AudioSource>().isPlaying)
                soundRun.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Player/HeartBeat"));
        }

        if (resistance >= 80f && resistance <= 90f)
            soundRun.GetComponent<AudioSource>().volume = 0.1f;
        else if (resistance >= 70f && resistance < 80f)
            soundRun.GetComponent<AudioSource>().volume = 0.3f;
        else if (resistance >= 60f && resistance < 70f)
            soundRun.GetComponent<AudioSource>().volume = 0.5f;
        else if (resistance >= 40f && resistance < 60f)
            soundRun.GetComponent<AudioSource>().volume = 0.7f;
        else 
            soundRun.GetComponent<AudioSource>().volume = 1f;

    }

}
