using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    public GameConfig GameConfig => gameConfig;
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        InitializeServices();
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("GameplayScene");
        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
    }

    private void InitializeServices()
    {
        ServiceLocator.Instance.RegisterService(this);
        ServiceLocator.Instance.RegisterService(new InputService());
        ServiceLocator.Instance.RegisterService(new EventSystem());
        ServiceLocator.Instance.RegisterService(gameConfig);
        Debug.Log("Core services registered");
    }

}