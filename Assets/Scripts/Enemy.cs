using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    private static readonly Queue<Enemy> Pool = new Queue<Enemy>();

    [SerializeField] private float maxHealth = 14f;
    [SerializeField] private float moveSpeed = 1.85f;
    [SerializeField] private float contactDamage = 8f;
    [SerializeField] private int xpValue = 1;
    [SerializeField] private string enemyTypeName = "Basic";
    [SerializeField] private Color baseColor = new Color(1f, 0.22f, 0.16f);
    [SerializeField] private Color hitColor = new Color(1f, 0.95f, 0.6f);
    [SerializeField] private float visualSize = 1.1f;

    private float currentHealth;
    private Rigidbody2D body;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private float hitFlashTimer;
    private SpriteRenderer hpBarBorder;
    private SpriteRenderer hpBarBack;
    private SpriteRenderer hpBarFill;
    private bool active;

    public string EnemyTypeName => enemyTypeName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPool()
    {
        Pool.Clear();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        circleCollider.radius = 0.42f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = baseColor;
        spriteRenderer.sortingOrder = 4;

        EnsureHpBar();
        ApplyVisuals();
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
        if (!active || target == null || GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - body.position).normalized;
        body.MovePosition(body.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

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

        UpdateHpBar();
    }

    public void Initialize(Transform playerTarget)
    {
        target = playerTarget;
    }

    public static Enemy Spawn(EnemySpawner.EnemyDefinition definition, Vector3 position, Transform playerTarget)
    {
        Enemy enemy = Get();
        enemy.transform.position = position;
        enemy.Configure(definition, playerTarget);
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void TakeDamage(float amount)
    {
        if (!active || amount <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= amount;
        hitFlashTimer = 0.08f;
        UpdateHpBar();
        DamageNumber.Spawn(transform.position, amount);
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.85f, 0.25f, 0.55f), 0.35f, 0.75f, 0.16f, 8);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHit();
        }

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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyDeath();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterKill(enemyTypeName);
        }

        XPOrb.Create(transform.position, xpValue);
        Release();
    }

    private static Enemy Get()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }

        GameObject enemyObject = new GameObject("Enemy");
        Enemy enemy = enemyObject.AddComponent<Enemy>();
        enemyObject.SetActive(false);
        return enemy;
    }

    private void Configure(EnemySpawner.EnemyDefinition definition, Transform playerTarget)
    {
        enemyTypeName = definition.TypeName;
        maxHealth = definition.MaxHealth;
        moveSpeed = definition.MoveSpeed;
        contactDamage = definition.ContactDamage;
        xpValue = definition.XPValue;
        baseColor = definition.VisualColor;
        visualSize = definition.VisualSize;
        hitColor = Color.Lerp(baseColor, Color.white, 0.75f);
        currentHealth = maxHealth;
        hitFlashTimer = 0f;
        active = true;
        target = playerTarget;
        gameObject.name = enemyTypeName + " Enemy";
        ApplyVisuals();
        UpdateHpBar();
    }

    private void ApplyVisuals()
    {
        transform.localScale = Vector3.one * visualSize;

        if (circleCollider != null)
        {
            circleCollider.radius = 0.42f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = baseColor;
        }

        Vector3 inverseVisualScale = Vector3.one / Mathf.Max(0.01f, visualSize);
        if (hpBarBorder != null)
        {
            hpBarBorder.transform.localScale = inverseVisualScale;
        }

        if (hpBarBack != null)
        {
            hpBarBack.transform.localScale = inverseVisualScale;
        }

        if (hpBarFill != null)
        {
            hpBarFill.transform.localScale = inverseVisualScale;
        }
    }

    private void EnsureHpBar()
    {
        if (hpBarBorder != null && hpBarBack != null && hpBarFill != null)
        {
            return;
        }

        GameObject borderObject = new GameObject("HP Bar Border");
        borderObject.transform.SetParent(transform, false);
        hpBarBorder = borderObject.AddComponent<SpriteRenderer>();
        hpBarBorder.sprite = PlaceholderSprites.Square;
        hpBarBorder.color = new Color(0f, 0f, 0f, 0.9f);
        hpBarBorder.sortingOrder = 10;

        GameObject backObject = new GameObject("HP Bar Back");
        backObject.transform.SetParent(transform, false);
        hpBarBack = backObject.AddComponent<SpriteRenderer>();
        hpBarBack.sprite = PlaceholderSprites.Square;
        hpBarBack.color = new Color(0.05f, 0.055f, 0.065f, 0.86f);
        hpBarBack.sortingOrder = 11;

        GameObject fillObject = new GameObject("HP Bar Fill");
        fillObject.transform.SetParent(transform, false);
        hpBarFill = fillObject.AddComponent<SpriteRenderer>();
        hpBarFill.sprite = PlaceholderSprites.Square;
        hpBarFill.color = new Color(0.2f, 1f, 0.35f, 0.9f);
        hpBarFill.sortingOrder = 12;
    }

    private void UpdateHpBar()
    {
        if (hpBarBack == null || hpBarFill == null || maxHealth <= 0f)
        {
            return;
        }

        float ratio = Mathf.Clamp01(currentHealth / maxHealth);
        bool shouldShow = ratio < 0.999f || enemyTypeName == "Tank" || enemyTypeName == "Elite";
        hpBarBorder.enabled = shouldShow;
        hpBarBack.enabled = shouldShow;
        hpBarFill.enabled = shouldShow;

        const float width = 1.25f;
        const float height = 0.13f;
        Vector3 barPosition = new Vector3(0f, 0.82f, 0f);
        Vector3 inverseVisualScale = Vector3.one / Mathf.Max(0.01f, visualSize);

        hpBarBorder.transform.localPosition = barPosition;
        hpBarBorder.transform.localScale = new Vector3((width + 0.12f) * inverseVisualScale.x, (height + 0.07f) * inverseVisualScale.y, 1f);
        hpBarBack.transform.localPosition = barPosition;
        hpBarBack.transform.localScale = new Vector3(width * inverseVisualScale.x, height * inverseVisualScale.y, 1f);
        hpBarFill.transform.localPosition = barPosition + new Vector3(-(width * (1f - ratio)) * 0.5f, 0f, 0f);
        hpBarFill.transform.localScale = new Vector3(width * ratio * inverseVisualScale.x, height * 0.68f * inverseVisualScale.y, 1f);
        hpBarFill.color = Color.Lerp(new Color(1f, 0.16f, 0.1f, 0.98f), new Color(0.18f, 1f, 0.42f, 0.98f), ratio);
    }

    private void Release()
    {
        active = false;
        target = null;
        if (hpBarBorder != null)
        {
            hpBarBorder.enabled = false;
        }
        if (hpBarBack != null)
        {
            hpBarBack.enabled = false;
        }
        if (hpBarFill != null)
        {
            hpBarFill.enabled = false;
        }
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
