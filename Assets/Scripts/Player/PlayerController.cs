using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private EventSystem eventSystem;

    private void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * GameConstants.PLAYER_FORWARD_SPEED * Time.deltaTime);

        var inputService = ServiceLocator.Instance.GetService<InputService>();
        ICommand command = inputService?.GetInputCommand();
        if (command != null)
        {
            command.Execute(model);
            view.MoveToLane(model.CurrentLanePosition, GameConstants.LANE_SWITCH_DURATION);
            eventSystem.Publish("PlayerMoved", model.CurrentLaneState);
        }
    }
}