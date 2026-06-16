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

    private float elapsedTime;
    private float spawnTimer;

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

        spawnTimer = Mathf.Max(minimumSpawnInterval, initialSpawnInterval - elapsedTime * intervalReductionPerSecond);
    }

    private void SpawnEnemy()
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        Vector3 spawnPosition = player.position + (Vector3)(direction * spawnDistance);
        Enemy.Spawn(ChooseEnemyDefinition(), spawnPosition, player);
    }

    private EnemyDefinition ChooseEnemyDefinition()
    {
        if (elapsedTime < 60f)
        {
            return basicEnemy;
        }

        if (elapsedTime < 120f)
        {
            return UnityEngine.Random.value < 0.25f ? fastEnemy : basicEnemy;
        }

        float roll = UnityEngine.Random.value;
        if (roll < 0.2f)
        {
            return tankEnemy;
        }

        return roll < 0.55f ? fastEnemy : basicEnemy;
    }
}
