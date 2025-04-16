using UnityEngine;

public class InputService
{
    private ICommand moveLeftCommand;
    private ICommand moveRightCommand;
    private Vector2 touchStartPos;
    private bool isSwiping;

    public InputService()
    {
       
    }

    public void Initialize(PlayerController playerController)
    {
        moveLeftCommand = new MoveLeftCommand(playerController);
        moveRightCommand = new MoveRightCommand(playerController);
    }

    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveLeftCommand?.Execute();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveRightCommand?.Execute();
        }

       
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        Vector2 touchEndPos = touch.position;
                        DetectSwipe(touchEndPos);
                        isSwiping = false;
                    }
                    break;
            }
        }
    }

    private void DetectSwipe(Vector2 touchEndPos)
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;
        if (Mathf.Abs(swipeDelta.x) > 50)
        {
            if (swipeDelta.x > 0)
            {
                moveRightCommand?.Execute();
            }
            else
            {
                moveLeftCommand?.Execute();
            }
        }
    }
}