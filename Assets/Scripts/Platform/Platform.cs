using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length { get; private set; }
    public float LanePositionX { get; private set; }

    public Collider middleCollider;

    private void OnEnable()
    {
        // Ensure the middle collider's trigger component is properly linked when platform is enabled
        if (middleCollider != null)
        {
            var triggerScript = middleCollider.GetComponent<MiddleColliderTrigger>();
            if (triggerScript != null)
            {
                triggerScript.parentPlatform = this;
            }
        }
    }

    public void Initialize(float length, float lanePositionX)
    {
        Length = length;
        LanePositionX = lanePositionX;
        transform.localScale = new Vector3(GameManager.Instance.GameConfig.platformWidth, GameManager.Instance.GameConfig.platformHeight, length);
        transform.position = new Vector3(lanePositionX, 0, transform.position.z);

        // Double-check that middle collider reference is properly set
        if (middleCollider != null)
        {
            var triggerScript = middleCollider.GetComponent<MiddleColliderTrigger>();
            if (triggerScript != null)
            {
                triggerScript.parentPlatform = this;
            }
            else
            {
                Debug.LogError("MiddleColliderTrigger component not found on middleCollider", this);
            }
        }
        else
        {
            Debug.LogError("Middle collider reference not set on Platform", this);
        }
    }

    public float GetHalfwayPointZ()
    {
        return transform.position.z;
    }
}