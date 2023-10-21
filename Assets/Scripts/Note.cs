using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : StoreItem
{
    
    public string title;
    
    [TextArea(1,1000)] public string description;
    
    public Sprite image;
}
