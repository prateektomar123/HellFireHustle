using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameConfig gameConfig;
    public GameConfig GameConfig => gameConfig;
    public bool IsInitialized { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeServices();
        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
    }

    private void InitializeServices()
    {
        var serviceLocator = ServiceLocator.Instance;
        serviceLocator.RegisterService(this);
        serviceLocator.RegisterService(new InputService());
        serviceLocator.RegisterService(new EventSystem());
        serviceLocator.RegisterService(gameConfig);
    }
}