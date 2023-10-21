using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scream : MonoBehaviour
{
    [Header("Components")]
    public GameObject uiScream;
    public Animator _animatorHand;

    [Header("Audio Components")]
    public AudioSource screamAudioSource;
    public AudioClip screamClip;

    [Header("VFX components")]
    public ParticleSystem rippleEffect;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _vCamera;
    
    [Header("Item equipped")]
    public EquipItem playerEquip;
    
    private bool canDamage = true;
    private bool isScreaming = false;
    private StarterAssets.StarterAssetsInputs _input;

    private void OnEnable() 
    {
        if (_input != null)
            _input.Reset(); 
    }

    void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        uiScream.SetActive(false); 
        _animatorHand.gameObject.SetActive(false);
        Physics.IgnoreCollision(GetComponent<Collider>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>());
    }

    void Update()
    {
        if (!EnemyFollow.Instance.isFollowing)
        {
            if (uiScream.activeSelf)
                uiScream.SetActive(false);
        }
    }

    private IEnumerator StartScream(Enemy enemy)
    {
        _input.Reset(); 
        _animatorHand.gameObject.SetActive(true);

        //Player can't shoot and can't make another scream
        canDamage = false;
        uiScream.SetActive(false);
        isScreaming = true;
        if (StateMachine.Instance != null)
            StateMachine.Instance.canEquipItem = false;

        //Unequip current right arm holding item
        playerEquip.NotEquipAnything();
        yield return new WaitForSeconds(1f);
        
        //Play shockwave ripple fx
        rippleEffect.Play();
        _animatorHand.gameObject.SetActive(true);  
        _animatorHand.SetBool("Scream", true);
        
        //Attack Enemy
        enemy.TakeDamage(20);
        canDamage = false;
        screamAudioSource.PlayOneShot(screamClip);
        
        //Add noise to camera to simulate vibration
        float frequency = _vCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
        _vCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 20;
        yield return new WaitForSecondsRealtime(screamClip.length);
        rippleEffect.Stop();
        _vCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;

        //Scream finished
        canDamage = true;
        _animatorHand.SetBool("Scream", false);
        yield return new WaitForSeconds(1f);
        _animatorHand.gameObject.SetActive(false);
        if (StateMachine.Instance != null)
            StateMachine.Instance.canEquipItem = true;

        //Equip previous item
        playerEquip.EquipLastItem();
        canDamage = true;

        isScreaming = false;
        _animatorHand.gameObject.SetActive(false);
        _input.Reset();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.GetComponent<Enemy>() != null)
            {
                if (!enabled)
                    enabled = true;

                if (!isScreaming)
                {
                    //Show player can make a scream
                    if (!uiScream.activeSelf)
                        uiScream.SetActive(true);

                    //If player can shoot and press R then shoot
                    if (canDamage)
                    {
                        if (_input.scream && canDamage)
                        {
                            _input.Reset();
                            StartCoroutine(StartScream(other.gameObject.GetComponent<Enemy>()));
                            return;
                            
                        }
                    }
                }
            } else
            {
                if (uiScream.activeSelf)
                    uiScream.SetActive(false);

                enabled = false;
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (uiScream.activeSelf) uiScream.SetActive(false);
            enabled = false;
    }

}
