using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Goals", menuName = "ScriptableObjects/Goal", order = 2)]
public class Goal : ScriptableObject
{
    [TextArea] public string nameGoal;

    public bool needObject;

    public GameObject Object;

    public bool activateOnTriggerZone;

    public bool completed;
}
