using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float contactDamageCooldown = 0.35f;

    private float currentHealth;
    private float nextDamageTime;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || Time.time < nextDamageTime || currentHealth <= 0f)
        {
            return;
        }

        nextDamageTime = Time.time + contactDamageCooldown;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            OnDied?.Invoke();
        }
    }
}
