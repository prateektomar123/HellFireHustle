using UnityEngine;

public class InputService
{
    private const float SWIPE_THRESHOLD = 50f;
    private const float SWIPE_TIME_THRESHOLD = 0.5f;
    private Vector2 touchStartPos;
    private float touchStartTime;
    private bool isTouching;
    private int trackingTouchId = -1;

    public ICommand GetInputCommand()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return new MoveLeftCommand();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            return new MoveRightCommand();
        }
        // Check for touch input on mobile devices or tablets 
        if (Input.touchCount > 0)
        {
            Touch? currentTouch = null;
            if (trackingTouchId >= 0)
            {
                foreach (Touch t in Input.touches)
                {
                    if (t.fingerId == trackingTouchId)
                    {
                        currentTouch = t;
                        break;
                    }
                }
            }
            if (currentTouch == null)
            {
                currentTouch = Input.GetTouch(0);
            }
            Touch touch = currentTouch.Value;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                    trackingTouchId = touch.fingerId;
                    isTouching = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isTouching && touch.fingerId == trackingTouchId)
                    {
                        float swipeDistance = touch.position.x - touchStartPos.x;
                        float swipeTime = Time.time - touchStartTime;
                        isTouching = false;
                        trackingTouchId = -1;
                        if (Mathf.Abs(swipeDistance) > SWIPE_THRESHOLD && swipeTime < SWIPE_TIME_THRESHOLD)
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