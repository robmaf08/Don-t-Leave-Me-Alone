using UnityEngine;
using UnityEngine.Events;

public class TriggerZoneEvent : MonoBehaviour
{

    public UnityEvent OnTriggerEnterEvent;

    public UnityEvent OnTriggerStayEvent;

    public UnityEvent OnTriggerExitEvent;

    public bool destroyAfterEvent = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnTriggerEnterEvent.Invoke();
            if (destroyAfterEvent)
                Destroy(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            OnTriggerStayEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            OnTriggerExitEvent.Invoke();
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position  , GetComponent<Collider>().bounds.size);
    }
}
