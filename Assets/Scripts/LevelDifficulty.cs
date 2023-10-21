using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelDifficulty
{
    public enum Difficulty 
    {
        Easy,
        Medium, 
        Hard
    }

    /* Setting easy difficulty as default */
    public static Difficulty difficulty = Difficulty.Medium;
}
