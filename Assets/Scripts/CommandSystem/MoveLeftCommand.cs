using UnityEngine;

public class MoveLeftCommand : ICommand
{
    private PlayerController playerController;

    public MoveLeftCommand(PlayerController controller)
    {
        playerController = controller;
    }

    public void Execute()
    {
        playerController.MoveLeft();
        Debug.Log("Move Left Command Executed");
    }
}