using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private static readonly Queue<DamageNumber> Pool = new Queue<DamageNumber>();

    [SerializeField] private float lifetime = 0.85f;
    [SerializeField] private float riseSpeed = 0.8f;

    private TextMesh textMesh;
    private TextMesh shadowTextMesh;
    private MeshRenderer meshRenderer;
    private MeshRenderer shadowRenderer;
    private Color baseColor;
    private Color shadowColor;
    private float elapsed;
    private bool active;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPool()
    {
        Pool.Clear();
    }

    private void Awake()
    {
        textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 52;
        textMesh.characterSize = 0.085f;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 21;

        GameObject shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform, false);
        shadowObject.transform.localPosition = new Vector3(0.035f, -0.035f, 0.01f);
        shadowTextMesh = shadowObject.AddComponent<TextMesh>();
        shadowTextMesh.anchor = TextAnchor.MiddleCenter;
        shadowTextMesh.alignment = TextAlignment.Center;
        shadowTextMesh.fontSize = textMesh.fontSize;
        shadowTextMesh.characterSize = textMesh.characterSize;
        shadowRenderer = shadowObject.GetComponent<MeshRenderer>();
        shadowRenderer.sortingOrder = 20;
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        Color faded = baseColor;
        faded.a = Mathf.Lerp(baseColor.a, 0f, t);
        textMesh.color = faded;
        Color fadedShadow = shadowColor;
        fadedShadow.a = Mathf.Lerp(shadowColor.a, 0f, t);
        shadowTextMesh.color = fadedShadow;

        if (elapsed >= lifetime)
        {
            Release();
        }
    }

    public static void Spawn(Vector3 position, float damage)
    {
        DamageNumber number = Get();
        number.transform.position = position + new Vector3(Random.Range(-0.12f, 0.12f), 0.5f, 0f);
        number.transform.localScale = Vector3.one;
        number.Configure(Mathf.CeilToInt(damage).ToString(), new Color(1f, 0.96f, 0.22f, 1f));
        number.gameObject.SetActive(true);
    }

    private static DamageNumber Get()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }

        GameObject numberObject = new GameObject("Damage Number");
        DamageNumber number = numberObject.AddComponent<DamageNumber>();
        numberObject.SetActive(false);
        return number;
    }

    private void Configure(string text, Color color)
    {
        elapsed = 0f;
        active = true;
        baseColor = color;
        shadowColor = new Color(0f, 0f, 0f, 0.88f);
        textMesh.fontSize = 52;
        textMesh.characterSize = 0.085f;
        textMesh.text = text;
        textMesh.color = color;
        if (shadowTextMesh != null)
        {
            shadowTextMesh.fontSize = textMesh.fontSize;
            shadowTextMesh.characterSize = textMesh.characterSize;
            shadowTextMesh.text = text;
            shadowTextMesh.color = shadowColor;
            shadowTextMesh.transform.localPosition = new Vector3(0.035f, -0.035f, 0.01f);
        }
    }

    private void Release()
    {
        active = false;
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
