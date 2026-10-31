using UnityEngine;
using Photon.Pun;

public class ShootingControler : MonoBehaviour
{
   Animator animator;
    InputManager inputManager;
    PlayerMovement playerMovement;
    [Header("Shooting var")]
    public Transform firePoint;
    public float fireRate = 0f;
    public float fireRange = 100f;
    public float fireDamage = 10f;
    private float nextFireTime = 0f;
    [Header("Reloading")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;

    [Header("Shooting Flags")]
    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;
    public bool isReloading = false;
    public bool isScopeInPut;

    [Header("Sound Effect")]
    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;
    public AudioClip reloadingSoundClip;

    [Header("Effect")]
    public ParticleSystem muzzelFlash;
    public ParticleSystem bloodEffact;
    PhotonView view;

    public int playerTeam;

    public GameManager gameManager
    {
        get
        {
            return FindFirstObjectByType<GameManager>();
        }
    }
    //Horizetal
    private void Start()
    {
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        currentAmmo = maxAmmo;
        if(view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }

    private void Update()
    {
        if (!view.IsMine)
            return;
        if (gameManager.isMenuOpen == true)
        {
            return;
        }
        if (isReloading || playerMovement.isSprinting)
        {
            animator.SetBool("shoot", false);
            animator.SetBool("shootingMovement", false);
            animator.SetBool("shootWalk", false);
            return;
        }
        isWalking = playerMovement.isMoving;
        isShootingInput=inputManager.fireInput;
        isScopeInPut = inputManager.scopeInput;

        if (isShootingInput && isWalking)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
                animator.SetBool("shootWalk", true);
            }

            animator.SetBool("shoot", false);
            animator.SetBool("shootingMovement", true);
            isShooting = true;

        }

       else if (isShootingInput )
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }

            animator.SetBool("shoot", true);
            animator.SetBool("shootingMovement", false);
            animator.SetBool("shootWalk", false);

            isShooting = true;

        }
        else if(isScopeInPut)
        {
            animator.SetBool("shoot", false);
            animator.SetBool("shootingMovement", true);
            animator.SetBool("shootWalk", false);

            isShooting = false;
        }

        else
        {
            animator.SetBool("shoot", false);
            animator.SetBool("shootingMovement", false);
            animator.SetBool("shootWalk", false);
            isShooting = false;
        }

        if (inputManager.reloadInput && currentAmmo < maxAmmo) 
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (currentAmmo > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Debug.Log(hit.transform.name);
                //extract hit information
                Vector3 hitPoint=hit.point;
                Vector3 hitNormal=hit.normal;
                //apply damage to the player
                PlayerMovement playerMovementDamage=hit.collider.GetComponent<PlayerMovement>();
                if (playerMovementDamage != null && playerMovementDamage.playerTeam!=playerTeam)
                {

                    //apply damage
                    playerMovementDamage.ApplyDamage(fireDamage);
                    view.RPC("RPC_Shoot", RpcTarget.All, hitPoint, hitNormal);
                }
            }

            muzzelFlash.Play();
            soundAudioSource.PlayOneShot(shootingSoundClip);
            currentAmmo--;
        }
        else
        {
            Reload();
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPoint,Vector3 hitNormal)
    {
        ParticleSystem blood = Instantiate(bloodEffact, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(blood.gameObject,blood.main.duration);
    }
    private void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            if (isShootingInput && isWalking)
            {
                animator.SetTrigger("shootReload");
            }
            else
            {
                animator.SetTrigger("reload");
            }
                isReloading = true;
            soundAudioSource.PlayOneShot(reloadingSoundClip);

            Invoke("FinishReloading", reloadTime);
        }
    }

    private void FinishReloading()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        inputManager.reloadInput = false;
        if (isShootingInput && isWalking)
        {
            animator.ResetTrigger("shootReload");
        }
        else
        {
            animator.ResetTrigger("reload");
        }

    }
}
