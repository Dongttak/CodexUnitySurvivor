using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float initialSpawnInterval = 2.1f;
    [SerializeField] private float minimumSpawnInterval = 0.45f;
    [SerializeField] private float intervalReductionPerSecond = 0.01f;
    [SerializeField] private float spawnDistance = 11f;
    [SerializeField] private int maxActiveEnemies = 90;

    private float elapsedTime;
    private float spawnTimer;

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
        Vector2 direction = Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        Vector3 spawnPosition = player.position + (Vector3)(direction * spawnDistance);
        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.transform.position = spawnPosition;
        Enemy enemy = enemyObject.AddComponent<Enemy>();
        enemy.Initialize(player);
    }
}
