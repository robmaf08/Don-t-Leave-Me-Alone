using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDifficulty
{
    [Header("Enemy properties")]
    public EnemyParameters parameters;
    
    [Header("Enemy Components")]
    public Animator enemyAnimator;
    public Image enemyHealthBar;
    public NavMeshAgent enemy;

    [Header("Death Information")]
    public GameObject deathVFX;
    public AudioClip deathAudio;

    [Header("Audio Enemy")]
    //private GameObject enemyAudio;
    public AudioClip runningAudio;
    public AudioClip attackAudio;

    public Transform player;

    //Enemy parameters
    [HideInInspector] public float maxHealth = 60f;
    [HideInInspector] public float health = 60f;
    [HideInInspector] public float damageToPlayer = 60f;
    [HideInInspector] public float damageByGun = 10f;
    [HideInInspector] public float damageByScream = 20f;
    private Vector3 lastPosition;


    public bool isFollowingPlayer = false;
    private bool isAttacking = false;
    private bool isGettingStucked = false;
    private bool wasStucked = false;
    private bool canAttack = true;
    private float lastAttackTime = 0;
    

    void Start()
    {
        enemyAnimator.Play("Idle");
        enemyHealthBar.fillAmount = health / maxHealth;
        enemyHealthBar.transform.parent.gameObject.SetActive(false);
        ChangeValuesByDifficulty();
        lastPosition = enemy.transform.position;
    }

    private void OnEnable()
    {
        enemyAnimator.Play("Idle");
    }

    public void ChangeValuesByDifficulty()
    {
        //Choose parameters to assign
        float health, maxHealth, speed, acceleration, damageGun, damageScream, damageToPlayer;
        if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
        {
            health = parameters.healthEasy;
            maxHealth = parameters.maxHealthEasy;
            speed = parameters.speedEasy;
            acceleration = parameters.accelerationEasy;
            damageToPlayer = parameters.damagePlayerEasy;
            damageGun = parameters.damageByGunEasy;
            damageScream = parameters.dameageByScreamEasy;
        } else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
        {
            health = parameters.healthMedium;
            maxHealth = parameters.maxHealthMedium;
            speed = parameters.speedMedium;
            acceleration = parameters.accelerationMedium;
            damageToPlayer = parameters.damagePlayerMedium;
            damageGun = parameters.damageByGunMedium;
            damageScream = parameters.dameageByScreamMedium;
        } else
        {
            health = parameters.healthHard;
            maxHealth = parameters.maxHealthHard;
            speed = parameters.speedHard;
            acceleration = parameters.accelerationHard;
            damageToPlayer = parameters.damagePlayerHard;
            damageGun = parameters.damageByGunHard;
            damageScream = parameters.dameageByScreamHard;
        }

        //Setting parameters
        this.health = health;
        this.maxHealth = maxHealth;
        this.damageToPlayer = damageToPlayer;
        enemy.speed = speed;
        enemy.acceleration = acceleration;
        damageByGun = damageGun;
        damageByScream = damageScream;
    }


    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health < 0)
        {
            Die();
            return;
        }

        if (!isFollowingPlayer)
        {
            //isPlayerAttacking = true;
            isFollowingPlayer = true;
            
            //Enemy starts following player if player hits it
            StartFollowPlayer();
        }
        enemyHealthBar.fillAmount = health / maxHealth;
        //Debug.Log("Enemy health: " + health);
    }

    private void Die() 
    {
        //Play death sound
        GameObject deathSound = new GameObject();
        deathSound.AddComponent<AudioSource>();
        deathSound.GetComponent<AudioSource>().PlayOneShot(deathAudio);

        //Play explosion vfx
        isFollowingPlayer = false;
        EnemyFollow.Instance.OnNotFollow();
        deathVFX.transform.position = transform.position;
        deathVFX.GetComponent<ParticleSystem>().Play();
        StateMachine.Instance.isFighting = false;
    
        //Reactivate Raycast
        Raycast.Instance.RaycastEnable(true);
        StateMachine.Instance.isFighting = false;

        Destroy(gameObject);
    }

    void Update()
    {
        if (!isFollowingPlayer)
        {
            if (!wasStucked)
            {
                if (DistanceFromPlayer() < 20f && canAttack)
                    StartFollowPlayer();
            }
            else
            {
                if (GetComponent<EnemyMovement>() == null)
                {
                    if (DistanceFromPlayer() < 7f && canAttack)
                        StartFollowPlayer();
                } else
                {
                    if (DistanceFromPlayer() < 20f && canAttack)
                        StartFollowPlayer();
                }
            }
        } else
        {
            if (DistanceFromPlayer() <= 100f)
            {
                FollowPlayer();
                transform.LookAt(player.position);
            } else
            {
                StopFollowPlayer();
            }
        }
    }

    private void StartFollowPlayer()
    {
        StopAllCoroutines();
        //Disable following path if any
        if (GetComponent<EnemyMovement>() != null)
        {
            GetComponent<EnemyMovement>().StopFollowingPath();
        }
      
        //Enemy starts running toward player
        isFollowingPlayer = true;
        isGettingStucked = false;
        wasStucked = false;

        //Stop enemy moving and Reset Path
        enemyAnimator.Rebind();
        enemy.isStopped = true;
        enemy.ResetPath();

        //Show Health Bar enemy
        enemyHealthBar.transform.parent.gameObject.SetActive(true);

        //Start follow player
        enemyAnimator.Play("Run");
        FollowPlayer();
        EnemyFollow.Instance.OnFollow();
        StateMachine.Instance.isFighting = true;

        StartCoroutine(SurrenderAfterTime());
    }

    private void StopFollowPlayer() 
    {
        isFollowingPlayer = false;
        EnemyFollow.Instance.OnNotFollow();
        StopAllCoroutines();
        StartCoroutine(StopAttackForSeconds(5));
        StartCoroutine(StopMoving());
        StateMachine.Instance.isFighting = false;
    }

    private void FollowPlayer() 
    {
        if (isFollowingPlayer)
        {
            enemy.SetDestination(player.transform.position);
            if (!StateMachine.Instance.isFighting)
                StateMachine.Instance.isFighting = true;

            if (enemy.transform.position != lastPosition)
            {
                lastPosition = enemy.transform.position;
                StopCoroutine(GetStucked());
            } else
            {
                StartCoroutine(GetStucked());
            }
        }
    }

    private IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(0.35f);
        //Stop enemy moving and Reset Path
        enemyAnimator.Rebind();
        enemy.isStopped = true;
        enemy.ResetPath();
        enemyAnimator.Play("Idle");
        isFollowingPlayer = false;

        if (GetComponent<EnemyMovement>() != null)
            GetComponent<EnemyMovement>().enabled = true;

    }

    private IEnumerator Attack() 
    {
        isAttacking = true;
        EnemyFollow.Instance.OnEnemyAttack();
        enemyAnimator.Play("Attack");
        enemy.isStopped = true;
        GameObject audioAttack = new GameObject();
        audioAttack.AddComponent<AudioSource>();
        audioAttack.GetComponent<AudioSource>().PlayOneShot(attackAudio);
        Destroy(audioAttack, attackAudio.length);
        yield return new WaitForSeconds(1.5f);
        enemy.isStopped = false;
        enemyAnimator.Rebind();
        enemyAnimator.Play("Run");
        isAttacking = false;
        lastAttackTime = Time.time;
        //StopAllCoroutines();
        StartCoroutine(SurrenderAfterAttack());
        //Debug.Log("Last-Attack-Time: " + lastAttackTime);   
    }

    private IEnumerator SurrenderAfterAttack() 
    {
        /* if player hides somewhere or enemy can't get close to him 
        in specific amount a time after having attacked,
        then surrend to follow */
        float timeToSurrend = 40f;
        while (Vector3.Distance(enemy.transform.position, player.transform.position) <= 100f)
        {
            //StartCoroutine
            yield return new WaitUntil(() => Vector3.Distance(enemy.transform.position, player.transform.position) <= 50f);
            yield return new WaitWhile(() => ((Time.time - lastAttackTime) < timeToSurrend) 
                && Vector3.Distance(enemy.transform.position, player.transform.position) >= 50f);
        }

        if (Time.time - lastAttackTime > timeToSurrend)
        {
            //Debug.Log("Enemy surrender after attack: ");
            StopFollowPlayer();
        }
    }

    private IEnumerator GetStucked() 
    {
        float timeToSurrend = 10f;
        if (!isGettingStucked)
        {
            isGettingStucked = true;
            Vector3 startDistance = enemy.transform.position;
            float timer = 0f;
            while (timer < timeToSurrend)
            {
                timer++;
                //Debug.Log("Timer stucked: " + timer);
                yield return new WaitForSeconds(1f);
                if (enemy.transform.position != startDistance)
                    break;
            }
            if (enemy.transform.position == startDistance)
            {
                //Debug.Log("Was stucked");
                wasStucked = true;
                isGettingStucked = false;
                StopFollowPlayer();
            }
            else
            {
                isGettingStucked = false;
            }
        }
    }

    private IEnumerator SurrenderAfterTime() 
    {
        /* If the enemy can get close to player or his distance became longer, 
         * he will surrender after a specific amount of time */
        float timeToSurrend = 20f;
        while (Vector3.Distance(enemy.transform.position, player.transform.position) <= 100f)
        {
            yield return new WaitUntil(() => Vector3.Distance(enemy.transform.position, player.transform.position) <= 50f);
            float time = 0f;
            while (time < timeToSurrend
                && Vector3.Distance(enemy.transform.position, player.transform.position) >= 50f)
            {
                time++;
                //Debug.Log("Time by t: " + time);
                yield return new WaitForSeconds(1f);
            }
            if (time >= timeToSurrend)
            {
                //Debug.Log("Enemy surrender by time: ");
                StopFollowPlayer();
            }
        }
    }

    private IEnumerator StopAttackForSeconds(float time) 
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        //Debug.Log("Enemy can attack");
        canAttack = true;
    }

    private IEnumerator SpawnAfterTime(float delay) 
    {
        enabled = false;
        yield return new WaitForSeconds(delay);
        enabled = true;
    }

    private float DistanceFromPlayer() 
    {
        return Vector3.Distance(enemy.transform.position, player.transform.position);
    }

    public void Spawn(float delay) 
    {
        gameObject.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(true);
        StartCoroutine(SpawnAfterTime(delay));
    }


    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.activeSelf) 
        {
            /* If enemy hit player then damage player 
            per once attack by choosen damage value*/
            if (other.CompareTag("Player"))
            {
                if (!isAttacking)
                {
                    StartCoroutine(Attack());
                    other.GetComponent<PlayerHealth>().TakeDamage(damageToPlayer);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gameObject.GetComponent<Collider>().bounds.center, gameObject.GetComponent<Collider>().bounds.size);
        Gizmos.DrawIcon(GetComponent<Collider>().bounds.center + new Vector3(0, 4.5f, 0), "Enemy_Gizmos", true);
    }

}
