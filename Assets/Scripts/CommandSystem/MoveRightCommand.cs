public class MoveRightCommand : ICommand
{
    public void Execute(PlayerModel model)
    {
        model.MoveRight();
    }
}