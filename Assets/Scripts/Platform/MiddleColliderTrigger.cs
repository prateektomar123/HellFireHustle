using UnityEngine;

public class MiddleColliderTrigger : MonoBehaviour
{
    public Platform parentPlatform;
    private EventSystem _eventSystem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && parentPlatform != null)
        {
            _eventSystem = ServiceLocator.Instance?.GetService<EventSystem>();
            _eventSystem.Publish(GameEventType.PlatformMidpointReached, parentPlatform);
        }
    }
}