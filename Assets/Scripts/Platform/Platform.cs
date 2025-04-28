using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length { get; private set; }
    public float LanePositionX { get; private set; }

    private BoxCollider _midpointCollider;

    private void Awake()
    {
        _midpointCollider = gameObject.AddComponent<BoxCollider>();
        _midpointCollider.isTrigger = true;
        _midpointCollider.size = new Vector3(GameManager.Instance.GameConfig.platformWidth, 0.1f, 0.1f);
    }

    public void Initialize(float length, float lanePositionX)
    {
        Length = length;
        LanePositionX = lanePositionX;
        transform.localScale = new Vector3(GameManager.Instance.GameConfig.platformWidth, GameManager.Instance.GameConfig.platformHeight, length);
        transform.position = new Vector3(lanePositionX, 0, transform.position.z);

        float midpointZ = (length * 0.5f) - (length / 2f);
        _midpointCollider.center = new Vector3(0, 0, midpointZ);
    }

    public float GetHalfwayPointZ()
    {
        return transform.position.z + (Length / 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ServiceLocator.Instance.GetService<EventSystem>()?
                .Publish(GameEventType.PlatformMidpointReached, this);

                
        }
    }
}