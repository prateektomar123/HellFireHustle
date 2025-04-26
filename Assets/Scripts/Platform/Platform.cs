using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length { get; private set; }
    public float LanePositionX { get; private set; }

    public void Initialize(float length, float lanePositionX)
    {
        Length = length;
        LanePositionX = lanePositionX;
        transform.localScale = new Vector3(2f, 0.1f, length);
        transform.position = new Vector3(lanePositionX, 0, transform.position.z);
    }

    public float GetHalfwayPointZ()
    {
        return transform.position.z + (Length / 2f);
    }
}