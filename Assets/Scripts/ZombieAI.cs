using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class ZombieAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float detectionRange = 40f;
    [SerializeField] private float stopDistance = 0.9f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 2.6f;
    [SerializeField] private float hitStaggerFallback = 2.1f;

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private Animator animator;
    private Rigidbody body;
    private Transform player;
    private PlayerHealth playerHealth;

    private bool isAttacking;
    private bool isStaggered;
    private float nextAttackTime;

    private bool Busy { get { return isAttacking || isStaggered; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        bool chasing = false;

        if (player != null && !Busy)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            chasing = distance <= detectionRange && distance > stopDistance;

            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                StartAttack();
                chasing = false;
            }
        }

        animator.SetBool(IsWalkingHash, chasing);
    }

    private void FixedUpdate()
    {
        if (player == null || Busy)
        {
            return;
        }

        Vector3 toPlayer = player.position - body.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
        {
            return;
        }

        Vector3 direction = toPlayer.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion nextRotation = Quaternion.RotateTowards(body.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        body.MoveRotation(nextRotation);

        if (distance <= stopDistance)
        {
            return;
        }

        Vector3 targetPosition = body.position + direction * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(targetPosition);
    }

    private void StartAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        animator.SetTrigger(AttackHash);
    }

    public void AttackHit()
    {
        if (!isAttacking || player == null || playerHealth == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.35f)
        {
            playerHealth.Kill();
        }
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }

    public void HitEnd()
    {
        isStaggered = false;
    }

    public void NotifyHit()
    {
        isAttacking = false;
        isStaggered = true;
        nextAttackTime = Mathf.Max(nextAttackTime, Time.time + hitStaggerFallback);

        CancelInvoke(nameof(ForceEndStagger));
        Invoke(nameof(ForceEndStagger), hitStaggerFallback);
    }

    private void ForceEndStagger()
    {
        isStaggered = false;
    }
}
