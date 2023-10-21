using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    //Saves a screenshot when a button is pressed
    public KeyCode screenShotButton;
    public string screenshotName;

    void Update()
    {
        if (Input.GetKeyDown(screenShotButton))
        {
            ScreenCapture.CaptureScreenshot(screenshotName+".png");
            Debug.Log("A screenshot was taken!");
        }
    }
}
