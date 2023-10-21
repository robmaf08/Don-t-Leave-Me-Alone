using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public static class InputHandler
{
    public static bool isGamepadConnected = false;

    public enum InputScheme
    {
        Keyboard,
        Playstation,
        Xbox
    }

    public static InputScheme currentInputScheme = InputScheme.Keyboard;

    public static void OnApplicationClose() 
    {
        isGamepadConnected = false;

        //Set Keyboard as default scheme
        currentInputScheme = InputScheme.Keyboard;
    }

    public static void OnGamepadDisconnect() 
    {
        currentInputScheme = InputScheme.Keyboard;
        isGamepadConnected = false;
    }

}
