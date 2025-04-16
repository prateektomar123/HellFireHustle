using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel model;
    private PlayerView view;
    private InputService inputService;
    private readonly float laneDistance = 2f; 
    private readonly float forwardSpeed = 5f; 

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

    private void Update()
    {
        
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    public void MoveLeft()
    {
        if (model.CanMoveLeft())
        {
            model.CurrentLaneState.MoveLeft();
            UpdatePlayerPosition();
            view.PlayMoveAnimation(true);
            var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.Publish("PlayerMoved", model.CurrentLaneState);
            }
            else
            {
                Debug.LogError("EventSystem not found in MoveLeft.");
            }
        }
    }

    public void MoveRight()
    {
        if (model.CanMoveRight())
        {
            model.CurrentLaneState.MoveRight();
            UpdatePlayerPosition();
            view.PlayMoveAnimation(false);
            var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.Publish("PlayerMoved", model.CurrentLaneState);
            }
            else
            {
                Debug.LogError("EventSystem not found in MoveRight.");
            }
        }
    }

    private void UpdatePlayerPosition()
    {
        float xPos = model.CurrentLaneState switch
        {
            LeftLaneState => -laneDistance,
            MiddleLaneState => 0,
            RightLaneState => laneDistance,
            _ => 0
        };
        view.UpdatePosition(new Vector3(xPos, transform.position.y, transform.position.z));
    }
}