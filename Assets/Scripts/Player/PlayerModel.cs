using UnityEngine;

public class PlayerModel
{
    public LaneState CurrentLaneState { get; private set; }

    public PlayerModel()
    {
        CurrentLaneState = new MiddleLaneState(this);
    }

    public void SetLaneState(LaneState newState)
    {
        CurrentLaneState = newState;
        Debug.Log($"Lane changed to: {newState.GetType().Name}");
    }

    public bool CanMoveLeft()
    {
        return CurrentLaneState.CanMoveLeft();
    }

    public bool CanMoveRight()
    {
        return CurrentLaneState.CanMoveRight();
    }
}