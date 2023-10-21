using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : StoreItem
{
    [Header("Battery Components")]
    public Torch flashLight;
    public float amountReload;

    private void Start() 
    {
        descriptionItem = descriptionItem 
            + "\nAumento energia: " 
            + amountReload + "%.";   
    }

    public override string UseItem()
    {
        base.UseItem();
        string message = string.Empty;
        if (flashLight != null)
        {
            if (flashLight.gameObject.activeSelf)
            {
                if (flashLight.batteryCapacity < flashLight.maxBatteryCapacity)
                {
                    flashLight.AddCapacity(amountReload);
                }
                else
                {
                    message = "Le batterie della torcia sono già al massimo!";
                }
            }
            else
            {
                message = "Nessuna torcia equipaggiata.";
            }
        }
        return message;
    }

}
