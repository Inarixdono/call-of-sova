using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshFilter))]
public class TargetHealth : MonoBehaviour, IObjective
{
    [SerializeField] private int maxScore = 50;
    [SerializeField] private int minScore = 5;

    private bool isDead;
    private float localRadius;

    public bool IsDead { get { return isDead; } }

    private void Awake()
    {
        Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        localRadius = Mathf.Min(bounds.extents.x, bounds.extents.y);
    }

    private void Start()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.RegisterObjective(this);
        }
    }

    public int RegisterHit(Vector3 worldHitPoint)
    {
        if (isDead)
        {
            return 0;
        }

        Vector3 local = transform.InverseTransformPoint(worldHitPoint);
        float distance = Mathf.Sqrt(local.x * local.x + local.y * local.y);
        float t = localRadius > 0f ? Mathf.Clamp01(distance / localRadius) : 0f;
        int points = Mathf.RoundToInt(Mathf.Lerp(maxScore, minScore, t));

        Die();

        return points;
    }

    private void Die()
    {
        isDead = true;

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.NotifyDestroyed(this);
        }

        Destroy(gameObject);
    }
}
