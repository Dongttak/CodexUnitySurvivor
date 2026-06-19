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
    private WeaponController weaponController;
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
        EnsureWeaponController();
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
            case UpgradeType.UnlockAuraPulse:
                if (EnsureWeaponController() != null)
                {
                    weaponController.UnlockAuraPulse();
                }
                break;
            case UpgradeType.UnlockOrbitBlade:
                if (EnsureWeaponController() != null)
                {
                    weaponController.UnlockOrbitBlade();
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

    private List<UpgradeChoice> BuildUpgradePool()
    {
        List<UpgradeChoice> pool = new List<UpgradeChoice>
        {
            new UpgradeChoice(UpgradeType.ProjectileDamage, "공격력 증가", "탄 공격력 증가"),
            new UpgradeChoice(UpgradeType.FireRate, "공격 속도 증가", "더 빠르게 발사"),
            new UpgradeChoice(UpgradeType.MoveSpeed, "이동 속도 증가", "더 빠르게 이동"),
            new UpgradeChoice(UpgradeType.MaxHealth, "최대 체력 증가", "최대 체력 증가 및 회복"),
            new UpgradeChoice(UpgradeType.Heal, "체력 회복", "체력 회복"),
            new UpgradeChoice(UpgradeType.ProjectileSize, "탄 크기 증가", "탄 크기 증가"),
            new UpgradeChoice(UpgradeType.XPMagnet, "경험치 자석", "경험치 획득 범위 증가"),
            new UpgradeChoice(UpgradeType.MultiShot, "추가 발사", "탄을 추가로 발사")
        };

        if (weaponController != null && !weaponController.HasAuraPulse)
        {
            pool.Add(new UpgradeChoice(UpgradeType.UnlockAuraPulse, "오라 파동 해금", "주기적으로 주변 적에게 피해"));
        }

        if (weaponController != null && !weaponController.HasOrbitBlade)
        {
            pool.Add(new UpgradeChoice(UpgradeType.UnlockOrbitBlade, "회전 칼날 해금", "주변을 도는 칼날이 적에게 피해"));
        }

        return pool;
    }

    private WeaponController EnsureWeaponController()
    {
        if (weaponController != null)
        {
            return weaponController;
        }

        weaponController = FindFirstObjectByType<WeaponController>();
        if (weaponController == null)
        {
            if (playerController == null)
            {
                playerController = FindFirstObjectByType<PlayerController>();
            }

            if (playerController != null)
            {
                weaponController = playerController.gameObject.AddComponent<WeaponController>();
            }
        }

        return weaponController;
    }
}
