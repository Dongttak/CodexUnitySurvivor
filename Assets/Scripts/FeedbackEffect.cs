using UnityEngine;

public class FeedbackEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float startScale = 0.35f;
    [SerializeField] private float endScale = 1.2f;
    [SerializeField] private Vector3 drift;

    private SpriteRenderer spriteRenderer;
    private Color color;
    private float elapsed;

    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.sortingOrder = 9;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, lifetime));

        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
        transform.position += drift * Time.deltaTime;

        Color faded = color;
        faded.a = Mathf.Lerp(color.a, 0f, t);
        spriteRenderer.color = faded;

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public static void SpawnPulse(Vector3 position, Color color, float startScale = 0.35f, float endScale = 1.2f, float lifetime = 0.3f, int sortingOrder = 9)
    {
        GameObject effectObject = new GameObject("Feedback Pulse");
        effectObject.transform.position = position;
        FeedbackEffect effect = effectObject.AddComponent<FeedbackEffect>();
        effect.Configure(color, startScale, endScale, lifetime, sortingOrder, Vector3.zero);
    }

    public static void SpawnPickup(Vector3 position)
    {
        SpawnPulse(position, new Color(0.45f, 1f, 0.65f, 0.85f), 0.25f, 0.85f, 0.22f, 10);
    }

    private void Configure(Color effectColor, float start, float end, float duration, int sortingOrder, Vector3 effectDrift)
    {
        color = effectColor;
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
}
