using System.Collections;
using UnityEngine;
using TMPro;

public class FoundHiiddenPlace : InteractObject
{
    [SerializeField] private TextMeshProUGUI TextGUI;
    [SerializeField] private TextMeshProUGUI SingleActionText;
    [SerializeField] private TextMeshProUGUI SingleActionKey;

    public override void OnInteract()
    {
        if (KillerFrostEntrance.canHide && !KillerFrostEntrance.isHidden)
        {
            KillerFrostEntrance.isHidden = true;
            Destroy(this);
        }
    }

}
