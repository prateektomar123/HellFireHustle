using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length { get; private set; }
    public float LanePositionX { get; private set; }
    [SerializeField] public Collider middleCollider;
    private GameConfig config;

    public void Initialize(float length, float lanePositionX)
    {
        config = ServiceLocator.Instance.GetService<GameConfig>();
        Length = length;
        LanePositionX = lanePositionX;
        transform.localScale = new Vector3(config.platformWidth, config.platformHeight, length);
        transform.position = new Vector3(lanePositionX, 0, transform.position.z);
    }

    public float GetHalfwayPointZ()
    {
        return transform.position.z;
    }
}