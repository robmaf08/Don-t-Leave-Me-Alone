using System.Collections;
using UnityEngine;
using TMPro;

public class HotZone : MonoBehaviour
{
    [HideInInspector] public static bool isInHotZone = false;

    [Header("Components")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TemperatureMonitor temperatureMonitor;

    [Header("Zone properties")]
    public int temperature;
    public float healthGain = 0.01f;
    public float timeToRecover = 2f;

    [Header("Is Insize a Cold-Zone")]
    public ColdZone coldZone;

    private int lastTemperature = 0; //default value
    private bool canRestore = true;

    private void Start()
    {
        isInHotZone = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            isInHotZone = true;
            temperatureMonitor.SetTemperature(temperature);
            if (coldZone != null)
                EnableColdZone(false);

            Shivering.instance.EnterHotZone(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //If the player enter the trigger zone then health starts to increase
        if (other != null && other.gameObject.tag == "Player")
        {
            if (canRestore)
            {
                StartCoroutine(RestoreHealth());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            temperatureMonitor.SetTemperature(lastTemperature);
            isInHotZone = false;
            if (coldZone != null)
                EnableColdZone(true);

            Shivering.instance.EnterHotZone(false);
        }
    }
    private void EnableColdZone(bool enable) 
    {
        coldZone.GetComponent<Collider>().enabled = enable;
    }

    private IEnumerator RestoreHealth()
    {
        var timer = 0.0f;
        canRestore = false;
        while (timer < timeToRecover)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        playerHealth.RestoreHealth(healthGain);
        canRestore = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 127, 0, 200);
        Gizmos.DrawWireCube(gameObject.GetComponent<Collider>().bounds.center, gameObject.GetComponent<Collider>().bounds.size);
        Gizmos.DrawIcon(GetComponent<Collider>().bounds.center, "HotZone_Gizmos", true);
    }
}
