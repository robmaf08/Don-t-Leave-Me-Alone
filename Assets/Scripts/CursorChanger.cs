using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.ForceSoftware;
    private Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        Cursor.visible = false;
    }

    void OnMouseEnter()
    {
        if (gameObject.activeSelf)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
       
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
