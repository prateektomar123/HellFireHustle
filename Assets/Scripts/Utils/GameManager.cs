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

        ServiceLocator.Instance.RegisterService(this);
        _inputService = new InputService();
        ServiceLocator.Instance.RegisterService(_inputService);
        ServiceLocator.Instance.RegisterService(new EventSystem());
        ServiceLocator.Instance.RegisterService(gameConfig);

        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
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