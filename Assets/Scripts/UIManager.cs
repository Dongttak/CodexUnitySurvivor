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
    private const int HudFontSize = 32;
    private const int TimerFontSize = 40;
    private const int StartHintFontSize = 30;
    private const int GameOverTitleFontSize = 64;
    private const int GameOverDetailsFontSize = 36;
    private const int MenuButtonFontSize = 34;
    private const int PauseTitleFontSize = 60;
    private const int PauseDetailsFontSize = 34;
    private const int StatsTitleFontSize = 36;
    private const int StatsRowFontSize = 28;
    private const int RuntimeStatsTitleFontSize = 32;
    private const int RuntimeStatsRowFontSize = 26;
    private const int SmallHintFontSize = 22;
    private const int AnnouncementFontSize = 44;
    private const float StandardMargin = 32f;
    private const float PanelAlpha = 0.86f;
    private const float AnnouncementLifetime = 3f;

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
    private GameObject announcementPanel;
    private Text announcementText;
    private CanvasGroup announcementGroup;
    private float startHintTimer;
    private float announcementTimer;
    private float statsRefreshTimer;

    private void Awake()
    {
        EnsureCanvas();
        EnsureEventSystem();
        BuildHud();
        BuildGameOverPanel();
        BuildPausePanel();
        BuildRuntimeStatsPanel();
        BuildAnnouncementPanel();
    }

    private void Update()
    {
        UpdateAnnouncement();

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
            hpText.text = $"체력 {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
        }

        if (hpBarFill != null)
        {
            SetProgressBarFill(hpBarFill, maximum <= 0f ? 0f : current / maximum);
        }
    }

    public void SetLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"레벨 {level}";
        }
    }

    public void SetXP(int current, int required, int level)
    {
        if (xpText != null)
        {
            xpText.text = $"경험치 {current} / {required}";
        }

        if (xpBarFill != null)
        {
            SetProgressBarFill(xpBarFill, required <= 0 ? 0f : (float)current / required);
        }
    }

    public void SetSurvivalTime(float seconds)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int remainder = Mathf.FloorToInt(seconds % 60f);
            timeText.text = $"시간 {minutes:00}:{remainder:00}";
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
        int kills = 0;
        LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
        if (levelSystem != null)
        {
            level = levelSystem.CurrentLevel;
        }
        if (GameManager.Instance != null)
        {
            kills = GameManager.Instance.KillCount;
        }

        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = "게임 오버";
        }

        if (gameOverDetailsText != null)
        {
            gameOverDetailsText.text =
                $"생존 시간 {minutes:00}:{remainder:00}\n" +
                $"최종 레벨 {level}\n" +
                $"처치 수 {kills}\n" +
                $"해금한 무기: {BuildUnlockedWeaponList()}\n" +
                "R 키로 다시 시작";
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

        GameObject leftPanel = EnsurePanel(hud.transform, "Left HUD Panel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(StandardMargin, -StandardMargin), new Vector2(500f, 206f), StandardPanelColor());
        hpText = CreateHudText(leftPanel.transform, "HP Text", "체력 100 / 100", new Vector2(20f, -16f));
        hpBarFill = EnsureProgressBar(leftPanel.transform, "HP Bar", new Vector2(20f, -62f), new Vector2(440f, 24f), new Color(0.2f, 0.95f, 0.35f, 0.95f));
        levelText = CreateHudText(leftPanel.transform, "Level Text", "레벨 1", new Vector2(20f, -96f));
        xpText = CreateHudText(leftPanel.transform, "XP Text", "경험치 0 / 4", new Vector2(20f, -136f));
        xpBarFill = EnsureProgressBar(leftPanel.transform, "XP Bar", new Vector2(20f, -176f), new Vector2(440f, 18f), new Color(0.35f, 0.78f, 1f, 0.95f));

        GameObject timePanel = EnsurePanel(hud.transform, "Time HUD Panel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -StandardMargin), new Vector2(320f, 76f), StandardPanelColor());
        timeText = CreateHudText(timePanel.transform, "Time Text", "시간 00:00", new Vector2(0f, -12f));
        timeText.fontSize = TimerFontSize;
        timeText.rectTransform.sizeDelta = new Vector2(290f, 50f);
        timeText.alignment = TextAnchor.UpperCenter;
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
        rect.sizeDelta = new Vector2(860f, 560f);
        rect.anchoredPosition = Vector2.zero;

        Image image = EnsureImage(gameOverPanel);
        image.color = new Color(0.05f, 0.035f, 0.045f, 0.96f);

        gameOverTitleText = GetOrCreateText(gameOverPanel.transform, "Game Over Title", "게임 오버", GameOverTitleFontSize, TextAnchor.MiddleCenter);
        RectTransform titleRect = gameOverTitleText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -36f);
        titleRect.sizeDelta = new Vector2(-80f, 88f);

        gameOverDetailsText = GetOrCreateText(gameOverPanel.transform, "Game Over Details", "생존 시간 00:00\n최종 레벨 1\n처치 수 0\n해금한 무기: 기본 탄\nR 키로 다시 시작", GameOverDetailsFontSize, TextAnchor.MiddleCenter);
        RectTransform detailsRect = gameOverDetailsText.rectTransform;
        detailsRect.anchorMin = new Vector2(0f, 0.5f);
        detailsRect.anchorMax = new Vector2(1f, 0.5f);
        detailsRect.pivot = new Vector2(0.5f, 0.5f);
        detailsRect.anchoredPosition = new Vector2(0f, 18f);
        detailsRect.sizeDelta = new Vector2(-100f, 240f);

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
        rect.sizeDelta = new Vector2(980f, 690f);
        rect.anchoredPosition = Vector2.zero;

        Image image = EnsureImage(pausePanel);
        image.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);

        pauseTitleText = GetOrCreateText(pausePanel.transform, "Pause Title", "일시정지", PauseTitleFontSize, TextAnchor.MiddleCenter);
        RectTransform titleRect = pauseTitleText.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -34f);
        titleRect.sizeDelta = new Vector2(-80f, 78f);

        pauseDetailsText = GetOrCreateText(pausePanel.transform, "Pause Details", "Esc 또는 P: 계속\n오디오 테스트: T", PauseDetailsFontSize, TextAnchor.MiddleCenter);
        RectTransform detailsRect = pauseDetailsText.rectTransform;
        detailsRect.anchorMin = new Vector2(0f, 1f);
        detailsRect.anchorMax = new Vector2(1f, 1f);
        detailsRect.pivot = new Vector2(0.5f, 1f);
        detailsRect.anchoredPosition = new Vector2(0f, -118f);
        detailsRect.sizeDelta = new Vector2(-90f, 52f);

        GameObject statsPanel = EnsurePanel(pausePanel.transform, "Stats Panel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -34f), new Vector2(830f, 350f), new Color(0f, 0f, 0f, 0.38f));
        Text statsTitle = GetOrCreateText(statsPanel.transform, "Stats Title", "능력치", StatsTitleFontSize, TextAnchor.UpperCenter);
        RectTransform statsTitleRect = statsTitle.rectTransform;
        statsTitleRect.anchorMin = new Vector2(0f, 1f);
        statsTitleRect.anchorMax = new Vector2(1f, 1f);
        statsTitleRect.pivot = new Vector2(0.5f, 1f);
        statsTitleRect.anchoredPosition = new Vector2(0f, -14f);
        statsTitleRect.sizeDelta = new Vector2(-36f, 48f);

        statsText = GetOrCreateText(statsPanel.transform, "Stats Text", "", StatsRowFontSize, TextAnchor.UpperLeft);
        RectTransform statsRect = statsText.rectTransform;
        statsRect.anchorMin = Vector2.zero;
        statsRect.anchorMax = Vector2.one;
        statsRect.offsetMin = new Vector2(34f, 24f);
        statsRect.offsetMax = new Vector2(-34f, -72f);
        statsText.color = new Color(0.9f, 0.96f, 1f);

        Button resumeButton = CreateMenuButton(pausePanel.transform, "Resume Button", "계속하기", new Vector2(-170f, 36f));
        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeFromPause();
            }
        });

        Button restartButton = CreateMenuButton(pausePanel.transform, "Restart Button", "다시 시작", new Vector2(170f, 36f));
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
        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
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
        string aura = BuildAuraStats(weaponController);
        string orbit = BuildOrbitStats(weaponController);
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        string wave = spawner == null ? "--" : spawner.CurrentPhaseNameKorean;
        string elite = spawner == null ? "--" : spawner.EliteSpawned ? "출현" : "대기";
        int kills = gameManager == null ? 0 : gameManager.KillCount;
        int activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        statsText.text =
            $"체력: {hp}        레벨: {level}        경험치: {xp}\n" +
            $"공격력: {damage}        공격 속도: {fireRate}\n" +
            $"이동 속도: {moveSpeed}        탄 크기: {projectileSize}        발사 수: {projectileCount}\n" +
            $"경험치 흡수 범위: {xpMagnet}        생존 시간: {survivalTime}        활성 적: {activeEnemies}\n" +
            $"웨이브: {wave}        처치 수: {kills}        정예 적: {elite}\n" +
            $"오라 파동: {aura}        회전 칼날: {orbit}";
    }

    private static Text CreateHudText(Transform parent, string name, string content, Vector2 anchoredPosition)
    {
        Text text = GetOrCreateText(parent, name, content, HudFontSize, TextAnchor.UpperLeft);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(452f, 42f);
        return text;
    }

    private static Button CreateRestartButton(Transform parent)
    {
        return CreateMenuButton(parent, "Restart Button", "다시 시작", new Vector2(0f, 36f));
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
        rect.sizeDelta = new Vector2(320f, 72f);

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
        GameObject hint = EnsurePanel(parent, "Start Hint", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(1060f, 128f), StandardPanelColor());
        startHintGroup = hint.GetComponent<CanvasGroup>();
        if (startHintGroup == null)
        {
            startHintGroup = hint.AddComponent<CanvasGroup>();
        }
        startHintGroup.alpha = 1f;
        startHintTimer = 0f;
        hint.SetActive(true);

        startHintText = GetOrCreateText(hint.transform, "Hint Text", "이동: WASD / 방향키    일시정지: Esc / P    Tab: 능력치    오디오 테스트: T\n강화 선택: 1 / 2 / 3 또는 클릭", StartHintFontSize, TextAnchor.MiddleCenter);
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
        runtimeStatsPanel = EnsurePanel(parent, "Runtime Stats Panel", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-StandardMargin, 0f), new Vector2(380f, 420f), StandardPanelColor());

        Text title = GetOrCreateText(runtimeStatsPanel.transform, "Stats Title", "능력치", RuntimeStatsTitleFontSize, TextAnchor.UpperCenter);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -14f);
        titleRect.sizeDelta = new Vector2(-32f, 40f);

        Text hint = GetOrCreateText(runtimeStatsPanel.transform, "Stats Hint", "Tab: 능력치", SmallHintFontSize, TextAnchor.UpperRight);
        RectTransform hintRect = hint.rectTransform;
        hintRect.anchorMin = new Vector2(0f, 1f);
        hintRect.anchorMax = new Vector2(1f, 1f);
        hintRect.pivot = new Vector2(1f, 1f);
        hintRect.anchoredPosition = new Vector2(-18f, -52f);
        hintRect.sizeDelta = new Vector2(-36f, 28f);
        hint.color = new Color(0.72f, 0.82f, 0.88f);

        runtimeStatsText = GetOrCreateText(runtimeStatsPanel.transform, "Stats Text", "", RuntimeStatsRowFontSize, TextAnchor.UpperLeft);
        RectTransform statsRect = runtimeStatsText.rectTransform;
        statsRect.anchorMin = Vector2.zero;
        statsRect.anchorMax = Vector2.one;
        statsRect.offsetMin = new Vector2(24f, 22f);
        statsRect.offsetMax = new Vector2(-24f, -86f);
        runtimeStatsText.color = new Color(0.9f, 0.96f, 1f);

        runtimeStatsPanel.SetActive(true);
        UpdateRuntimeStatsText();
    }

    private void BuildAnnouncementPanel()
    {
        Canvas canvas = EnsureCanvas();
        Transform existingHud = canvas.transform.Find("HUD");
        Transform parent = existingHud != null ? existingHud : canvas.transform;
        announcementPanel = EnsurePanel(parent, "Announcement Panel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -134f), new Vector2(900f, 92f), new Color(0.02f, 0.03f, 0.04f, 0.74f));
        announcementGroup = announcementPanel.GetComponent<CanvasGroup>();
        if (announcementGroup == null)
        {
            announcementGroup = announcementPanel.AddComponent<CanvasGroup>();
        }
        announcementGroup.blocksRaycasts = false;
        announcementGroup.interactable = false;

        announcementText = GetOrCreateText(announcementPanel.transform, "Announcement Text", "", AnnouncementFontSize, TextAnchor.MiddleCenter);
        RectTransform textRect = announcementText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(28f, 10f);
        textRect.offsetMax = new Vector2(-28f, -10f);
        announcementText.color = new Color(1f, 0.92f, 0.44f);

        announcementPanel.SetActive(false);
    }

    public void ShowAnnouncement(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (announcementPanel == null)
        {
            BuildAnnouncementPanel();
        }

        announcementText.text = message;
        announcementTimer = AnnouncementLifetime;
        announcementGroup.alpha = 1f;
        announcementPanel.SetActive(true);
    }

    private void UpdateAnnouncement()
    {
        if (announcementPanel == null || !announcementPanel.activeSelf || announcementGroup == null)
        {
            return;
        }

        announcementTimer -= Time.unscaledDeltaTime;
        if (announcementTimer <= 0f)
        {
            announcementPanel.SetActive(false);
            return;
        }

        announcementGroup.alpha = announcementTimer < 0.75f ? Mathf.Clamp01(announcementTimer / 0.75f) : 1f;
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

    private static Color StandardPanelColor()
    {
        return new Color(0.02f, 0.025f, 0.035f, PanelAlpha);
    }

    private static Image EnsureProgressBar(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color fillColor)
    {
        GameObject root = EnsurePanel(parent, name, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), anchoredPosition, size, new Color(0f, 0f, 0f, 0.58f));
        GameObject fillObject = GetOrCreateChild(root.transform, "Fill");
        RectTransform fillRect = EnsureRectTransform(fillObject);
        fillRect.anchorMin = new Vector2(0f, 0.5f);
        fillRect.anchorMax = new Vector2(0f, 0.5f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = new Vector2(2f, 0f);
        fillRect.sizeDelta = new Vector2(Mathf.Max(0f, size.x - 4f), Mathf.Max(0f, size.y - 4f));

        Image fill = EnsureImage(fillObject);
        fill.color = fillColor;
        fill.type = Image.Type.Simple;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;
        return fill;
    }

    private static void SetProgressBarFill(Image fill, float rawRatio)
    {
        if (fill == null)
        {
            return;
        }

        float ratio = Mathf.Clamp01(rawRatio);
        RectTransform fillRect = fill.rectTransform;
        RectTransform parentRect = fillRect.parent as RectTransform;
        if (parentRect == null)
        {
            fill.fillAmount = ratio;
            return;
        }

        float parentWidth = parentRect.rect.width > 0f ? parentRect.rect.width : parentRect.sizeDelta.x;
        float parentHeight = parentRect.rect.height > 0f ? parentRect.rect.height : parentRect.sizeDelta.y;
        float maxWidth = Mathf.Max(0f, parentWidth - 4f);
        float height = Mathf.Max(0f, parentHeight - 4f);

        // The bars are generated at runtime, so resize the fill rect directly instead of
        // relying on Image.Type.Filled sprite behavior.
        fillRect.anchorMin = new Vector2(0f, 0.5f);
        fillRect.anchorMax = new Vector2(0f, 0.5f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = new Vector2(2f, 0f);
        fillRect.sizeDelta = new Vector2(maxWidth * ratio, height);
        fill.enabled = ratio > 0.001f;
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
        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
        GameManager gameManager = GameManager.Instance;

        string hp = playerHealth == null ? "-- / --" : $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}";
        string damage = weapon == null ? "--" : weapon.Damage.ToString("0.0");
        string fireRate = weapon == null ? "--" : $"{weapon.FireRate:0.00}/s";
        string moveSpeed = playerController == null ? "--" : playerController.MoveSpeed.ToString("0.0");
        string projectileSize = weapon == null ? "--" : $"{weapon.ProjectileSize:0.00}x";
        string projectileCount = weapon == null ? "--" : weapon.ProjectileCount.ToString();
        string xpMagnet = XPOrb.CurrentAttractionRadius.ToString("0.0");
        string level = levelSystem == null ? "--" : levelSystem.CurrentLevel.ToString();
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        string wave = spawner == null ? "--" : spawner.CurrentPhaseNameKorean;
        string elite = spawner == null ? "--" : spawner.EliteSpawned ? "출현" : "대기";
        string kills = gameManager == null ? "0" : gameManager.KillCount.ToString();
        string survivalTime = "--:--";
        if (gameManager != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.SurvivalTime / 60f);
            int seconds = Mathf.FloorToInt(gameManager.SurvivalTime % 60f);
            survivalTime = $"{minutes:00}:{seconds:00}";
        }

        return
            $"체력   {hp}\n" +
            $"공격력 {damage}\n" +
            $"공속   {fireRate}\n" +
            $"이속   {moveSpeed}\n" +
            $"탄크기 {projectileSize}\n" +
            $"발사수 {projectileCount}\n" +
            $"흡수   {xpMagnet}\n" +
            $"웨이브 {wave}\n" +
            $"처치   {kills} / 정예 {elite}\n" +
            $"무기   오라 {BuildWeaponShortLabel(weaponController, true)} / 칼날 {BuildWeaponShortLabel(weaponController, false)}\n" +
            $"레벨/시간 {level} / {survivalTime}";
    }

    private static string BuildAuraStats(WeaponController weaponController)
    {
        if (weaponController == null || !weaponController.HasAuraPulse || weaponController.AuraPulse == null)
        {
            return "잠김";
        }

        AuraPulseWeapon aura = weaponController.AuraPulse;
        return $"해금 {aura.Damage:0.0} 피해 / {aura.Radius:0.0}범위 / {aura.Cooldown:0.0}초";
    }

    private static string BuildOrbitStats(WeaponController weaponController)
    {
        if (weaponController == null || !weaponController.HasOrbitBlade || weaponController.OrbitBlade == null)
        {
            return "잠김";
        }

        OrbitBladeWeapon orbit = weaponController.OrbitBlade;
        return $"해금 {orbit.OrbitCount}개 / {orbit.OrbitRadius:0.0}범위";
    }

    private static string BuildWeaponShortLabel(WeaponController weaponController, bool aura)
    {
        if (weaponController == null)
        {
            return "잠김";
        }

        return aura
            ? weaponController.HasAuraPulse ? "해금" : "잠김"
            : weaponController.HasOrbitBlade ? "해금" : "잠김";
    }

    private static string BuildUnlockedWeaponList()
    {
        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
        string weapons = "기본 탄";
        if (weaponController != null && weaponController.HasAuraPulse)
        {
            weapons += ", 오라 파동";
        }

        if (weaponController != null && weaponController.HasOrbitBlade)
        {
            weapons += ", 회전 칼날";
        }

        return weapons;
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
