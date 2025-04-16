using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private InputService inputService;
    private readonly float laneDistance = 2f;

    private void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        inputService = ServiceLocator.Instance.GetService<InputService>();
        inputService.Initialize(this);
    }

    public void MoveLeft()
    {
        if (model.CanMoveLeft)
        {
            PlayerModel.Lane newLane = model.CurrentLane == PlayerModel.Lane.Middle ? PlayerModel.Lane.Left : PlayerModel.Lane.Middle;
            model.SetLane(newLane);
            UpdatePlayerPosition();
            view.PlayMoveAnimation(true);
            ServiceLocator.Instance.GetService<EventSystem>().Publish("PlayerMoved", newLane);
        }
    }

    public void MoveRight()
    {
        if (model.CanMoveRight)
        {
            PlayerModel.Lane newLane = model.CurrentLane == PlayerModel.Lane.Middle ? PlayerModel.Lane.Right : PlayerModel.Lane.Middle;
            model.SetLane(newLane);
            UpdatePlayerPosition();
            view.PlayMoveAnimation(false);
            ServiceLocator.Instance.GetService<EventSystem>().Publish("PlayerMoved", newLane);
        }
    }

    private void UpdatePlayerPosition()
    {
        float xPos = model.CurrentLane switch
        {
            PlayerModel.Lane.Left => -laneDistance,
            PlayerModel.Lane.Middle => 0,
            PlayerModel.Lane.Right => laneDistance,
            _ => 0
        };
        view.UpdatePosition(new Vector3(xPos, transform.position.y, transform.position.z));
    }
}