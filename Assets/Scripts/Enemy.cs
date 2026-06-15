using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 12f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private int xpValue = 1;

    private float currentHealth;
    private Rigidbody2D body;
    private Transform target;

    private void Awake()
    {
        currentHealth = maxHealth;
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        CircleCollider2D collider2d = GetComponent<CircleCollider2D>();
        if (collider2d == null)
        {
            collider2d = gameObject.AddComponent<CircleCollider2D>();
        }
        collider2d.radius = 0.42f;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = new Color(1f, 0.25f, 0.2f);
        spriteRenderer.sortingOrder = 4;
    }

    private void Start()
    {
        if (target == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null || GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - body.position).normalized;
        body.MovePosition(body.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public void Initialize(Transform playerTarget)
    {
        target = playerTarget;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }

    private void Die()
    {
        XPOrb.Create(transform.position, xpValue);
        Destroy(gameObject);
    }
}
