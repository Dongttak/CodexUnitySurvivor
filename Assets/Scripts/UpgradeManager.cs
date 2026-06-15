using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private GameObject panel;
    private Button damageButton;
    private Button fireRateButton;
    private Button moveSpeedButton;
    private Text titleText;
    private LevelSystem activeLevelSystem;

    private void Update()
    {
        if (panel == null || !panel.activeSelf || activeLevelSystem == null)
        {
            return;
        }

        if (WasChoicePressed(1))
        {
            Choose(activeLevelSystem, UpgradeType.ProjectileDamage);
        }
        else if (WasChoicePressed(2))
        {
            Choose(activeLevelSystem, UpgradeType.FireRate);
        }
        else if (WasChoicePressed(3))
        {
            Choose(activeLevelSystem, UpgradeType.MoveSpeed);
        }
    }

    public void ShowChoices(LevelSystem levelSystem)
    {
        EnsurePanel();

        activeLevelSystem = levelSystem;
        titleText.text = "Level Up - Choose 1 Upgrade";
        ConfigureButton(damageButton, "1  Sharper Projectiles\nMore damage per hit", () => Choose(levelSystem, UpgradeType.ProjectileDamage));
        ConfigureButton(fireRateButton, "2  Faster Casting\nShoot a little more often", () => Choose(levelSystem, UpgradeType.FireRate));
        ConfigureButton(moveSpeedButton, "3  Quicker Footwork\nMove faster to escape crowds", () => Choose(levelSystem, UpgradeType.MoveSpeed));
        panel.SetActive(true);
    }

    public void HideChoices()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        activeLevelSystem = null;
    }

    private void Choose(LevelSystem levelSystem, UpgradeType upgradeType)
    {
        HideChoices();
        levelSystem.ApplyUpgrade(upgradeType);
    }

    private void EnsurePanel()
    {
        if (panel != null)
        {
            return;
        }

        Canvas canvas = UIManager.EnsureCanvas();
        panel = new GameObject("Upgrade Panel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500f, 340f);
        rect.anchoredPosition = Vector2.zero;

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.05f, 0.06f, 0.08f, 0.94f);

        titleText = UIManager.CreateText(panel.transform, "Title", "Level Up", 32, TextAnchor.MiddleCenter);
        SetRect(titleText.rectTransform, new Vector2(0f, 122f), new Vector2(450f, 48f));

        damageButton = CreateChoiceButton("Damage Button", new Vector2(0f, 48f));
        fireRateButton = CreateChoiceButton("Fire Rate Button", new Vector2(0f, -36f));
        moveSpeedButton = CreateChoiceButton("Move Speed Button", new Vector2(0f, -120f));

        panel.SetActive(false);
    }

    private Button CreateChoiceButton(string name, Vector2 position)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(panel.transform, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        SetRect(rect, position, new Vector2(400f, 64f));

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.32f, 0.42f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        Text label = UIManager.CreateText(buttonObject.transform, "Label", "", 18, TextAnchor.MiddleCenter);
        label.color = Color.white;
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private static void ConfigureButton(Button button, string label, UnityEngine.Events.UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = label;
        }
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static bool WasChoicePressed(int choice)
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return false;
        }

        return choice switch
        {
            1 => keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame,
            2 => keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame,
            3 => keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame,
            _ => false
        };
#else
        return choice switch
        {
            1 => Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1),
            2 => Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2),
            3 => Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3),
            _ => false
        };
#endif
    }
}
