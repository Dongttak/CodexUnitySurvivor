using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private static readonly Queue<Projectile> Pool = new Queue<Projectile>();

    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;
    private float damage;
    private float speed;
    private float remainingLifetime;
    private float sizeMultiplier = 1f;
    private CircleCollider2D circleCollider;
    private bool active;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPool()
    {
        Pool.Clear();
    }

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.18f * sizeMultiplier;

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body == null)
        {
            body = gameObject.AddComponent<Rigidbody2D>();
        }
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = new Color(1f, 0.92f, 0.18f);
        spriteRenderer.sortingOrder = 6;
        ApplySize();
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
        {
            Release();
        }
    }

    public static Projectile Spawn(Vector3 position, Vector2 travelDirection, float projectileDamage, float projectileSpeed, float projectileSize)
    {
        Projectile projectile = Get();
        projectile.transform.position = position;
        projectile.Initialize(travelDirection, projectileDamage, projectileSpeed, projectileSize);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Initialize(Vector2 travelDirection, float projectileDamage, float projectileSpeed)
    {
        Initialize(travelDirection, projectileDamage, projectileSpeed, 1f);
    }

    public void Initialize(Vector2 travelDirection, float projectileDamage, float projectileSpeed, float projectileSize)
    {
        direction = travelDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;
        sizeMultiplier = Mathf.Max(0.5f, projectileSize);
        remainingLifetime = lifetime;
        active = true;
        ApplySize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.95f, 0.25f, 0.6f), 0.2f, 0.55f, 0.12f, 8);
        Release();
    }

    private void ApplySize()
    {
        transform.localScale = Vector3.one * 0.42f * sizeMultiplier;

        if (circleCollider != null)
        {
            circleCollider.radius = 0.18f * sizeMultiplier;
        }
    }

    private static Projectile Get()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }

        GameObject projectileObject = new GameObject("Projectile");
        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectileObject.SetActive(false);
        return projectile;
    }

    private void Release()
    {
        active = false;
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
