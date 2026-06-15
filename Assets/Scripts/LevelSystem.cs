using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP;
    [SerializeField] private int xpToNextLevel = 4;
    [SerializeField] private float xpRequirementMultiplier = 1.35f;
    [SerializeField] private float damageUpgradeAmount = 2f;
    [SerializeField] private float fireRateUpgradeAmount = 0.25f;
    [SerializeField] private float moveSpeedUpgradeAmount = 0.45f;
    [SerializeField] private float maxHealthUpgradeAmount = 20f;
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float projectileSizeUpgradeAmount = 0.25f;
    [SerializeField] private float xpMagnetUpgradeAmount = 1.25f;
    [SerializeField] private int multiShotUpgradeAmount = 1;

    private PlayerController playerController;
    private AutoWeapon autoWeapon;
    private UpgradeManager upgradeManager;
    private bool choosingUpgrade;

    public event Action<int, int, int> OnXPChanged;
    public event Action<int> OnLevelChanged;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;

    public readonly struct UpgradeChoice
    {
        public UpgradeChoice(UpgradeType type, string name, string description)
        {
            Type = type;
            Name = name;
            Description = description;
        }

        public UpgradeType Type { get; }
        public string Name { get; }
        public string Description { get; }
    }

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        autoWeapon = FindFirstObjectByType<AutoWeapon>();
        upgradeManager = FindFirstObjectByType<UpgradeManager>();

        OnLevelChanged?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel, currentLevel);
    }

    public void AddXP(int amount)
    {
        if (amount <= 0 || GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        currentXP += amount;
        OnXPChanged?.Invoke(currentXP, xpToNextLevel, currentLevel);

        if (!choosingUpgrade && currentXP >= xpToNextLevel)
        {
            BeginLevelUp();
        }
    }

    public void ApplyUpgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.ProjectileDamage:
                if (autoWeapon != null)
                {
                    autoWeapon.IncreaseDamage(damageUpgradeAmount);
                }
                break;
            case UpgradeType.FireRate:
                if (autoWeapon != null)
                {
                    autoWeapon.IncreaseFireRate(fireRateUpgradeAmount);
                }
                break;
            case UpgradeType.MoveSpeed:
                if (playerController != null)
                {
                    playerController.IncreaseMoveSpeed(moveSpeedUpgradeAmount);
                }
                break;
            case UpgradeType.MaxHealth:
                PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.IncreaseMaxHealth(maxHealthUpgradeAmount, maxHealthUpgradeAmount);
                }
                break;
            case UpgradeType.Heal:
                PlayerHealth healingTarget = FindFirstObjectByType<PlayerHealth>();
                if (healingTarget != null)
                {
                    healingTarget.Heal(healAmount);
                }
                break;
            case UpgradeType.ProjectileSize:
                if (autoWeapon != null)
                {
                    autoWeapon.IncreaseProjectileSize(projectileSizeUpgradeAmount);
                }
                break;
            case UpgradeType.XPMagnet:
                XPOrb.IncreaseAttractionRadius(xpMagnetUpgradeAmount);
                break;
            case UpgradeType.MultiShot:
                if (autoWeapon != null)
                {
                    autoWeapon.IncreaseProjectileCount(multiShotUpgradeAmount);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUpgradeSelected();
        }

        choosingUpgrade = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGameplay();
        }

        if (currentXP >= xpToNextLevel)
        {
            BeginLevelUp();
        }
    }

    private void BeginLevelUp()
    {
        choosingUpgrade = true;
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.CeilToInt(xpToNextLevel * xpRequirementMultiplier);

        OnLevelChanged?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel, currentLevel);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUp();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseForUpgrade();
        }

        if (upgradeManager != null)
        {
            upgradeManager.ShowChoices(this, RollUpgradeChoices());
        }
    }

    private List<UpgradeChoice> RollUpgradeChoices()
    {
        List<UpgradeChoice> pool = BuildUpgradePool();
        List<UpgradeChoice> choices = new List<UpgradeChoice>();

        while (choices.Count < 3 && pool.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            choices.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return choices;
    }

    private static List<UpgradeChoice> BuildUpgradePool()
    {
        return new List<UpgradeChoice>
        {
            new UpgradeChoice(UpgradeType.ProjectileDamage, "Sharper Projectiles", "More damage per hit"),
            new UpgradeChoice(UpgradeType.FireRate, "Faster Casting", "Shoot a little more often"),
            new UpgradeChoice(UpgradeType.MoveSpeed, "Quicker Footwork", "Move faster to escape crowds"),
            new UpgradeChoice(UpgradeType.MaxHealth, "Max HP Up", "Raise max HP and heal by the same amount"),
            new UpgradeChoice(UpgradeType.Heal, "Patch Up", "Instantly restore some HP"),
            new UpgradeChoice(UpgradeType.ProjectileSize, "Bigger Shots", "Larger projectiles with wider collision"),
            new UpgradeChoice(UpgradeType.XPMagnet, "XP Magnet", "Pull XP orbs from farther away"),
            new UpgradeChoice(UpgradeType.MultiShot, "Multi Shot", "Fire one additional projectile per attack")
        };
    }
}
