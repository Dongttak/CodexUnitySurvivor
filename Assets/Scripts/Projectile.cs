using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;
    private float damage;
    private float speed;
    private float sizeMultiplier = 1f;
    private CircleCollider2D circleCollider;

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
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
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
        ApplySize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.95f, 0.25f, 0.6f), 0.2f, 0.55f, 0.12f, 8);
        Destroy(gameObject);
    }

    private void ApplySize()
    {
        transform.localScale = Vector3.one * 0.42f * sizeMultiplier;

        if (circleCollider != null)
        {
            circleCollider.radius = 0.18f * sizeMultiplier;
        }
    }
}
