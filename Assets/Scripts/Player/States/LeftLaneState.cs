public class LeftLaneState : LaneState
{
    public LeftLaneState(PlayerModel model) : base(model) { }

    public override bool CanMoveLeft() => false;
    public override bool CanMoveRight() => true;

    public override void MoveLeft()
    {
       
    }

    public override void MoveRight()
    {
        model.SetLaneState(new MiddleLaneState(model));
    }
}