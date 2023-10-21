using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 4)]
public class EnemyParameters : ScriptableObject
{

    [Header("Parameters on EASY difficulty")]
    public float healthEasy;
    public float maxHealthEasy;
    public float speedEasy;
    public float accelerationEasy;
    public float damagePlayerEasy;
    public float damageByGunEasy;
    public float dameageByScreamEasy;


    [Header("Parameters on MEDIUM difficulty")]
    public float healthMedium;
    public float maxHealthMedium;
    public float speedMedium;
    public float accelerationMedium;
    public float damagePlayerMedium;
    public float damageByGunMedium;
    public float dameageByScreamMedium;

    [Header("Parameters on HARD difficulty")]
    public float healthHard;
    public float maxHealthHard;
    public float speedHard;
    public float accelerationHard;
    public float damagePlayerHard;
    public float damageByGunHard;
    public float dameageByScreamHard;

  
}
