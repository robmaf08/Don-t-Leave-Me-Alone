using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdZone : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TemperatureMonitor temperatureMonitor;

    [Header("Zone Properties")]
    public int temperature;
    public float healthDrop = 0.05f;

    [Header("Is inside a Hot-Zone")]
    public HotZone hotZone;

    private int lastTemperature = 0;
    public float timeToDrop = 2f;
    private bool canDrop = true;

    private void OnTriggerEnter(Collider other)
    {
        /* When the player enters in a Cold-Zone we set the 
         * monitor temperature */
        if (other != null && other.gameObject.tag == "Player")
        {
            temperatureMonitor.SetTemperature(temperature);
            if (hotZone != null)
                EnableHotZone(false);

            Shivering.instance.EnterColdZone(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            //Enemy is attacking player then cold zone stop working for a minute
            if (EnemyFollow.Instance.isFollowing)
            {
                StartCoroutine(StopTemporaryColdSuffer());
            } else
            {
                if (canDrop)
                {
                    StartCoroutine(ReduceHealth());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /* When the player gets out of the Cold-Zone 
         * we set the last temperature (this because user
         * can enter a hot-zone in a cold-zone) */
        if (other != null && other.gameObject.tag == "Player")
        {
            temperatureMonitor.SetTemperature(lastTemperature);
            if (hotZone != null)
                EnableHotZone(true);
        }
        Shivering.instance.EnterColdZone(false);
    }

    private void EnableHotZone(bool enable)
    {
        hotZone.GetComponent<Collider>().enabled = enable;
    }

    private IEnumerator ReduceHealth()
    {
        var timer = 0.0f;
        canDrop = false;
        while (timer < timeToDrop)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        playerHealth.TakeDamage(healthDrop);
        canDrop = true;
    }

    private IEnumerator StopTemporaryColdSuffer()
    {
        /* If the player is in Cold-Zone and is fighting, he will not
         * suffer cold anymore */
        if (EnemyFollow.Instance != null && EnemyFollow.Instance.isFollowing)
        {
            canDrop = false;
            //Wait until fighting is over
            while (EnemyFollow.Instance.isFollowing)
            {
                yield return null;
            }

            //Fight is over and wait 50 sec to restart suffering cold
            yield return new WaitForSecondsRealtime(50f);
            canDrop = true;
        } else
        {
            StopCoroutine(StopTemporaryColdSuffer());   
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(gameObject.GetComponent<Collider>().bounds.center, gameObject.GetComponent<Collider>().bounds.size);
        Gizmos.DrawIcon(GetComponent<Collider>().bounds.center, "ColdZone_Gizmos", true);
    }

}
