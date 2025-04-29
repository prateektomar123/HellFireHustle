using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private InputService _inputService;
    [SerializeField] private GameConfig gameConfig;
    public GameConfig GameConfig => gameConfig;
    public bool IsInitialized { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (gameConfig == null)
        {
            Debug.LogError("GameConfig not assigned to GameManager!", this);
            return;
        }

        // Clear any existing services to avoid duplicates after scene reload
        CleanupServices();

        // Register core services
        InitializeServices();

        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
    }

    private void CleanupServices()
    {
        if (ServiceLocator.Instance != null)
        {
            // Check and remove existing services to prevent duplicates
            ServiceLocator.Instance.RemoveService<GameConfig>();
            ServiceLocator.Instance.RemoveService<EventSystem>();
            ServiceLocator.Instance.RemoveService<InputService>();
            ServiceLocator.Instance.RemoveService<GameManager>();
        }
    }

    private void InitializeServices()
    {
        ServiceLocator.Instance.RegisterService(this);

        // Create and register essential services
        _inputService = new InputService();
        ServiceLocator.Instance.RegisterService(_inputService);

        // Create a new EventSystem (important for clean restart)
        EventSystem eventSystem = new EventSystem();
        ServiceLocator.Instance.RegisterService(eventSystem);

        // Register game configuration
        ServiceLocator.Instance.RegisterService(gameConfig);

        Debug.Log("Core services registered");
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.RemoveService<GameConfig>();
            ServiceLocator.Instance.RemoveService<EventSystem>();
            ServiceLocator.Instance.RemoveService<InputService>();
            ServiceLocator.Instance.RemoveService<GameManager>();
        }
    }
}