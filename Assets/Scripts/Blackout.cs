using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour
{
    public static Blackout instance;

    [Header("Light/s that are excluded from blackout")]
    [SerializeField] private Light[] lightExcluded;

    public bool isBlackoutOn = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start() 
    {
        isBlackoutOn = false;
    }   

    public void StartBlackout()
    {
        if (!isBlackoutOn)
        {
            isBlackoutOn = true;
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light l in lights)
            {
                bool isExcluded = false;
                foreach (Light l2 in lightExcluded)
                {
                    if (l2 == l)
                    {
                        isExcluded = true;
                        break;
                    }
                }
                if (!isExcluded)
                    l.gameObject.SetActive(false);
            }
        }
    }

    public void Restore()
    {
        if (isBlackoutOn)
        {
            isBlackoutOn = false;
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light l in lights)
            {
                bool isExcluded = false;
                if (!isExcluded)
                    l.enabled = true;
            }
        }
    }


}
