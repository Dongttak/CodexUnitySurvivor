using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static Canvas cachedCanvas;

    private Text hpText;
    private Text levelText;
    private Text xpText;
    private Text timeText;
    private GameObject gameOverPanel;
    private Text gameOverText;

    private void Awake()
    {
        EnsureCanvas();
        EnsureEventSystem();
        BuildHud();
        BuildGameOverPanel();
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
        gameOverText.text = $"Game Over\nSurvived {minutes:00}:{remainder:00}\nPress R or Restart";
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
        if (existing != null)
        {
            return;
        }

        GameObject hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform, false);

        RectTransform rect = hud.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        hpText = CreateHudText(hud.transform, "HP Text", "HP 100 / 100", new Vector2(28f, -28f));
        levelText = CreateHudText(hud.transform, "Level Text", "Level 1", new Vector2(28f, -68f));
        xpText = CreateHudText(hud.transform, "XP Text", "XP 0 / 4", new Vector2(28f, -108f));
        timeText = CreateHudText(hud.transform, "Time Text", "Time 00:00", new Vector2(-28f, -28f));
        timeText.alignment = TextAnchor.UpperRight;
        timeText.rectTransform.anchorMin = new Vector2(1f, 1f);
        timeText.rectTransform.anchorMax = new Vector2(1f, 1f);
        timeText.rectTransform.pivot = new Vector2(1f, 1f);
    }

    private void BuildGameOverPanel()
    {
        Canvas canvas = EnsureCanvas();
        Transform existing = canvas.transform.Find("Game Over Panel");
        if (existing != null)
        {
            gameOverPanel = existing.gameObject;
            gameOverText = existing.GetComponentInChildren<Text>();
            return;
        }

        gameOverPanel = new GameObject("Game Over Panel");
        gameOverPanel.transform.SetParent(canvas.transform, false);

        RectTransform rect = gameOverPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(460f, 260f);
        rect.anchoredPosition = Vector2.zero;

        Image image = gameOverPanel.AddComponent<Image>();
        image.color = new Color(0.08f, 0.04f, 0.05f, 0.95f);

        gameOverText = CreateText(gameOverPanel.transform, "Game Over Text", "Game Over", 32, TextAnchor.MiddleCenter);
        RectTransform textRect = gameOverText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(24f, 24f);
        textRect.offsetMax = new Vector2(-24f, -78f);

        Button restartButton = CreateRestartButton(gameOverPanel.transform);
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
        Text text = CreateText(parent, name, content, 25, TextAnchor.UpperLeft);
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
        GameObject buttonObject = new GameObject("Restart Button");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 28f);
        rect.sizeDelta = new Vector2(220f, 52f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.25f, 0.4f, 0.52f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        Text label = CreateText(buttonObject.transform, "Label", "Restart", 22, TextAnchor.MiddleCenter);
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
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
