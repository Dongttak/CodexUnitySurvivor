using System.Collections.Generic;
using UnityEngine;

public class FeedbackEffect : MonoBehaviour
{
    private static readonly Queue<FeedbackEffect> Pool = new Queue<FeedbackEffect>();

    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float startScale = 0.35f;
    [SerializeField] private float endScale = 1.2f;
    [SerializeField] private Vector3 drift;

    private SpriteRenderer spriteRenderer;
    private Color color;
    private float elapsed;
    private bool active;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPool()
    {
        Pool.Clear();
    }

    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.sortingOrder = 9;
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, lifetime));

        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
        transform.position += drift * Time.deltaTime;

        Color faded = color;
        faded.a = Mathf.Lerp(color.a, 0f, t);
        spriteRenderer.color = faded;

        if (elapsed >= lifetime)
        {
            Release();
        }
    }

    public static void SpawnPulse(Vector3 position, Color color, float startScale = 0.35f, float endScale = 1.2f, float lifetime = 0.3f, int sortingOrder = 9)
    {
        FeedbackEffect effect = Get();
        effect.transform.position = position;
        effect.Configure(color, startScale, endScale, lifetime, sortingOrder, Vector3.zero);
        effect.gameObject.SetActive(true);
    }

    public static void SpawnPickup(Vector3 position)
    {
        SpawnPulse(position, new Color(0.45f, 1f, 0.65f, 0.85f), 0.25f, 0.85f, 0.22f, 10);
    }

    private void Configure(Color effectColor, float start, float end, float duration, int sortingOrder, Vector3 effectDrift)
    {
        color = effectColor;
        elapsed = 0f;
        active = true;
        startScale = start;
        endScale = end;
        lifetime = duration;
        drift = effectDrift;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        transform.localScale = Vector3.one * startScale;
    }

    private static FeedbackEffect Get()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }

        GameObject effectObject = new GameObject("Feedback Pulse");
        FeedbackEffect effect = effectObject.AddComponent<FeedbackEffect>();
        effectObject.SetActive(false);
        return effect;
    }

    private void Release()
    {
        active = false;
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
