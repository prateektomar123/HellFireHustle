public class RightLaneState : LaneState
{
    public RightLaneState(PlayerModel model) : base(model) { }

    public override bool CanMoveLeft() => true;
    public override bool CanMoveRight() => false;

    public override void MoveLeft()
    {
        model.SetLaneState(new MiddleLaneState(model));
    }

    public override void MoveRight()
    {
       
    }
}