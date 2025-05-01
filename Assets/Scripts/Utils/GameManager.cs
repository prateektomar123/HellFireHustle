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
        ServiceLocator.Instance.RegisterService(this);
        ServiceLocator.Instance.RegisterService(new InputService());
        ServiceLocator.Instance.RegisterService(new EventSystem());
        ServiceLocator.Instance.RegisterService(gameConfig);
    }
}