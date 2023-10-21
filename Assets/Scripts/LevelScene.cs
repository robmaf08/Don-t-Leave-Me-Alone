using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/LevelScene", order = 3)]
public class LevelScene : ScriptableObject
{
    public int idLevel;
    
    public string nameLevel;

    [TextArea] public string descriptionLevel;

    public Sprite imageLevel;

}
