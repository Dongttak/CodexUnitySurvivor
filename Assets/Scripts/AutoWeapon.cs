using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [SerializeField] private float range = 8f;
    [SerializeField] private float damage = 7f;
    [SerializeField] private float fireRate = 1.15f;
    [SerializeField] private float projectileSpeed = 11f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float multiShotSpreadDegrees = 14f;

    private float fireTimer;

    public float Damage => damage;
    public float FireRate => fireRate;

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f)
        {
            return;
        }

        Enemy target = FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        FireAt(target.transform.position);
        fireTimer = 1f / Mathf.Max(0.1f, fireRate);
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate += amount;
    }

    public void IncreaseProjectileSize(float amount)
    {
        projectileSize = Mathf.Max(0.5f, projectileSize + amount);
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectileCount = Mathf.Max(1, projectileCount + amount);
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float nearestDistanceSquared = range * range;
        Vector3 origin = transform.position;

        foreach (Enemy enemy in enemies)
        {
            float distanceSquared = (enemy.transform.position - origin).sqrMagnitude;
            if (distanceSquared <= nearestDistanceSquared)
            {
                nearest = enemy;
                nearestDistanceSquared = distanceSquared;
            }
        }

        return nearest;
    }

    private void FireAt(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        int shots = Mathf.Max(1, projectileCount);
        float startAngle = shots > 1 ? -multiShotSpreadDegrees * (shots - 1) * 0.5f : 0f;

        for (int i = 0; i < shots; i++)
        {
            float angle = startAngle + multiShotSpreadDegrees * i;
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, angle) * direction;
            SpawnProjectile(shotDirection);
        }

        FeedbackEffect.SpawnPulse(transform.position + (Vector3)(direction * 0.35f), new Color(1f, 0.95f, 0.25f, 0.35f), 0.2f, 0.45f, 0.1f, 7);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }

    private void SpawnProjectile(Vector2 direction)
    {
        Projectile.Spawn(transform.position, direction, damage, projectileSpeed, projectileSize);
    }
}
