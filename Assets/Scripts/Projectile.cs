using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;
    private float damage;
    private float speed;

    private void Awake()
    {
        CircleCollider2D collider2d = GetComponent<CircleCollider2D>();
        if (collider2d == null)
        {
            collider2d = gameObject.AddComponent<CircleCollider2D>();
        }
        collider2d.isTrigger = true;
        collider2d.radius = 0.14f;

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
        spriteRenderer.color = new Color(1f, 0.95f, 0.25f);
        spriteRenderer.sortingOrder = 6;
        transform.localScale = Vector3.one * 0.35f;
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
        direction = travelDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
