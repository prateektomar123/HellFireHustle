public class PlayerModel
{
    public enum Lane { Left, Middle, Right }
    public Lane CurrentLane { get; private set; }
    public bool CanMoveLeft { get; private set; }
    public bool CanMoveRight { get; private set; }

    public PlayerModel()
    {
        CurrentLane = Lane.Middle;
        UpdateMovementPermissions();
    }

    public void SetLane(Lane newLane)
    {
        CurrentLane = newLane;
        UpdateMovementPermissions();
    }

    private void UpdateMovementPermissions()
    {
        switch (CurrentLane)
        {
            case Lane.Middle:
                CanMoveLeft = true;
                CanMoveRight = true;
                break;
            case Lane.Left:
                CanMoveLeft = false;
                CanMoveRight = true;
                break;
            case Lane.Right:
                CanMoveLeft = true;
                CanMoveRight = false;
                break;
        }
    }
}