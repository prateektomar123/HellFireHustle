using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private EventSystem eventSystem;
    private InputService inputService;
    private GameConfig gameConfig;
    private void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        inputService = ServiceLocator.Instance.GetService<InputService>();
        gameConfig = ServiceLocator.Instance.GetService<GameConfig>();
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * gameConfig.playerForwardSpeed * Time.deltaTime);
        ICommand command = inputService.GetInputCommand();
        if (command != null)
        {
            command.Execute(model);
            view.MoveToLane(model.CurrentLanePosition, gameConfig.laneSwitchDuration);
            eventSystem.Publish(GameEventType.PlayerMoved, model.CurrentLaneState);
        }
    }
}