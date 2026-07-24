using UnityEngine;

[RequireComponent(typeof(ZombieAI))]
[RequireComponent(typeof(ZombieAI))]
public class ZombieHealth : MonoBehaviour, IObjective
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float destroyDelay = 4f;

    private int currentHealth;
    private bool isDead;
    private Animator animator;
    private ZombieAI ai;

    public bool IsDead { get { return isDead; } }

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        ai = GetComponent<ZombieAI>();
    }

    private void Start()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.RegisterObjective(this);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayHitReaction();
        }
    }

    private void PlayHitReaction()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (ai != null)
        {
            ai.NotifyHit();
        }
    }

    private void Die()
    {
        isDead = true;

        if (ai != null)
        {
            ai.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsDead", true);
            animator.SetTrigger("Die");
        }

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowMessage("Zombie eliminado");
        }

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.NotifyDestroyed(this);
        }

        Destroy(gameObject, destroyDelay);
    }
}