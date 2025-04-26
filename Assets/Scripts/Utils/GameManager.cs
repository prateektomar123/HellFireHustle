using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private InputService inputService;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Instance.RegisterService(this);
        inputService = new InputService();
        ServiceLocator.Instance.RegisterService(inputService);
        ServiceLocator.Instance.RegisterService(new EventSystem());
        ServiceLocator.Instance.RegisterService(GetComponent<EnvironmentManager>());
    }

    private void Update()
    {
        inputService?.Update();
    }
}