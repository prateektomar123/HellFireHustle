// Update PlayerController to use constructor injection
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private EventSystem eventSystem;
    private InputService inputService;

    

    private void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        inputService = ServiceLocator.Instance.GetService<InputService>();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * GameManager.Instance.GameConfig.playerForwardSpeed * Time.deltaTime);

        ICommand command = inputService?.GetInputCommand();
        if (command != null)
        {
            command.Execute(model);
            view.MoveToLane(model.CurrentLanePosition, GameManager.Instance.GameConfig.laneSwitchDuration);
            eventSystem.Publish(GameEventType.PlayerMoved, model.CurrentLaneState);
        }
    }
}