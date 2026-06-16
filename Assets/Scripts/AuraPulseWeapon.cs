using UnityEngine;

public class AuraPulseWeapon : MonoBehaviour
{
    [SerializeField] private float cooldown = 3.5f;
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float damage = 6f;
    [SerializeField] private LayerMask enemyMask = ~0;

    private float pulseTimer;
    private readonly Collider2D[] hits = new Collider2D[96];

    public float Cooldown => cooldown;
    public float Radius => radius;
    public float Damage => damage;

    private void OnEnable()
    {
        pulseTimer = Mathf.Min(pulseTimer, 0.35f);
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        pulseTimer -= Time.deltaTime;
        if (pulseTimer > 0f)
        {
            return;
        }

        Pulse();
        pulseTimer = Mathf.Max(0.1f, cooldown);
    }

    private void Pulse()
    {
        FeedbackEffect.SpawnPulse(transform.position, new Color(0.35f, 0.9f, 1f, 0.32f), 0.2f, radius * 2f, 0.42f, 5);

        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits, enemyMask);
        for (int i = 0; i < hitCount; i++)
        {
            Enemy enemy = hits[i] == null ? null : hits[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            hits[i] = null;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }
}
