using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 14f;
    [SerializeField] private float moveSpeed = 1.85f;
    [SerializeField] private float contactDamage = 8f;
    [SerializeField] private int xpValue = 1;

    private readonly Color baseColor = new Color(1f, 0.22f, 0.16f);
    private readonly Color hitColor = new Color(1f, 0.95f, 0.6f);
    private float currentHealth;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private float hitFlashTimer;

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

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = baseColor;
        spriteRenderer.sortingOrder = 4;
        transform.localScale = Vector3.one * 1.1f;
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

    private void Update()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= Time.deltaTime;
            spriteRenderer.color = hitColor;
        }
        else
        {
            spriteRenderer.color = baseColor;
        }
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
        hitFlashTimer = 0.08f;
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.85f, 0.25f, 0.55f), 0.35f, 0.75f, 0.16f, 8);

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
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.2f, 0.12f, 0.85f), 0.45f, 1.45f, 0.28f, 8);
        XPOrb.Create(transform.position, xpValue);
        Destroy(gameObject);
    }
}
