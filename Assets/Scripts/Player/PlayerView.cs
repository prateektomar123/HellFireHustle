using UnityEngine;
public class PlayerView : MonoBehaviour
{
    private Vector3 targetPosition;
    private float transitionSpeed;
    private bool isMoving;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    public void MoveToLane(float laneX, float duration)
    {
        if (duration <= 0)
        {
            Debug.LogError("MoveToLane: Duration must be positive.");
            return;
        }

        targetPosition = new Vector3(laneX, transform.position.y, transform.position.z);
        transitionSpeed = Vector3.Distance(transform.position, targetPosition) / duration;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            transitionSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }
}