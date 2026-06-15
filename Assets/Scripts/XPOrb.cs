using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float attractRadius = 3f;
    [SerializeField] private float collectRadius = 0.35f;
    [SerializeField] private float moveSpeed = 6f;

    private Transform player;
    private LevelSystem levelSystem;

    private void Awake()
    {
        CircleCollider2D collider2d = GetComponent<CircleCollider2D>();
        if (collider2d == null)
        {
            collider2d = gameObject.AddComponent<CircleCollider2D>();
        }
        collider2d.isTrigger = true;
        collider2d.radius = 0.22f;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = new Color(0.35f, 1f, 0.4f);
        spriteRenderer.sortingOrder = 3;
        transform.localScale = Vector3.one * 0.45f;
    }

    private void Start()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }

        levelSystem = FindFirstObjectByType<LevelSystem>();
    }

    private void Update()
    {
        if (player == null || levelSystem == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= collectRadius)
        {
            Collect();
            return;
        }

        if (distance <= attractRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    public static XPOrb Create(Vector3 position, int xpValue)
    {
        GameObject orbObject = new GameObject("XP Orb");
        orbObject.transform.position = position;
        XPOrb orb = orbObject.AddComponent<XPOrb>();
        orb.value = xpValue;
        return orb;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (levelSystem != null)
        {
            levelSystem.AddXP(value);
        }

        Destroy(gameObject);
    }
}
