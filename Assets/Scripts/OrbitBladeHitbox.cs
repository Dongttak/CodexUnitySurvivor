using System.Collections.Generic;
using UnityEngine;

public class OrbitBladeHitbox : MonoBehaviour
{
    private readonly Dictionary<Enemy, float> nextHitTimes = new Dictionary<Enemy, float>();
    private OrbitBladeWeapon weapon;

    public void Initialize(OrbitBladeWeapon owner)
    {
        weapon = owner;
    }

    private void OnEnable()
    {
        nextHitTimes.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (weapon == null)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        float nextTime;
        if (nextHitTimes.TryGetValue(enemy, out nextTime) && Time.time < nextTime)
        {
            return;
        }

        nextHitTimes[enemy] = Time.time + weapon.HitCooldown;
        weapon.HitEnemy(enemy);
    }
}
