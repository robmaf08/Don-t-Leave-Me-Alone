using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    
    public static Difficulty Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDifficulty(LevelDifficulty.Difficulty diff)
    {
        LevelDifficulty.difficulty = diff;
    }

    public LevelDifficulty.Difficulty GetDifficulty()
    {
        return LevelDifficulty.difficulty;
    }

}
