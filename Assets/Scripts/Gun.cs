using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour, IDifficulty
{
    public static Gun Instance;

    [Header("Gun info")]
    public int consecutiveShot = 3;
    public float timeToReload = 3.5f;
    public float timePerShoot = 0.5f;
    public float damage = 10f;
    public float biasDamage = 3.25f;
    public float range = 3f;
    public GameObject ui;
    public TextMeshProUGUI numberShot;
    private int shot = 0;

    [Header("Gun VFX")]
    public Camera fpsCamera;
    public Transform startPos;
    public ParticleSystem muzzleFlash;
    public ParticleSystem electricity;
    public GameObject impactEffect;
    public GameObject explosionEnemy;
    public Animator animator;

    [Header("Reload Components animation")]
    public GameObject uiReload;
    public GameObject uiReloadCommand;
    public GameObject ledReload;
    public Material ledReloaded;
    public Material ledReloding;
    public AudioSource reloadAudio;

    //Parameters values
    private const int AMMO_EASY = 3;
    private const int AMMO_MEDIUM = 6;
    private const int AMMO_HARD = 10;
    private const float RANGE_EASY = 10.5f;
    private const float RANGE_MEDIUM = 15.5f;
    private const float RANGE_HARD = 30.5f;

    private bool isAiming = false;
    private bool isReloading = false;
    private bool canShoot = true;
    private int _animIDAim;

    private StarterAssets.StarterAssetsInputs _input;

    private void AssignID() 
    {
        _animIDAim = Animator.StringToHash("aiming");
    }

    private void Start() 
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        _input.shootGun = false;
        uiReloadCommand.SetActive(false);
        uiReload.SetActive(false);
    }

    private void Awake() 
    {
        Instance = this;
        AssignID();
        ChangeValuesByDifficulty();
    }

    public void ChangeValuesByDifficulty()
    {
        if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Easy)
        {
            consecutiveShot = AMMO_EASY;
            range = RANGE_EASY;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Medium)
        {
            consecutiveShot = AMMO_MEDIUM;
            range = RANGE_MEDIUM;
        }
        else if (Difficulty.Instance.GetDifficulty() == LevelDifficulty.Difficulty.Hard)
        {
            consecutiveShot = AMMO_HARD;
            range = RANGE_HARD;
        }
    }

    private void OnEnable() 
    {
        numberShot.gameObject.SetActive(true);
        numberShot.text = (consecutiveShot - shot).ToString() + " | " + "\u221E";
        uiReload.SetActive(false);
        uiReloadCommand.SetActive(false);
    }

    private void OnDisable() 
    {
        animator.Rebind();
        canShoot = true;
        ledReload.GetComponent<MeshRenderer>().material = ledReloaded;
        isReloading = false;
        StopAllCoroutines();
    }

    void Update()
    {
        if (StateMachine.Instance.canShoot && shot >= consecutiveShot / 2)
        {
            if (!uiReloadCommand.activeSelf)
                uiReloadCommand.SetActive(true);

            if (_input.reload)
            {
                _input.Reset();
                if (!isReloading)
                    StartCoroutine(InstantReload());
            }
        }

        if (!isAiming)
        {
            if (_input.aimingGun) 
            {
                animator.SetBool(_animIDAim, true);
                isAiming = true;
                Raycast.Instance.SetCrosshair(Raycast.Instance.crosshairAiming);
            } else
            {
                _input.aimingGun = false;
            }
        } else
        {
            if (!_input.aimingGun)
            {
                _input.aimingGun = false;
                animator.SetBool(_animIDAim, false);
                isAiming = false;
                Raycast.Instance.SetCrosshair(Raycast.Instance.crosshairNotColliding);
            }
        }

        if (_input.shootGun && StateMachine.Instance.canShoot)
        {
            _input.shootGun = false;
            if (canShoot)
            {
                Shoot();
            }
        }

    }

    public void Shoot() 
    {
        if (animator.GetBool("aiming"))
            animator.Play("GunAimingShoot");
        else
            animator.Play("GunShoot");

        ShootSound();
        StartCoroutine(ShootLaser());
        StartCoroutine(Reload());

        /* A ray will be emitted. If it hit a collider, it will add force to it. 
         * If it hit the enemy, it will damage him. */
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            StartCoroutine(ImpactEffect(hit));
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Enemy>() != null)
                {
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddRelativeForce(-hit.normal * 100f);
                    }
                    
                    if (hit.distance < range)
                    {
                        //Debug.Log("Distance shoot: " + hit.distance + "Range: " + range); 
                        float damage = hit.collider.gameObject.GetComponent<Enemy>().damageByGun;
                        damage = Random.Range(damage - biasDamage, damage + biasDamage);
                        //Debug.Log("Damage Enemy: " + damage + "");
                        hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                 
                }
            }
        }
    }

    /* This method reload the gun after each shoot. This means that the player
     * cannot shoot multiple shot simultaneosly */
    private IEnumerator Reload() 
    {
        isReloading = true;
        canShoot = false;
        shot++;
        numberShot.text = (consecutiveShot - shot).ToString() + " | " + "\u221E";
        if (shot == consecutiveShot)
        {
            shot = 0;
            numberShot.gameObject.SetActive(false);
            uiReload.SetActive(true);
            Material ledStart = ledReload.GetComponent<MeshRenderer>().material;
            StartCoroutine(ReloadSound());
            ledReload.GetComponent<MeshRenderer>().material = ledReloding;
            uiReloadCommand.SetActive(false);
            yield return new WaitForSeconds(timeToReload);
            ledReload.GetComponent<MeshRenderer>().material = ledReloaded;
            uiReload.SetActive(false);
            uiReload.GetComponent<Animator>().Rebind();
        } else
        {
            yield return new WaitForSeconds(timePerShoot);
        }
        numberShot.gameObject.SetActive(true);
        numberShot.text = (consecutiveShot - shot).ToString() + " | " + "\u221E";
        canShoot = true;
        isReloading = false;
    }

    /* This method will reload the gun instantly */
    private IEnumerator InstantReload() 
    {
        isReloading = true;
        canShoot = false;
        shot = 0;
        numberShot.gameObject.SetActive(false);
        uiReload.SetActive(true);
        StartCoroutine(ReloadSound());
        ledReload.GetComponent<MeshRenderer>().material = ledReloding;
        uiReloadCommand.SetActive(false);
        yield return new WaitForSeconds(timeToReload + 0.5f);
        ledReload.GetComponent<MeshRenderer>().material = ledReloaded;
        uiReload.SetActive(false);
        uiReload.GetComponent<Animator>().Rebind();
        numberShot.gameObject.SetActive(true);
        numberShot.text = (consecutiveShot - shot).ToString() + " | " + "\u221E";
        canShoot = true;
        isReloading = false;
    }

    private IEnumerator ImpactEffect(RaycastHit hit) 
    {
        GameObject cloneImpact = Instantiate(impactEffect, hit.point, Quaternion.identity);
        yield return new WaitForSeconds(0.75f);
        Destroy(cloneImpact);
    }

    private IEnumerator ShootLaser() 
    {
        electricity.Play();
        yield return new WaitForSeconds(electricity.main.duration);
        electricity.Stop();
    }

    private IEnumerator ReloadSound() {
        reloadAudio.Play();
        yield return new WaitForSeconds(reloadAudio.clip.length);
        reloadAudio.Stop();
    }

    private void ShootSound()
    {
        GameObject shoot = new GameObject();
        GameObject eletric = new GameObject();
        shoot.AddComponent<AudioSource>();
        eletric.AddComponent<AudioSource>();
        eletric.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Weapon/ElectricityGun"));
        shoot.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/Weapon/LaserGun2"));
        Destroy(eletric, Resources.Load<AudioClip>("Audio/Weapon/ElectricityGun").length);
        Destroy(shoot, Resources.Load<AudioClip>("Audio/Weapon/LaserGun2").length);
    }

}
