using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Campfire : MonoBehaviour, IDifficulty
{

    private float damagePlayer = 3f;
    private float damageEnemy = 3f;

    private const float DAMAGE_PLAYER_EASY = 1.5f;
    private const float DAMAGE_PLAYER_MEDIUM = 2.5f;
    private const float DAMAGE_PLAYER_HARD = 4.75f;
    private const float DAMAGE_ENEMY_EASY = 3.25f;
    private const float DAMAGE_ENEMY_MEDIUM = 5.75f;
    private const float DAMAGE_ENEMY_HARD = 7.95f;

    private void Start () 
    {
       ChangeValuesByDifficulty();
    }
 
    public void ChangeValuesByDifficulty() 
    {
        if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
        {
            damagePlayer = DAMAGE_PLAYER_EASY;
            damageEnemy = DAMAGE_ENEMY_EASY;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
        {
            damagePlayer = DAMAGE_PLAYER_MEDIUM;
            damageEnemy = DAMAGE_ENEMY_MEDIUM;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Hard)
        {
            damagePlayer = DAMAGE_PLAYER_HARD;
            damageEnemy = DAMAGE_ENEMY_HARD;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.activeSelf)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damagePlayer);
                EnemyFollow.Instance.OnEnemyAttack();
            }

            if (other.gameObject.GetComponent<Enemy>() != null)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageEnemy);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.activeSelf)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damagePlayer * 0.07f);
                EnemyFollow.Instance.OnEnemyAttack();
            }

            if (other.gameObject.GetComponent<Enemy>() != null)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(damageEnemy);
            }
        }
    }

}
