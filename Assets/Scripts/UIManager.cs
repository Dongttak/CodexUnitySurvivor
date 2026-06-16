using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static Canvas cachedCanvas;
    private const float StartHintLifetime = 6f;

    private Text hpText;
    private Text levelText;
    private Text xpText;
    private Text timeText;
    private Image hpBarFill;
    private Image xpBarFill;
    private GameObject gameOverPanel;
    private Text gameOverText;
    private Text startHintText;
    private CanvasGroup startHintGroup;
    private float startHintTimer;

    private void Awake()
    {
        EnsureCanvas();
        EnsureEventSystem();
        BuildHud();
        BuildGameOverPanel();
    }

    private void Update()
    {
        if (startHintGroup == null || !startHintGroup.gameObject.activeSelf)
        {
            return;
        }

        startHintTimer += Time.unscaledDeltaTime;
        float fadeStart = StartHintLifetime - 1.5f;
        if (startHintTimer >= StartHintLifetime)
        {
            startHintGroup.gameObject.SetActive(false);
            return;
        }

        if (startHintTimer > fadeStart)
        {
            startHintGroup.alpha = Mathf.InverseLerp(StartHintLifetime, fadeStart, startHintTimer);
        }
    }

    private void Start()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += SetHealth;
            SetHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
        if (levelSystem != null)
        {
            levelSystem.OnLevelChanged += SetLevel;
            levelSystem.OnXPChanged += SetXP;
            SetLevel(levelSystem.CurrentLevel);
            SetXP(levelSystem.CurrentXP, levelSystem.XPToNextLevel, levelSystem.CurrentLevel);
        }
    }

    public static Canvas EnsureCanvas()
    {
        if (cachedCanvas != null)
        {
            return cachedCanvas;
        }

        cachedCanvas = FindFirstObjectByType<Canvas>();
        if (cachedCanvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            cachedCanvas = canvasObject.AddComponent<Canvas>();
            cachedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        CanvasScaler scaler = cachedCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = cachedCanvas.gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        if (cachedCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            cachedCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        return cachedCanvas;
    }

    public static Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        EnsureRectTransform(textObject);
        Text text = textObject.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return text;
    }

    public void SetHealth(float current, float maximum)
    {
        if (hpText != null)
        {
            hpText.text = $"HP {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
        }

        if (hpBarFill != null)
        {
            hpBarFill.fillAmount = maximum <= 0f ? 0f : Mathf.Clamp01(current / maximum);
        }
    }

    public void SetLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }

    public void SetXP(int current, int required, int level)
    {
        if (xpText != null)
        {
            xpText.text = $"XP {current} / {required}";
        }

        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = required <= 0 ? 0f : Mathf.Clamp01((float)current / required);
        }
    }

    public void SetSurvivalTime(float seconds)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int remainder = Mathf.FloorToInt(seconds % 60f);
            timeText.text = $"Time {minutes:00}:{remainder:00}";
        }
    }

    public void ShowGameOver(float seconds)
    {
        if (gameOverPanel == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainder = Mathf.FloorToInt(seconds % 60f);
        int level = 1;
        LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
        if (levelSystem != null)
        {
            level = levelSystem.CurrentLevel;
        }

        gameOverText.text = $"Game Over\nSurvived {minutes:00}:{remainder:00}\nFinal Level {level}\nPress R or Restart";
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void BuildHud()
    {
        Canvas canvas = EnsureCanvas();
        Transform existing = canvas.transform.Find("HUD");
        GameObject hud = existing != null ? existing.gameObject : new GameObject("HUD");
        hud.transform.SetParent(canvas.transform, false);

        RectTransform rect = EnsureRectTransform(hud);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject leftPanel = EnsurePanel(hud.transform, "Left HUD Panel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(22f, -22f), new Vector2(390f, 154f), new Color(0.03f, 0.04f, 0.06f, 0.72f));
        hpText = CreateHudText(leftPanel.transform, "HP Text", "HP 100 / 100", new Vector2(16f, -14f));
        hpBarFill = EnsureProgressBar(leftPanel.transform, "HP Bar", new Vector2(16f, -52f), new Vector2(340f, 18f), new Color(0.2f, 0.95f, 0.35f, 0.95f));
        levelText = CreateHudText(leftPanel.transform, "Level Text", "Level 1", new Vector2(16f, -76f));
        xpText = CreateHudText(leftPanel.transform, "XP Text", "XP 0 / 4", new Vector2(16f, -108f));
        xpBarFill = EnsureProgressBar(leftPanel.transform, "XP Bar", new Vector2(16f, -138f), new Vector2(340f, 14f), new Color(0.35f, 0.78f, 1f, 0.95f));

        GameObject timePanel = EnsurePanel(hud.transform, "Time HUD Panel", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-22f, -22f), new Vector2(230f, 52f), new Color(0.03f, 0.04f, 0.06f, 0.72f));
        timeText = CreateHudText(timePanel.transform, "Time Text", "Time 00:00", new Vector2(-14f, -10f));
        timeText.alignment = TextAnchor.UpperRight;
        timeText.rectTransform.anchorMin = new Vector2(1f, 1f);
        timeText.rectTransform.anchorMax = new Vector2(1f, 1f);
        timeText.rectTransform.pivot = new Vector2(1f, 1f);

        BuildStartHint(hud.transform);
        HideLegacyDirectHudText(hud.transform);
    }

    private void BuildGameOverPanel()
    {
        Canvas canvas = EnsureCanvas();
        Transform existing = canvas.transform.Find("Game Over Panel");
        gameOverPanel = existing != null ? existing.gameObject : new GameObject("Game Over Panel");
        gameOverPanel.transform.SetParent(canvas.transform, false);

        RectTransform rect = EnsureRectTransform(gameOverPanel);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(520f, 330f);
        rect.anchoredPosition = Vector2.zero;

        Image image = EnsureImage(gameOverPanel);
        image.color = new Color(0.08f, 0.04f, 0.05f, 0.95f);

        gameOverText = GetOrCreateText(gameOverPanel.transform, "Game Over Text", "Game Over", 34, TextAnchor.MiddleCenter);
        RectTransform textRect = gameOverText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(34f, 92f);
        textRect.offsetMax = new Vector2(-34f, -30f);

        Button restartButton = CreateRestartButton(gameOverPanel.transform);
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        });

        gameOverPanel.SetActive(false);
    }

    private static Text CreateHudText(Transform parent, string name, string content, Vector2 anchoredPosition)
    {
        Text text = GetOrCreateText(parent, name, content, 23, TextAnchor.UpperLeft);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(360f, 34f);
        return text;
    }

    private static Button CreateRestartButton(Transform parent)
    {
        GameObject buttonObject = GetOrCreateChild(parent, "Restart Button");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = EnsureRectTransform(buttonObject);
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 32f);
        rect.sizeDelta = new Vector2(250f, 58f);

        Image image = EnsureImage(buttonObject);
        image.color = new Color(0.22f, 0.48f, 0.62f);

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }
        button.targetGraphic = image;

        Text label = GetOrCreateText(buttonObject.transform, "Label", "Restart", 23, TextAnchor.MiddleCenter);
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private void BuildStartHint(Transform parent)
    {
        GameObject hint = EnsurePanel(parent, "Start Hint", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 34f), new Vector2(660f, 82f), new Color(0.02f, 0.03f, 0.04f, 0.72f));
        startHintGroup = hint.GetComponent<CanvasGroup>();
        if (startHintGroup == null)
        {
            startHintGroup = hint.AddComponent<CanvasGroup>();
        }
        startHintGroup.alpha = 1f;
        startHintTimer = 0f;
        hint.SetActive(true);

        startHintText = GetOrCreateText(hint.transform, "Hint Text", "Move: WASD / Arrow Keys\nSurvive, collect XP, and choose upgrades", 22, TextAnchor.MiddleCenter);
        RectTransform textRect = startHintText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 8f);
        textRect.offsetMax = new Vector2(-18f, -8f);
    }

    private static GameObject EnsurePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject panel = GetOrCreateChild(parent, name);
        RectTransform rect = EnsureRectTransform(panel);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = EnsureImage(panel);
        image.color = color;
        image.raycastTarget = false;
        return panel;
    }

    private static Image EnsureProgressBar(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color fillColor)
    {
        GameObject root = EnsurePanel(parent, name, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), anchoredPosition, size, new Color(0f, 0f, 0f, 0.58f));
        GameObject fillObject = GetOrCreateChild(root.transform, "Fill");
        RectTransform fillRect = EnsureRectTransform(fillObject);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);

        Image fill = EnsureImage(fillObject);
        fill.color = fillColor;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;
        return fill;
    }

    private static Text GetOrCreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment)
    {
        Transform existing = parent.Find(name);
        Text text = existing != null ? existing.GetComponent<Text>() : null;
        if (text == null)
        {
            GameObject textObject = existing != null ? existing.gameObject : new GameObject(name);
            textObject.transform.SetParent(parent, false);
            EnsureRectTransform(textObject);
            text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static void HideLegacyDirectHudText(Transform hud)
    {
        string[] legacyNames = { "HP Text", "Level Text", "XP Text", "Time Text" };
        foreach (string legacyName in legacyNames)
        {
            Transform legacy = hud.Find(legacyName);
            if (legacy != null)
            {
                legacy.gameObject.SetActive(false);
            }
        }
    }

    private static GameObject GetOrCreateChild(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        return child;
    }

    private static RectTransform EnsureRectTransform(GameObject target)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = target.AddComponent<RectTransform>();
        }

        return rect;
    }

    private static Image EnsureImage(GameObject target)
    {
        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            image = target.AddComponent<Image>();
        }

        return image;
    }

    private static void EnsureEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

#if ENABLE_INPUT_SYSTEM
        if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }
#else
        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }
#endif
    }
}
