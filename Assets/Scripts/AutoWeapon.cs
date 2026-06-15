using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [SerializeField] private float range = 8f;
    [SerializeField] private float damage = 7f;
    [SerializeField] private float fireRate = 1.15f;
    [SerializeField] private float projectileSpeed = 11f;

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
        GameObject projectileObject = new GameObject("Projectile");
        projectileObject.transform.position = transform.position;
        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectile.Initialize(direction, damage, projectileSpeed);
        FeedbackEffect.SpawnPulse(transform.position + (Vector3)(direction * 0.35f), new Color(1f, 0.95f, 0.25f, 0.35f), 0.2f, 0.45f, 0.1f, 7);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }
}
