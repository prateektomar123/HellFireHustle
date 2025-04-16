public abstract class LaneState
{
    protected PlayerModel model;

    protected LaneState(PlayerModel model)
    {
        this.model = model;
    }

    public abstract bool CanMoveLeft();
    public abstract bool CanMoveRight();
    public abstract void MoveLeft();
    public abstract void MoveRight();
}