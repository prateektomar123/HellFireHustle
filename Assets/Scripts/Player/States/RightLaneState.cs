public class RightLaneState : LaneState
{
    public RightLaneState(PlayerModel model) : base(model) { }

    public override LaneState MoveLeft()
    {
        return new MiddleLaneState(model);
    }
    public override LaneState MoveRight()
    {
        return this; // Cannot move right from Right
    }
    public override float GetLanePosition()
    {
        return ServiceLocator.Instance.GetService<GameConfig>().laneDistance;
    }
}