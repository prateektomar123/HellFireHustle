public class MoveLeftCommand : ICommand
{
    public void Execute(PlayerModel model)
    {
        model.MoveLeft();
    }
}