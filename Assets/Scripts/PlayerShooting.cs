using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Camera shootCamera;
    [SerializeField] private Animator armsAnimator;
    [SerializeField] private float range = 100f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 1.2f;
    [SerializeField] private float fireRate = 0.15f;

    private static readonly int FireHash = Animator.StringToHash("Fire");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    private PlayerHealth playerHealth;
    private int currentAmmo;
    private bool isReloading;
    private bool ammoCapInitialized;
    private float nextFireTime;

    private void Awake()
    {
        if (shootCamera == null)
        {
            shootCamera = Camera.main;
        }

        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!ammoCapInitialized)
        {
            InitializeAmmoCap();
        }

        if (isReloading)
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    private void InitializeAmmoCap()
    {
        int totalObjectives = ObjectiveManager.Instance != null ? ObjectiveManager.Instance.Total : 0;

        if (totalObjectives > 0 && GameManager.Instance != null)
        {
            magazineSize = GameManager.Instance.GetRequiredShots(totalObjectives);
        }
        else
        {
            magazineSize = Mathf.Max(1, magazineSize);
        }

        currentAmmo = magazineSize;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetAmmo(currentAmmo, magazineSize);
        }

        ammoCapInitialized = true;
    }

    private void Shoot()
    {
        if (currentAmmo <= 0)
        {
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetAmmo(currentAmmo, magazineSize);
        }

        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger(FireHash);
        }

        if (shootCamera != null)
        {
            Ray ray = new Ray(shootCamera.transform.position, shootCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, range))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                if (hitbox != null)
                {
                    int gained = hitbox.RegisterHit();
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddScore(gained, hitbox.PartLabel());
                    }
                }
                else
                {
                    TargetHealth target = hit.collider.GetComponent<TargetHealth>();
                    if (target != null)
                    {
                        int gained = target.RegisterHit(hit.point);
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.AddScore(gained, "Diana");
                        }
                    }
                }
            }
        }

        if (currentAmmo <= 0 && playerHealth != null)
        {
            playerHealth.LoseLife("Sin municion");
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger(ReloadHash);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetAmmo(currentAmmo, magazineSize);
        }

        isReloading = false;
    }
}
