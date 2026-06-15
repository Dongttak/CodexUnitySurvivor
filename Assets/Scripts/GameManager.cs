using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LevelSystem levelSystem;

    public bool IsGameOver { get; private set; }
    public float SurvivalTime { get; private set; }
    public bool IsGameplayActive => !IsGameOver && Time.timeScale > 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        ResolveReferences();

        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDied;
        }

        if (uiManager != null)
        {
            uiManager.SetSurvivalTime(SurvivalTime);
            uiManager.HideGameOver();
        }
    }

    private void Update()
    {
        if (IsGameOver && WasRestartPressed())
        {
            RestartGame();
            return;
        }

        if (IsGameplayActive)
        {
            SurvivalTime += Time.deltaTime;
            if (uiManager != null)
            {
                uiManager.SetSurvivalTime(SurvivalTime);
            }
        }
    }

    private static bool WasRestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    public void PauseForUpgrade()
    {
        if (!IsGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    public void ResumeGameplay()
    {
        if (!IsGameOver)
        {
            Time.timeScale = 1f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandlePlayerDied()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }

        if (uiManager != null)
        {
            uiManager.ShowGameOver(SurvivalTime);
        }
    }

    private void ResolveReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }

        if (levelSystem == null)
        {
            levelSystem = FindFirstObjectByType<LevelSystem>();
        }
    }
}
