using UnityEngine;

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
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        eventSystem.Subscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        
        // Register this service
        ServiceLocator.Instance.RegisterService(this);
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        eventSystem.Publish(GameEventType.GameStarted);
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
    }

    private void OnPlayerDied(object data)
    {
        GameOver();
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
        
        // Update UI based on state
        mainMenuUI?.SetActive(newState == GameState.MainMenu);
        gameUI?.SetActive(newState == GameState.Playing);
        pauseMenuUI?.SetActive(newState == GameState.Paused);
        gameOverUI?.SetActive(newState == GameState.GameOver);
    }

    private void OnDestroy()
    {
        eventSystem.Unsubscribe(GameEventType.PlayerHitFireGround, OnPlayerDied);
        ServiceLocator.Instance.RemoveService<GameStateManager>();
    }
}