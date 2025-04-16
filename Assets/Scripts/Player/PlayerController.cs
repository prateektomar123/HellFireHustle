using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private InputService inputService;
    private readonly float laneDistance = 2f; // Distance between lanes

    private void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
    }

    private void Start()
    {
        inputService = ServiceLocator.Instance.GetService<InputService>();
        if (inputService != null)
        {
            inputService.Initialize(this);
        }
        else
        {
            Debug.LogError("InputService not found in Start. Ensure GameManager is initialized first.");
        }
    }

    public void MoveLeft()
    {
        if (model.CanMoveLeft)
        {
            PlayerModel.Lane newLane = model.CurrentLane == PlayerModel.Lane.Middle ? PlayerModel.Lane.Left : PlayerModel.Lane.Middle;
            model.SetLane(newLane);
            UpdatePlayerPosition();
            view.PlayMoveAnimation(true);
            var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.Publish("PlayerMoved", newLane);
            }
            else
            {
                Debug.LogError("EventSystem not found in MoveLeft.");
            }
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
            var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.Publish("PlayerMoved", newLane);
            }
            else
            {
                Debug.LogError("EventSystem not found in MoveRight.");
            }
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