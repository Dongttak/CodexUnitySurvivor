using System;
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

    private PlayerController playerController;
    private AutoWeapon autoWeapon;
    private UpgradeManager upgradeManager;
    private bool choosingUpgrade;

    public event Action<int, int, int> OnXPChanged;
    public event Action<int> OnLevelChanged;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;

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
            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
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
            upgradeManager.ShowChoices(this);
        }
    }
}
