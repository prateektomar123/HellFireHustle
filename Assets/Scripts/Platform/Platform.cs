using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length { get; private set; }
    public float LanePositionX { get; private set; }

    public Collider middleCollider;
    public void Initialize(float length, float lanePositionX)
    {
        Length = length;
        LanePositionX = lanePositionX;
        transform.localScale = new Vector3(GameManager.Instance.GameConfig.platformWidth, GameManager.Instance.GameConfig.platformHeight, length);
        transform.position = new Vector3(lanePositionX, 0, transform.position.z);

        if (middleCollider != null)
        {
            var triggerScript = middleCollider.GetComponent<MiddleColliderTrigger>();
            if (triggerScript != null)
            {
                triggerScript.parentPlatform = this;
            }
        }
    }

    public float GetHalfwayPointZ()
    {
        return transform.position.z;
    }


}