using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private static readonly Queue<DamageNumber> Pool = new Queue<DamageNumber>();

    [SerializeField] private float lifetime = 0.45f;
    [SerializeField] private float riseSpeed = 0.95f;

    private TextMesh textMesh;
    private MeshRenderer meshRenderer;
    private Color baseColor;
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
        textMesh.fontSize = 28;
        textMesh.characterSize = 0.052f;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 20;
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

        if (elapsed >= lifetime)
        {
            Release();
        }
    }

    public static void Spawn(Vector3 position, float damage)
    {
        DamageNumber number = Get();
        number.transform.position = position + new Vector3(Random.Range(-0.1f, 0.1f), 0.42f, 0f);
        number.Configure(Mathf.CeilToInt(damage).ToString(), new Color(1f, 0.95f, 0.55f, 1f));
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
        textMesh.text = text;
        textMesh.color = color;
    }

    private void Release()
    {
        active = false;
        gameObject.SetActive(false);
        Pool.Enqueue(this);
    }
}
