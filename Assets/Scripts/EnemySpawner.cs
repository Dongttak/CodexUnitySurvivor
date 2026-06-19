using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float initialSpawnInterval = 2.1f;
    [SerializeField] private float minimumSpawnInterval = 0.45f;
    [SerializeField] private float intervalReductionPerSecond = 0.01f;
    [SerializeField] private float spawnDistance = 11f;
    [SerializeField] private int maxActiveEnemies = 90;
    [SerializeField] private EnemyDefinition basicEnemy = new EnemyDefinition("Basic", 14f, 1.85f, 8f, 1, new Color(1f, 0.22f, 0.16f), 1.1f);
    [SerializeField] private EnemyDefinition fastEnemy = new EnemyDefinition("Fast", 9f, 2.65f, 6f, 1, new Color(1f, 0.78f, 0.18f), 0.82f);
    [SerializeField] private EnemyDefinition tankEnemy = new EnemyDefinition("Tank", 34f, 1.25f, 10f, 3, new Color(0.72f, 0.25f, 1f), 1.55f);
    [SerializeField] private EnemyDefinition eliteEnemy = new EnemyDefinition("Elite", 78f, 1.45f, 12f, 12, new Color(0.25f, 0.95f, 1f), 2.05f);
    [SerializeField] private float eliteSpawnTime = 300f;

    private float elapsedTime;
    private float spawnTimer;
    private int currentPhaseIndex = -1;
    private bool eliteSpawned;

    public string CurrentPhaseNameKorean => GetCurrentPhase().NameKorean;
    public bool EliteSpawned => eliteSpawned;
    public float ElapsedTime => elapsedTime;

    [Serializable]
    public class EnemyDefinition
    {
        public EnemyDefinition(string typeName, float maxHealth, float moveSpeed, float contactDamage, int xpValue, Color visualColor, float visualSize)
        {
            TypeName = typeName;
            MaxHealth = maxHealth;
            MoveSpeed = moveSpeed;
            ContactDamage = contactDamage;
            XPValue = xpValue;
            VisualColor = visualColor;
            VisualSize = visualSize;
        }

        public string TypeName = "Basic";
        public float MaxHealth = 14f;
        public float MoveSpeed = 1.85f;
        public float ContactDamage = 8f;
        public int XPValue = 1;
        public Color VisualColor = new Color(1f, 0.22f, 0.16f);
        public float VisualSize = 1.1f;
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        spawnTimer = 1f;
    }

    private void Update()
    {
        if (player == null || GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        UpdatePhaseAnnouncements();
        TrySpawnElite();

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
        {
            return;
        }

        int activeEnemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (activeEnemyCount < maxActiveEnemies)
        {
            SpawnEnemy();
        }

        spawnTimer = Mathf.Max(minimumSpawnInterval, GetCurrentPhase().SpawnInterval - elapsedTime * intervalReductionPerSecond);
    }

    private void SpawnEnemy()
    {
        SpawnEnemy(ChooseEnemyDefinition());
    }

    private void SpawnEnemy(EnemyDefinition definition)
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        Vector3 spawnPosition = player.position + (Vector3)(direction * spawnDistance);
        Enemy.Spawn(definition, spawnPosition, player);
    }

    private EnemyDefinition ChooseEnemyDefinition()
    {
        DirectorPhase phase = GetCurrentPhase();
        float roll = UnityEngine.Random.value;

        switch (phase.Index)
        {
            case 0:
                return basicEnemy;
            case 1:
                return roll < 0.28f ? fastEnemy : basicEnemy;
            case 2:
                if (roll < 0.18f)
                {
                    return tankEnemy;
                }
                return roll < 0.48f ? fastEnemy : basicEnemy;
            case 3:
                if (roll < 0.12f)
                {
                    return tankEnemy;
                }
                return roll < 0.62f ? fastEnemy : basicEnemy;
            default:
                if (roll < 0.18f)
                {
                    return tankEnemy;
                }
                return roll < 0.58f ? fastEnemy : basicEnemy;
        }
    }

    private void UpdatePhaseAnnouncements()
    {
        DirectorPhase phase = GetCurrentPhase();
        if (phase.Index == currentPhaseIndex)
        {
            return;
        }

        bool shouldAnnounce = currentPhaseIndex >= 0 && !string.IsNullOrEmpty(phase.AnnouncementKorean);
        currentPhaseIndex = phase.Index;
        if (shouldAnnounce)
        {
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowAnnouncement(phase.AnnouncementKorean);
            }
        }
    }

    private void TrySpawnElite()
    {
        if (eliteSpawned || elapsedTime < eliteSpawnTime)
        {
            return;
        }

        eliteSpawned = true;
        SpawnEnemy(eliteEnemy);

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowAnnouncement("정예 적 접근 중!");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUp();
        }
    }

    private DirectorPhase GetCurrentPhase()
    {
        if (elapsedTime < 60f)
        {
            return new DirectorPhase(0, "1단계: 기본 적", 2.1f, "");
        }

        if (elapsedTime < 120f)
        {
            return new DirectorPhase(1, "2단계: 빠른 적", 1.75f, "빠른 적 등장!");
        }

        if (elapsedTime < 210f)
        {
            return new DirectorPhase(2, "3단계: 탱커 적", 1.35f, "탱커 적 등장!");
        }

        if (elapsedTime < 300f)
        {
            return new DirectorPhase(3, "4단계: 거센 공세", 0.95f, "적의 공세가 거세집니다!");
        }

        return new DirectorPhase(4, "5단계: 정예 적", 0.85f, "");
    }

    private readonly struct DirectorPhase
    {
        public DirectorPhase(int index, string nameKorean, float spawnInterval, string announcementKorean)
        {
            Index = index;
            NameKorean = nameKorean;
            SpawnInterval = spawnInterval;
            AnnouncementKorean = announcementKorean;
        }

        public int Index { get; }
        public string NameKorean { get; }
        public float SpawnInterval { get; }
        public string AnnouncementKorean { get; }
    }
}
