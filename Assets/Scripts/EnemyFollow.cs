using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFollow : MonoBehaviour
{

    public static EnemyFollow Instance;

    public AudioSource backgroundMusic;

    public Image attackedImage;

    [HideInInspector] public bool isFollowing = false;

  
    private void Awake() 
    {
        Instance = this;
    }

    public void OnFollow() 
    {
        if (!isFollowing)
        {
            StopAllCoroutines();
            backgroundMusic.Play();
            StartCoroutine(StartMusic());
            isFollowing = true;
            enabled = true;
        }
    }

    public void OnNotFollow()
    {
        isFollowing = false;
        StopAllCoroutines();
        if (attackedImage != null && attackedImage.gameObject.activeSelf)
            attackedImage.gameObject.SetActive(false);

        StartCoroutine(StopMusic());
    }

    private IEnumerator StartMusic() 
    {
        while (backgroundMusic.volume <= 1)
        {
            backgroundMusic.volume = Mathf.Lerp(backgroundMusic.volume, 1f, 0.09f);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator StopMusic()
    {
        /* If player is fighting with two or more enemis together
         * the music will continue to play */
        if (!IsAnyEnemyFollowing())
        {
            while (backgroundMusic.volume > 0)
            {
                backgroundMusic.volume = Mathf.Lerp(backgroundMusic.volume, 0f, 0.01f);
                yield return new WaitForEndOfFrame();
            }
            backgroundMusic.Stop();
        }
    }

    public bool IsAnyEnemyFollowing() 
    {
        bool isFollowing = false;
        Enemy[] enemies = FindObjectsOfType<Enemy>(true);
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.isFollowingPlayer)
            {
                isFollowing = true;
                break;
            }
        }
        return isFollowing;
    }

    public void OnEnemyAttack() 
    {
        StopCoroutine(PlayerAttacked());
        StartCoroutine(PlayerAttacked());
    }

    /* This method shows a red screen animation when player 
     * gets attacked */
    private IEnumerator PlayerAttacked()
    {
        attackedImage.gameObject.SetActive(true);
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            attackedImage.color = new Color(attackedImage.color.r, attackedImage.color.g, attackedImage.color.b, i);
            yield return new WaitForSeconds(0.01f);
        }

        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            attackedImage.color = new Color(attackedImage.color.r, attackedImage.color.g, attackedImage.color.b, i);
            yield return new WaitForSeconds(0.01f);
        }
        attackedImage.gameObject.SetActive(false);
    }
    
    void Start()
    {
        enabled = false;   
        attackedImage.gameObject.SetActive(false);
    }

    private void Update() 
    {
        if (IsAnyEnemyFollowing())
        {
            if (backgroundMusic != null && !backgroundMusic.isPlaying)
                backgroundMusic.Play();
        }
    }

}
