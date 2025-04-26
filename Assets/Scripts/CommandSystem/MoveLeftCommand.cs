/// <summary>
/// Command to move the player to the left lane.
/// </summary>
public class MoveLeftCommand : ICommand
{
    public void Execute(PlayerModel model)
    {
        model.MoveLeft();
    }
}