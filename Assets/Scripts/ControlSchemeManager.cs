using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ControlSchemeManager : MonoBehaviour 
{
    public static ControlSchemeManager instance;
    public bool gamepadConnected = false;

    public Image layoutCurrentScheme;
    public Sprite layoutKeyboard;
    public Sprite layoutPs4;
    public Sprite layoutXbox;
    public PlayerInput playerInput;

    private void Awake() 
    {
        gamepadConnected = false;
        instance = this;
    }


    private void OnEnable() 
    {
        ChangeControlScheme();
    }

    private void OnDisable() 
    {
        gamepadConnected = false;
    }

    private void OnApplicationQuit() 
    {
        InputHandler.OnApplicationClose();
    }

    private void Start() 
    {
        if (!InputHandler.isGamepadConnected)
            CheckGamepadConnected();
        else
            ChangeControlScheme();
    }

    void Update() 
    {
        if (!InputHandler.isGamepadConnected)
            CheckGamepadConnected();
    }

    public void ChangeControlScheme() 
    {
        //Check if Gamepad is connected
        if (InputHandler.isGamepadConnected)
        {
            if (InputHandler.currentInputScheme == InputHandler.InputScheme.Playstation)
                ChangeToPs4Icon();
            else if(InputHandler.currentInputScheme == InputHandler.InputScheme.Xbox)
                ChangeToXboxIcon();
        }
        else
        {
            //If Gamepad is not conneted then use Defualt Keyboard Input Layout
            ChangeToKeyboardIcon();
            InputHandler.isGamepadConnected = false;
        }
    }

    public void ChangeControlScheme(int type)
    {
        //Check if Gamepad is connected
        if (InputHandler.isGamepadConnected)
        {
            InputHandler.currentInputScheme = (InputHandler.InputScheme) type;

            if (InputHandler.currentInputScheme == InputHandler.InputScheme.Playstation)
                ChangeToPs4Icon();
            else if (InputHandler.currentInputScheme == InputHandler.InputScheme.Xbox)
                ChangeToXboxIcon();
            else if (InputHandler.currentInputScheme == InputHandler.InputScheme.Keyboard)
                ChangeToKeyboardIcon();

        } else
        {
            ChangeToKeyboardIcon();
        }
    }

    public void ChangeToKeyboardIcon() 
    {
        CommandIcon[] commands = FindObjectsOfType<CommandIcon>(true);
        foreach (CommandIcon command in commands)
        {
            command.GetComponent<Image>().sprite = command.keyboardIcon;
        }

        if (layoutCurrentScheme != null)
            layoutCurrentScheme.sprite = layoutKeyboard;

        InputHandler.isGamepadConnected = false;
        InputHandler.currentInputScheme = InputHandler.InputScheme.Keyboard;
    }

    public void ChangeToPs4Icon()
    {
        //Search for all buttons that implemets Class CommandIcon
        CommandIcon[] commands = FindObjectsOfType<CommandIcon>(true);
        foreach (CommandIcon command in commands)
        {
            command.GetComponent<Image>().sprite = command.ps4Icon;
        }

        if (layoutCurrentScheme != null)
            layoutCurrentScheme.sprite = layoutPs4;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

       // typeInput = ControlSchemeType.Ps4;

        InputHandler.isGamepadConnected = true;
        InputHandler.currentInputScheme = InputHandler.InputScheme.Playstation;
    }

    public void ChangeToXboxIcon() 
    {
        //Search for all buttons that implemets Class CommandIcon
        CommandIcon[] commands = FindObjectsOfType<CommandIcon>(true);
        foreach (CommandIcon command in commands)
        {
            command.GetComponent<Image>().sprite = command.xboxIcon;
        }

        if (layoutCurrentScheme != null)
            layoutCurrentScheme.sprite = layoutXbox;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

       // type = ControlSchemeType.Xbox;

        InputHandler.isGamepadConnected = true;
        InputHandler.currentInputScheme = InputHandler.InputScheme.Xbox;

    }

    public void CheckGamepadConnected()
    {
        if (Gamepad.current != null &&
            Gamepad.current.device.wasUpdatedThisFrame)
            InputHandler.isGamepadConnected = true;
        else
            InputHandler.OnGamepadDisconnect();

        InputSystem.onDeviceChange +=
         (device, change) =>
         {
             switch (change)
             {
                 case InputDeviceChange.Added:
                     // New Device.
                     Cursor.visible = false;
                     Cursor.lockState = CursorLockMode.Locked;
                     gamepadConnected = true;
                     InputHandler.isGamepadConnected = true;
                     break;
                 case InputDeviceChange.Disconnected:
                     Cursor.visible = true;
                     Cursor.lockState = CursorLockMode.None;
                     gamepadConnected = false;
                     InputHandler.OnGamepadDisconnect();
                     // Device got unplugged.
                     break;
                 case InputDeviceChange.Removed:
                     gamepadConnected = false;
                     InputHandler.OnGamepadDisconnect();
                     // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                     break;
                 case InputDeviceChange.ConfigurationChanged:
                     gamepadConnected = false;
                     InputHandler.OnGamepadDisconnect();
                     // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                     break;
                 case InputDeviceChange.Reconnected:
                     gamepadConnected = true;
                     InputHandler.isGamepadConnected = true;
                     break;
                 default:
                     // See InputDeviceChange reference for other event types.
                     break;
             }
         };
    }

}
