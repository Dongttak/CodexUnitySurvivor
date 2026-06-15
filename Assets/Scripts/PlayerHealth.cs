using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float contactDamageCooldown = 0.35f;

    private float currentHealth;
    private float nextDamageTime;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private float damageFlashTimer;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.deltaTime;
            spriteRenderer.color = new Color(1f, 0.95f, 0.95f);
        }
        else
        {
            spriteRenderer.color = baseColor;
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || Time.time < nextDamageTime || currentHealth <= 0f)
        {
            return;
        }

        nextDamageTime = Time.time + contactDamageCooldown;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        damageFlashTimer = 0.1f;
        FeedbackEffect.SpawnPulse(transform.position, new Color(1f, 0.15f, 0.15f, 0.55f), 0.45f, 1.25f, 0.22f, 10);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDamage();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            OnDied?.Invoke();
        }
    }

    public void IncreaseMaxHealth(float amount, float healAmount)
    {
        if (amount <= 0f)
        {
            return;
        }

        maxHealth += amount;
        Heal(healAmount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
