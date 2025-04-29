using UnityEngine;

public class MiddleColliderTrigger : MonoBehaviour
{
    public Platform parentPlatform;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && parentPlatform != null && !hasTriggered)
        {
            Debug.Log("Player has reached the midpoint of the platform.");

            // Get the EventSystem from ServiceLocator
            var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem not found in ServiceLocator");
                return;
            }

            eventSystem.Publish(GameEventType.PlatformMidpointReached, parentPlatform);
            hasTriggered = true;
        }
    }

    // Reset the trigger state when the platform is reused from pool
    private void OnEnable()
    {
        hasTriggered = false;
    }
}