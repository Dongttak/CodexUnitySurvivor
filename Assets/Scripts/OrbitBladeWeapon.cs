using System.Collections.Generic;
using UnityEngine;

public class OrbitBladeWeapon : MonoBehaviour
{
    [SerializeField] private int orbitCount = 1;
    [SerializeField] private float orbitRadius = 1.4f;
    [SerializeField] private float rotationSpeedDegrees = 165f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float hitCooldown = 0.4f;
    [SerializeField] private float bladeSize = 0.34f;

    private readonly List<OrbitBladeHitbox> blades = new List<OrbitBladeHitbox>();
    private float angle;

    public int OrbitCount => Mathf.Max(1, orbitCount);
    public float OrbitRadius => orbitRadius;
    public float Damage => damage;
    public float HitCooldown => hitCooldown;

    private void OnEnable()
    {
        EnsureBlades();
        SetBladesActive(true);
    }

    private void OnDisable()
    {
        SetBladesActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        EnsureBlades();
        angle += rotationSpeedDegrees * Time.deltaTime;
        PositionBlades();
    }

    private void EnsureBlades()
    {
        int desiredCount = OrbitCount;
        while (blades.Count < desiredCount)
        {
            GameObject bladeObject = new GameObject("Orbit Blade");
            bladeObject.transform.SetParent(transform, false);

            SpriteRenderer spriteRenderer = bladeObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PlaceholderSprites.Circle;
            spriteRenderer.color = new Color(0.72f, 0.95f, 1f, 0.95f);
            spriteRenderer.sortingOrder = 7;

            CircleCollider2D collider = bladeObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;

            Rigidbody2D body = bladeObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;

            OrbitBladeHitbox hitbox = bladeObject.AddComponent<OrbitBladeHitbox>();
            hitbox.Initialize(this);
            bladeObject.transform.localScale = Vector3.one * bladeSize;
            blades.Add(hitbox);
        }

        for (int i = 0; i < blades.Count; i++)
        {
            blades[i].gameObject.SetActive(i < desiredCount && enabled);
        }
    }

    private void SetBladesActive(bool isActive)
    {
        foreach (OrbitBladeHitbox blade in blades)
        {
            if (blade != null)
            {
                blade.gameObject.SetActive(isActive);
            }
        }
    }

    private void PositionBlades()
    {
        int count = OrbitCount;
        for (int i = 0; i < blades.Count; i++)
        {
            if (blades[i] == null || i >= count)
            {
                continue;
            }

            float bladeAngle = (angle + 360f * i / count) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(bladeAngle), Mathf.Sin(bladeAngle), 0f) * orbitRadius;
            blades[i].transform.localPosition = offset;
        }
    }

    public void HitEnemy(Enemy enemy)
    {
        if (enemy == null || GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        enemy.TakeDamage(damage);
    }
}
