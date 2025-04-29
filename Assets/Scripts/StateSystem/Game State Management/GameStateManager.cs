using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameStateManager : MonoBehaviour
{
    private GameState currentState = GameState.MainMenu;
    private EventSystem eventSystem;

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverUI;

    public GameState CurrentState => currentState;

    private void Awake()
    {
        // Register this service early in the lifecycle
        ServiceLocator.Instance.RegisterService(this);

        // Fetch and subscribe to the EventSystem
        InitializeEventSystem();

        // Default to main menu state with time stopped
        SetGameState(GameState.MainMenu);
        Time.timeScale = 0f;
    }

    private void InitializeEventSystem()
    {
        // Get event system and subscribe to events
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Subscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        }
        else
        {
            Debug.LogError("EventSystem not found in ServiceLocator");
        }
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);

        // Publish GameStarted event
        if (eventSystem != null)
        {
            eventSystem.Publish(GameEventType.GameStarted);
        }
        else
        {
            Debug.LogError("EventSystem not found when starting game");
        }

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        SetGameState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);

        if (eventSystem != null)
        {
            eventSystem.Publish(GameEventType.GameOver);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Clean up before reloading scene
        CleanupBeforeRestart();

        // Load the current scene again
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CleanupBeforeRestart()
    {
        // Unsubscribe from events to prevent duplicate subscriptions after reload
        if (eventSystem != null)
        {
            eventSystem.Unsubscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        }

        // Reset time scale to ensure proper restart
        Time.timeScale = 1f;

        // Set game state to ensure UI is in correct state during transition
        SetGameState(GameState.MainMenu);

        // Reinitialize EventSystem after cleanup
        InitializeEventSystem();
    }

    private void OnPlayerDied(object data)
    {
        GameOver();
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;

        // Update UI visibility based on game state
        if (mainMenuUI != null) mainMenuUI.SetActive(newState == GameState.MainMenu);
        if (gameUI != null) gameUI.SetActive(newState == GameState.Playing);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(newState == GameState.Paused);
        if (gameOverUI != null) gameOverUI.SetActive(newState == GameState.GameOver);

        Debug.Log($"Game state changed to: {newState}");
    }

    private void OnDestroy()
    {
        if (eventSystem != null)
        {
            eventSystem.Unsubscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        }

        ServiceLocator.Instance?.RemoveService<GameStateManager>();
    }
}