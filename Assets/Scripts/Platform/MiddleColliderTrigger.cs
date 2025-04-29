using UnityEngine;

public class MiddleColliderTrigger : MonoBehaviour
{
    public Platform parentPlatform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && parentPlatform != null)
        {
            Debug.Log("Player has reached the midpoint of the platform.");
            ServiceLocator.Instance.GetService<EventSystem>()?
                .Publish(GameEventType.PlatformMidpointReached, parentPlatform);

        }
    }
}
