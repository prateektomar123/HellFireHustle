public class MiddleLaneState : LaneState
{
    public MiddleLaneState(PlayerModel model) : base(model) { }

    public override bool CanMoveLeft() => true;
    public override bool CanMoveRight() => true;

    public override void MoveLeft()
    {
        model.SetLaneState(new LeftLaneState(model));
    }

    public override void MoveRight()
    {
        model.SetLaneState(new RightLaneState(model));
    }
}