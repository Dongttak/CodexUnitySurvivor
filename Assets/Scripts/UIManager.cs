using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static Canvas cachedCanvas;
    private const float StartHintLifetime = 6f;
    private const int HudFontSize = 28;
    private const int TimerFontSize = 34;
    private const int StartHintFontSize = 26;
    private const int GameOverTitleFontSize = 56;
    private const int GameOverDetailsFontSize = 30;
    private const int MenuButtonFontSize = 30;
    private const int PauseTitleFontSize = 54;
    private const int PauseDetailsFontSize = 28;
    private const int StatsTitleFontSize = 34;
    private const int StatsRowFontSize = 25;
    private const int RuntimeStatsTitleFontSize = 26;
    private const int RuntimeStatsRowFontSize = 22;

    private Text hpText;
    private Text levelText;
    private Text xpText;
    private Text timeText;
    private Image hpBarFill;
    private Image xpBarFill;
    private GameObject gameOverPanel;
    private Text gameOverTitleText;
    private Text gameOverDetailsText;
    private GameObject pausePanel;
    private Text pauseTitleText;
    private Text pauseDetailsText;
    private Text statsText;
    private GameObject runtimeStatsPanel;
    private Text runtimeStatsText;
    private Text startHintText;
    private CanvasGroup startHintGroup;
    private float startHintTimer;
    private float statsRefreshTimer;

    private void Awake()
    {
        EnsureCanvas();
        EnsureEventSystem();
        BuildHud();
        BuildGameOverPanel();
        BuildPausePanel();
        BuildRuntimeStatsPanel();
    }

    private void Update()
    {
        if (WasStatsTogglePressed() && CanToggleRuntimeStats())
        {
            ToggleRuntimeStats();
        }

        UpdateRuntimeStatsIfNeeded();

        if (startHintGroup == null || !startHintGroup.gameObject.activeSelf)
        {
            if (pausePanel != null && pausePanel.activeSelf)
            {
                UpdateStatsText();
            }
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

        if (pausePanel != null && pausePanel.activeSelf)
        {
            UpdateStatsText();
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
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

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

        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = "Game Over";
        }

        if (gameOverDetailsText != null)
        {
            gameOverDetailsText.text = $"Survived {minutes:00}:{remainder:00}\nFinal Level {level}\nPress R to Restart";
        }
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowPause()
    {
        if (pausePanel != null)
        {
            UpdateStatsText();
            pausePanel.SetActive(true);
        }
    }

    private void ToggleRuntimeStats()
    {
        if (runtimeStatsPanel != null)
        {
            runtimeStatsPanel.SetActive(!runtimeStatsPanel.activeSelf);
            UpdateRuntimeStatsText();
        }
    }

    public void HidePause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
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

        GameObject leftPanel = EnsurePanel(hud.transform, "Left HUD Panel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(430f, 176f), new Color(0.02f, 0.025f, 0.035f, 0.82f));
        hpText = CreateHudText(leftPanel.transform, "HP Text", "HP 100 / 100", new Vector2(16f, -14f));
        hpBarFill = EnsureProgressBar(leftPanel.transform, "HP Bar", new Vector2(16f, -56f), new Vector2(380f, 20f), new Color(0.2f, 0.95f, 0.35f, 0.95f));
        levelText = CreateHudText(leftPanel.transform, "Level Text", "Level 1", new Vector2(16f, -84f));
        xpText = CreateHudText(leftPanel.transform, "XP Text", "XP 0 / 4", new Vector2(16f, -120f));
        xpBarFill = EnsureProgressBar(leftPanel.transform, "XP Bar", new Vector2(16f, -154f), new Vector2(380f, 16f), new Color(0.35f, 0.78f, 1f, 0.95f));

        GameObject timePanel = EnsurePanel(hud.transform, "Time HUD Panel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(270f, 62f), new Color(0.02f, 0.025f, 0.035f, 0.82f));
        timeText = CreateHudText(timePanel.transform, "Time Text", "Time 00:00", new Vector2(0f, -10f));
        timeText.fontSize = TimerFontSize;
        timeText.rectTransform.sizeDelta = new Vector2(240f, 42f);
        timeText.alignment = TextAnchor.UpperRight;
        timeText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        timeText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        timeText.rectTransform.pivot = new Vector2(0.5f, 1f);

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
        rect.sizeDelta = new Vector2(680f, 430f);
        rect.anchoredPosition = Vector2.zero;

        Image image = EnsureImage(gameOverPanel);
        image.color = new Color(0.06f, 0.035f, 0.045f, 0.96f);

        gameOverTitleText = GetOrCreateText(gameOverPanel.transform, "Game Over Title", "Game Over", GameOverTitleFontSize, TextAnchor.MiddleCenter);
        RectTransform titleRect = gameOverTitleText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -34f);
        titleRect.sizeDelta = new Vector2(-70f, 76f);

        gameOverDetailsText = GetOrCreateText(gameOverPanel.transform, "Game Over Details", "Survived 00:00\nFinal Level 1\nPress R to Restart", GameOverDetailsFontSize, TextAnchor.MiddleCenter);
        RectTransform detailsRect = gameOverDetailsText.rectTransform;
        detailsRect.anchorMin = new Vector2(0f, 0.5f);
        detailsRect.anchorMax = new Vector2(1f, 0.5f);
        detailsRect.pivot = new Vector2(0.5f, 0.5f);
        detailsRect.anchoredPosition = new Vector2(0f, 16f);
        detailsRect.sizeDelta = new Vector2(-80f, 140f);

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

    private void BuildPausePanel()
    {
        Canvas canvas = EnsureCanvas();
        Transform existing = canvas.transform.Find("Pause Panel");
        pausePanel = existing != null ? existing.gameObject : new GameObject("Pause Panel");
        pausePanel.transform.SetParent(canvas.transform, false);

        RectTransform rect = EnsureRectTransform(pausePanel);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(920f, 640f);
        rect.anchoredPosition = Vector2.zero;

        Image image = EnsureImage(pausePanel);
        image.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);

        pauseTitleText = GetOrCreateText(pausePanel.transform, "Pause Title", "Paused", PauseTitleFontSize, TextAnchor.MiddleCenter);
        RectTransform titleRect = pauseTitleText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -34f);
        titleRect.sizeDelta = new Vector2(-70f, 70f);

        pauseDetailsText = GetOrCreateText(pausePanel.transform, "Pause Details", "Press Esc or P to resume", PauseDetailsFontSize, TextAnchor.MiddleCenter);
        RectTransform detailsRect = pauseDetailsText.rectTransform;
        detailsRect.anchorMin = new Vector2(0f, 1f);
        detailsRect.anchorMax = new Vector2(1f, 1f);
        detailsRect.pivot = new Vector2(0.5f, 1f);
        detailsRect.anchoredPosition = new Vector2(0f, -112f);
        detailsRect.sizeDelta = new Vector2(-80f, 46f);

        GameObject statsPanel = EnsurePanel(pausePanel.transform, "Stats Panel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -34f), new Vector2(780f, 330f), new Color(0f, 0f, 0f, 0.36f));
        Text statsTitle = GetOrCreateText(statsPanel.transform, "Stats Title", "Current Stats", StatsTitleFontSize, TextAnchor.UpperCenter);
        RectTransform statsTitleRect = statsTitle.rectTransform;
        statsTitleRect.anchorMin = new Vector2(0f, 1f);
        statsTitleRect.anchorMax = new Vector2(1f, 1f);
        statsTitleRect.pivot = new Vector2(0.5f, 1f);
        statsTitleRect.anchoredPosition = new Vector2(0f, -14f);
        statsTitleRect.sizeDelta = new Vector2(-32f, 44f);

        statsText = GetOrCreateText(statsPanel.transform, "Stats Text", "", StatsRowFontSize, TextAnchor.UpperLeft);
        RectTransform statsRect = statsText.rectTransform;
        statsRect.anchorMin = Vector2.zero;
        statsRect.anchorMax = Vector2.one;
        statsRect.offsetMin = new Vector2(30f, 24f);
        statsRect.offsetMax = new Vector2(-30f, -68f);
        statsText.color = new Color(0.9f, 0.96f, 1f);

        Button resumeButton = CreateMenuButton(pausePanel.transform, "Resume Button", "Resume", new Vector2(-155f, 32f));
        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeFromPause();
            }
        });

        Button restartButton = CreateMenuButton(pausePanel.transform, "Restart Button", "Restart", new Vector2(155f, 32f));
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        });

        pausePanel.SetActive(false);
    }

    private void UpdateStatsText()
    {
        if (statsText == null)
        {
            return;
        }

        AutoWeapon weapon = FindFirstObjectByType<AutoWeapon>();
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
        GameManager gameManager = GameManager.Instance;

        string survivalTime = "00:00";
        if (gameManager != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.SurvivalTime / 60f);
            int seconds = Mathf.FloorToInt(gameManager.SurvivalTime % 60f);
            survivalTime = $"{minutes:00}:{seconds:00}";
        }

        string hp = playerHealth == null ? "-- / --" : $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}";
        string level = levelSystem == null ? "--" : levelSystem.CurrentLevel.ToString();
        string xp = levelSystem == null ? "-- / --" : $"{levelSystem.CurrentXP} / {levelSystem.XPToNextLevel}";
        string damage = weapon == null ? "--" : weapon.Damage.ToString("0.0");
        string fireRate = weapon == null ? "--" : $"{weapon.FireRate:0.00}/s ({weapon.FireInterval:0.00}s)";
        string moveSpeed = playerController == null ? "--" : playerController.MoveSpeed.ToString("0.0");
        string projectileSize = weapon == null ? "--" : $"{weapon.ProjectileSize:0.00}x";
        string projectileCount = weapon == null ? "--" : weapon.ProjectileCount.ToString();
        string xpMagnet = XPOrb.CurrentAttractionRadius.ToString("0.0");
        int activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        statsText.text =
            $"HP: {hp}        Level: {level}        XP: {xp}\n" +
            $"Damage: {damage}        Fire Rate: {fireRate}\n" +
            $"Move Speed: {moveSpeed}        Projectile Size: {projectileSize}        Multi Shot: {projectileCount}\n" +
            $"XP Magnet: {xpMagnet}        Survival Time: {survivalTime}        Active Enemies: {activeEnemies}";
    }

    private static Text CreateHudText(Transform parent, string name, string content, Vector2 anchoredPosition)
    {
        Text text = GetOrCreateText(parent, name, content, HudFontSize, TextAnchor.UpperLeft);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(392f, 38f);
        return text;
    }

    private static Button CreateRestartButton(Transform parent)
    {
        return CreateMenuButton(parent, "Restart Button", "Restart", new Vector2(0f, 36f));
    }

    private static Button CreateMenuButton(Transform parent, string name, string labelText, Vector2 anchoredPosition)
    {
        GameObject buttonObject = GetOrCreateChild(parent, name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = EnsureRectTransform(buttonObject);
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(280f, 66f);

        Image image = EnsureImage(buttonObject);
        image.color = new Color(0.22f, 0.48f, 0.62f);

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }
        button.targetGraphic = image;

        Text label = GetOrCreateText(buttonObject.transform, "Label", labelText, MenuButtonFontSize, TextAnchor.MiddleCenter);
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private void BuildStartHint(Transform parent)
    {
        GameObject hint = EnsurePanel(parent, "Start Hint", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(900f, 112f), new Color(0.02f, 0.03f, 0.04f, 0.82f));
        startHintGroup = hint.GetComponent<CanvasGroup>();
        if (startHintGroup == null)
        {
            startHintGroup = hint.AddComponent<CanvasGroup>();
        }
        startHintGroup.alpha = 1f;
        startHintTimer = 0f;
        hint.SetActive(true);

        startHintText = GetOrCreateText(hint.transform, "Hint Text", "Move: WASD / Arrow Keys    Pause: Esc / P    Tab: Stats\nChoose Upgrade: 1 / 2 / 3 or Click", StartHintFontSize, TextAnchor.MiddleCenter);
        RectTransform textRect = startHintText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 8f);
        textRect.offsetMax = new Vector2(-18f, -8f);
    }

    private void BuildRuntimeStatsPanel()
    {
        Canvas canvas = EnsureCanvas();
        Transform existingHud = canvas.transform.Find("HUD");
        Transform parent = existingHud != null ? existingHud : canvas.transform;
        runtimeStatsPanel = EnsurePanel(parent, "Runtime Stats Panel", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-24f, 0f), new Vector2(330f, 292f), new Color(0.02f, 0.025f, 0.035f, 0.82f));

        Text title = GetOrCreateText(runtimeStatsPanel.transform, "Stats Title", "Player Stats", RuntimeStatsTitleFontSize, TextAnchor.UpperCenter);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -12f);
        titleRect.sizeDelta = new Vector2(-28f, 34f);

        Text hint = GetOrCreateText(runtimeStatsPanel.transform, "Stats Hint", "Tab: Stats", 18, TextAnchor.UpperRight);
        RectTransform hintRect = hint.rectTransform;
        hintRect.anchorMin = new Vector2(0f, 1f);
        hintRect.anchorMax = new Vector2(1f, 1f);
        hintRect.pivot = new Vector2(1f, 1f);
        hintRect.anchoredPosition = new Vector2(-14f, -44f);
        hintRect.sizeDelta = new Vector2(-28f, 24f);
        hint.color = new Color(0.72f, 0.82f, 0.88f);

        runtimeStatsText = GetOrCreateText(runtimeStatsPanel.transform, "Stats Text", "", RuntimeStatsRowFontSize, TextAnchor.UpperLeft);
        RectTransform statsRect = runtimeStatsText.rectTransform;
        statsRect.anchorMin = Vector2.zero;
        statsRect.anchorMax = Vector2.one;
        statsRect.offsetMin = new Vector2(20f, 18f);
        statsRect.offsetMax = new Vector2(-20f, -74f);
        runtimeStatsText.color = new Color(0.9f, 0.96f, 1f);

        runtimeStatsPanel.SetActive(true);
        UpdateRuntimeStatsText();
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

    private void UpdateRuntimeStatsIfNeeded()
    {
        if (runtimeStatsPanel == null || !runtimeStatsPanel.activeSelf)
        {
            return;
        }

        statsRefreshTimer -= Time.unscaledDeltaTime;
        if (statsRefreshTimer > 0f)
        {
            return;
        }

        statsRefreshTimer = 0.25f;
        UpdateRuntimeStatsText();
    }

    private void UpdateRuntimeStatsText()
    {
        if (runtimeStatsText == null)
        {
            return;
        }

        runtimeStatsText.text = BuildCompactStatsText();
    }

    private string BuildCompactStatsText()
    {
        AutoWeapon weapon = FindFirstObjectByType<AutoWeapon>();
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
        GameManager gameManager = GameManager.Instance;

        string hp = playerHealth == null ? "-- / --" : $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}";
        string damage = weapon == null ? "--" : weapon.Damage.ToString("0.0");
        string fireRate = weapon == null ? "--" : $"{weapon.FireRate:0.00}/s";
        string moveSpeed = playerController == null ? "--" : playerController.MoveSpeed.ToString("0.0");
        string projectileSize = weapon == null ? "--" : $"{weapon.ProjectileSize:0.00}x";
        string projectileCount = weapon == null ? "--" : weapon.ProjectileCount.ToString();
        string xpMagnet = XPOrb.CurrentAttractionRadius.ToString("0.0");
        string level = levelSystem == null ? "--" : levelSystem.CurrentLevel.ToString();
        string survivalTime = "--:--";
        if (gameManager != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.SurvivalTime / 60f);
            int seconds = Mathf.FloorToInt(gameManager.SurvivalTime % 60f);
            survivalTime = $"{minutes:00}:{seconds:00}";
        }

        return
            $"HP     {hp}\n" +
            $"DMG    {damage}\n" +
            $"Rate   {fireRate}\n" +
            $"Speed  {moveSpeed}\n" +
            $"Size   {projectileSize}\n" +
            $"Shots  {projectileCount}\n" +
            $"Magnet {xpMagnet}\n" +
            $"Lv/Time {level} / {survivalTime}";
    }

    private static bool CanToggleRuntimeStats()
    {
        GameObject upgradePanel = GameObject.Find("Upgrade Panel");
        bool upgradeVisible = upgradePanel != null && upgradePanel.activeSelf;
        GameObject pause = GameObject.Find("Pause Panel");
        bool pauseVisible = pause != null && pause.activeSelf;
        GameObject gameOver = GameObject.Find("Game Over Panel");
        bool gameOverVisible = gameOver != null && gameOver.activeSelf;
        return !upgradeVisible && !pauseVisible && !gameOverVisible;
    }

    private static bool WasStatsTogglePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.tabKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Tab);
#endif
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
