using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    public void PlayMoveAnimation(bool isMovingLeft)
    {
        
        Debug.Log($"Playing move animation: {(isMovingLeft ? "Left" : "Right")}");
    }
}