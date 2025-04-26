using UnityEngine;

public class InputService
{
    private const float SWIPE_THRESHOLD = 50f;
    private Vector2 touchStartPos;
    private bool isTouching;

    public void Update()
    {
    }

    public ICommand GetInputCommand()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            return new MoveLeftCommand();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            return new MoveRightCommand();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    break;

                case TouchPhase.Ended:
                    if (isTouching)
                    {
                        float swipeDistance = touch.position.x - touchStartPos.x;
                        isTouching = false;

                        if (Mathf.Abs(swipeDistance) > SWIPE_THRESHOLD)
                        {
                            return swipeDistance < 0 ? new MoveLeftCommand() : new MoveRightCommand();
                        }
                    }
                    break;
            }
        }

        return null;
    }
}