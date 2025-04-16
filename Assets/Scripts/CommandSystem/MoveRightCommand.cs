using UnityEngine;

public class MoveRightCommand : ICommand
{
    private PlayerController playerController;

    public MoveRightCommand(PlayerController controller)
    {
        playerController = controller;
    }

    public void Execute()
    {
        playerController.MoveRight();
        Debug.Log("Move Right Command Executed");
    }
}