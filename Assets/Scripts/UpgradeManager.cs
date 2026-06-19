using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    private const int TitleFontSize = 52;
    private const int ShortcutFontSize = 36;
    private const int UpgradeNameFontSize = 38;
    private const int DescriptionFontSize = 28;

    private GameObject panel;
    private Button damageButton;
    private Button fireRateButton;
    private Button moveSpeedButton;
    private Text titleText;
    private LevelSystem activeLevelSystem;
    private readonly List<LevelSystem.UpgradeChoice> activeChoices = new List<LevelSystem.UpgradeChoice>();

    private void Update()
    {
        if (panel == null || !panel.activeSelf || activeLevelSystem == null)
        {
            return;
        }

        if (WasChoicePressed(1))
        {
            Choose(0);
        }
        else if (WasChoicePressed(2))
        {
            Choose(1);
        }
        else if (WasChoicePressed(3))
        {
            Choose(2);
        }
    }

    public void ShowChoices(LevelSystem levelSystem, IReadOnlyList<LevelSystem.UpgradeChoice> choices)
    {
        EnsurePanel();

        activeLevelSystem = levelSystem;
        activeChoices.Clear();
        activeChoices.AddRange(choices);
        titleText.text = "강화 선택";
        ConfigureButton(damageButton, 0, () => Choose(0));
        ConfigureButton(fireRateButton, 1, () => Choose(1));
        ConfigureButton(moveSpeedButton, 2, () => Choose(2));
        panel.SetActive(true);
    }

    public void HideChoices()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        activeLevelSystem = null;
        activeChoices.Clear();
    }

    private void Choose(int choiceIndex)
    {
        if (activeLevelSystem == null || choiceIndex < 0 || choiceIndex >= activeChoices.Count)
        {
            return;
        }

        UpgradeType upgradeType = activeChoices[choiceIndex].Type;
        LevelSystem levelSystem = activeLevelSystem;
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
        Transform existing = canvas.transform.Find("Upgrade Panel");
        panel = existing != null ? existing.gameObject : new GameObject("Upgrade Panel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform rect = EnsureRectTransform(panel);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(860f, 570f);
        rect.anchoredPosition = Vector2.zero;

        Image background = EnsureImage(panel);
        background.color = new Color(0.035f, 0.045f, 0.06f, 0.96f);

        titleText = GetOrCreateText(panel.transform, "Title", "강화 선택", TitleFontSize, TextAnchor.MiddleCenter);
        SetRect(titleText.rectTransform, new Vector2(0f, 224f), new Vector2(760f, 74f));

        damageButton = CreateChoiceButton("Damage Button", new Vector2(0f, 110f));
        fireRateButton = CreateChoiceButton("Fire Rate Button", new Vector2(0f, -36f));
        moveSpeedButton = CreateChoiceButton("Move Speed Button", new Vector2(0f, -182f));

        panel.SetActive(false);
    }

    private Button CreateChoiceButton(string name, Vector2 position)
    {
        GameObject buttonObject = GetOrCreateChild(panel.transform, name);
        buttonObject.transform.SetParent(panel.transform, false);

        RectTransform rect = EnsureRectTransform(buttonObject);
        SetRect(rect, position, new Vector2(720f, 122f));

        Image image = EnsureImage(buttonObject);
        image.color = new Color(0.13f, 0.24f, 0.31f, 0.98f);

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.13f, 0.24f, 0.31f, 0.98f);
        colors.highlightedColor = new Color(0.22f, 0.42f, 0.52f, 1f);
        colors.pressedColor = new Color(0.4f, 0.68f, 0.58f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.65f);
        button.colors = colors;

        Text shortcut = GetOrCreateText(buttonObject.transform, "Shortcut", "[1]", ShortcutFontSize, TextAnchor.MiddleCenter);
        SetChildRect(shortcut.rectTransform, new Vector2(0f, 0f), new Vector2(96f, 96f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));

        Text nameText = GetOrCreateText(buttonObject.transform, "Name", "", UpgradeNameFontSize, TextAnchor.LowerLeft);
        SetChildRect(nameText.rectTransform, new Vector2(112f, 18f), new Vector2(560f, 46f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));

        Text description = GetOrCreateText(buttonObject.transform, "Description", "", DescriptionFontSize, TextAnchor.UpperLeft);
        description.color = new Color(0.84f, 0.92f, 0.96f);
        SetChildRect(description.rectTransform, new Vector2(112f, -24f), new Vector2(560f, 44f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));

        return button;
    }

    private void ConfigureButton(Button button, int choiceIndex, UnityEngine.Events.UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);

        LevelSystem.UpgradeChoice choice = activeChoices[choiceIndex];
        Text shortcut = FindChildText(button.transform, "Shortcut");
        Text nameText = FindChildText(button.transform, "Name");
        Text description = FindChildText(button.transform, "Description");

        if (shortcut != null)
        {
            shortcut.text = $"[{choiceIndex + 1}]";
        }

        if (nameText != null)
        {
            nameText.text = choice.Name;
        }

        if (description != null)
        {
            description.text = choice.Description;
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

    private static void SetChildRect(RectTransform rect, Vector2 position, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
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

    private static Text GetOrCreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment)
    {
        GameObject target = GetOrCreateChild(parent, name);
        Text text = target.GetComponent<Text>();
        if (text == null)
        {
            EnsureRectTransform(target);
            text = target.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Text FindChildText(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        return child == null ? null : child.GetComponent<Text>();
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
