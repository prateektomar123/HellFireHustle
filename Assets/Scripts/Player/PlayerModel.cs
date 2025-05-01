using UnityEngine;
public class PlayerModel
{
    public LaneState CurrentLaneState { get; private set; }
    public float CurrentLanePosition { get; private set; }

    public PlayerModel()
    {
        CurrentLaneState = new MiddleLaneState(this);
        CurrentLanePosition = 0f;
    }

    public void MoveLeft()
    {
        CurrentLaneState = CurrentLaneState.MoveLeft();
        CurrentLanePosition = CurrentLaneState.GetLanePosition();
    }

    public void MoveRight()
    {
        CurrentLaneState = CurrentLaneState.MoveRight();
        CurrentLanePosition = CurrentLaneState.GetLanePosition();
    }
}