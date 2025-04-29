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
        ServiceLocator.Instance.RegisterService(this);
        InitializeEventSystem();
        SetGameState(GameState.MainMenu);
        Time.timeScale = 0f;
    }

    private void InitializeEventSystem()
    {
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        eventSystem.Subscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        eventSystem.Publish(GameEventType.GameStarted);
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
        eventSystem.Publish(GameEventType.GameOver);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        CleanupBeforeRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CleanupBeforeRestart()
    {
        eventSystem.Unsubscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        Time.timeScale = 1f;
        SetGameState(GameState.MainMenu);
        InitializeEventSystem();
    }

    private void OnPlayerDied(object data)
    {
        GameOver();
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
        if (mainMenuUI != null) mainMenuUI.SetActive(newState == GameState.MainMenu);
        if (gameUI != null) gameUI.SetActive(newState == GameState.Playing);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(newState == GameState.Paused);
        if (gameOverUI != null) gameOverUI.SetActive(newState == GameState.GameOver);
        Debug.Log($"Game state changed to: {newState}");
    }

    private void OnDestroy()
    {
        eventSystem.Unsubscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        ServiceLocator.Instance?.RemoveService<GameStateManager>();
    }
}