using System.Collections.Generic;
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    private static readonly Queue<XPOrb> Pool = new Queue<XPOrb>();

    [SerializeField] private int value = 1;
    [SerializeField] private float attractRadius = 3.4f;
    [SerializeField] private float collectRadius = 0.35f;
    [SerializeField] private float moveSpeed = 6f;

    private Transform player;
    private LevelSystem levelSystem;
    private static float attractionRadiusBonus;
    private bool active;

    public static float BaseAttractionRadius => 3.4f;
    public static float AttractionRadiusBonus => attractionRadiusBonus;
    public static float CurrentAttractionRadius => BaseAttractionRadius + attractionRadiusBonus;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        attractionRadiusBonus = 0f;
        Pool.Clear();
    }

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
        spriteRenderer.color = new Color(0.25f, 1f, 0.55f);
        spriteRenderer.sortingOrder = 3;
        transform.localScale = Vector3.one * 0.52f;
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
        if (!active)
        {
            return;
        }

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

        if (distance <= attractRadius + attractionRadiusBonus)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    public static void IncreaseAttractionRadius(float amount)
    {
        attractionRadiusBonus = Mathf.Max(0f, attractionRadiusBonus + amount);
    }

    public static XPOrb Create(Vector3 position, int xpValue)
    {
        XPOrb orb = Get();
        orb.transform.position = position;
        orb.value = xpValue;
        orb.ResolveReferences();
        orb.active = true;
        orb.gameObject.SetActive(true);
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
        FeedbackEffect.SpawnPickup(transform.position);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayXpPickup();
        }

        if (levelSystem != null)
        {
            levelSystem.AddXP(value);
        }

        Release();
    }

    private static XPOrb Get()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }

        GameObject orbObject = new GameObject("XP Orb");
        XPOrb orb = orbObject.AddComponent<XPOrb>();
        orbObject.SetActive(false);
        return orb;
    }

    private void ResolveReferences()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        player = playerController != null ? playerController.transform : null;
        levelSystem = FindFirstObjectByType<LevelSystem>();
    }

    private void Release()
    {
        active = false;
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
