using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private InputService _inputService;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Instance.RegisterService(this);
        _inputService = new InputService();
        ServiceLocator.Instance.RegisterService(_inputService);
        ServiceLocator.Instance.RegisterService(new EventSystem());
        // ServiceLocator.Instance.RegisterService(GetComponent<PlatformManager>());
        // ServiceLocator.Instance.RegisterService(GetComponent<FireGroundManager>());
    }

    private void Update()
    {
        if (_inputService != null)
        {
            _inputService.Update();
        }
    }
}