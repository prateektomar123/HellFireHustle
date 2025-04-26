public abstract class LaneState
{
    protected PlayerModel model;

    protected LaneState(PlayerModel model)
    {
        this.model = model;
    }

    public abstract LaneState MoveLeft();
    public abstract LaneState MoveRight();
    public abstract float GetLanePosition();
}