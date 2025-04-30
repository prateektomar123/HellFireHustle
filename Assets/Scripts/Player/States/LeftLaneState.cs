public class LeftLaneState : LaneState
{
    public LeftLaneState(PlayerModel model) : base(model) { }
    public override LaneState MoveLeft()
    {
        return this; // Cannot move left from Left
    }
    public override LaneState MoveRight()
    {
        return new MiddleLaneState(model);
    }
    public override float GetLanePosition()
    {
        return -ServiceLocator.Instance.GetService<GameConfig>().laneDistance;
    }
}