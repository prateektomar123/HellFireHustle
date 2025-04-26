public class MiddleLaneState : LaneState
{
    public MiddleLaneState(PlayerModel model) : base(model) { }

    public override LaneState MoveLeft()
    {
        return new LeftLaneState(model);
    }

    public override LaneState MoveRight()
    {
        return new RightLaneState(model);
    }

    public override float GetLanePosition()
    {
        return 0f;
    }
}