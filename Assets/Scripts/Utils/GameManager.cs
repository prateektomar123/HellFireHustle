using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Register services
        ServiceLocator.Instance.RegisterService(this);
        ServiceLocator.Instance.RegisterService(new InputService());
        ServiceLocator.Instance.RegisterService(new EventSystem());
    }

    private void Update()
    {
        var inputService = ServiceLocator.Instance.GetService<InputService>();
        inputService?.Update();
    }

    public void StartGame()
    {
        Debug.Log("Game Started");
        // Initialize game state
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        // Handle game over logic
    }
}