using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class SnowFlakeManager : MonoBehaviour
{

    public enum PowerUpType 
    {
         SuperSpeed,
         Invincible,
         SuperJump
    }

    public PowerUpType powerUpType;

    [SerializeField] private Vector3 Rotation;
    [SerializeField] float Speed;
    public float timeDuration = 10f;

    void Update()
    {
        transform.Rotate(Rotation * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlaySoundPowerUp();
            if (powerUpType == PowerUpType.SuperSpeed)
            {
                PlayerHealth.superSpeedActivated = true;
                StartCoroutine(UseSuperSpeed());
                GetComponent<Collider>().enabled = false;
                GetComponent<MeshRenderer>().enabled = false;
                //gameObject.SetActive(false);
                return;
            } else if (powerUpType == PowerUpType.Invincible)
            {
                PlayerHealth.invincibiltyActivated = true;
                StartCoroutine(UseInvicibility());
                GetComponent<Collider>().enabled = false;
                GetComponent<MeshRenderer>().enabled = false;
                return;
            }
        }
    }

    private void PlaySoundPowerUp()
    {
        GameObject audio = new GameObject();
        audio.AddComponent<AudioSource>();
        audio.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/CollectSnowflake"));
        Destroy(audio, Resources.Load<AudioClip>("Audio/CollectSnowflake").length);
    }


    private IEnumerator UseSuperSpeed() 
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().resistance = 100;
        float originalSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>().SprintSpeed;
        GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>().SprintSpeed = 26.3f;
        yield return new WaitForSeconds(timeDuration);
        GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>().SprintSpeed = originalSpeed;
        PlayerHealth.superSpeedActivated = false;
        Destroy(gameObject);
    }

    private IEnumerator UseInvicibility()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health = 100;
        yield return new WaitForSeconds(timeDuration);
        PlayerHealth.invincibiltyActivated = false;
        Destroy(gameObject);
    }
}
