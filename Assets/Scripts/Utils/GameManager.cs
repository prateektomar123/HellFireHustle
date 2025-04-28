using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private InputService _inputService;
    [SerializeField] private GameConfig gameConfig;

    public GameConfig GameConfig => gameConfig;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Instance.RegisterService(this);
        _inputService = new InputService();
        ServiceLocator.Instance.RegisterService(_inputService);
        ServiceLocator.Instance.RegisterService(new EventSystem());
        ServiceLocator.Instance.RegisterService(gameConfig);
    }

    private void Update()
    {
        if (_inputService != null)
        {
            _inputService.Update();
        }
    }
}