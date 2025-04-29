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
        CleanupServices();
        InitializeServices();
        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
    }
    private void CleanupServices()
    {
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.RemoveService<GameConfig>();
            ServiceLocator.Instance.RemoveService<EventSystem>();
            ServiceLocator.Instance.RemoveService<InputService>();
            ServiceLocator.Instance.RemoveService<GameManager>();
        }
    }
    private void InitializeServices()
    {
        ServiceLocator.Instance.RegisterService(this);
        _inputService = new InputService();
        ServiceLocator.Instance.RegisterService(_inputService);
        EventSystem eventSystem = new EventSystem();
        ServiceLocator.Instance.RegisterService(eventSystem);
        ServiceLocator.Instance.RegisterService(gameConfig);
        Debug.Log("Core services registered");
    }
    private void OnDestroy()
    {
        CleanupServices();
    }
}