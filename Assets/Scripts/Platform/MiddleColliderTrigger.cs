using UnityEngine;

public class MiddleColliderTrigger : MonoBehaviour
{
    public Platform parentPlatform;
    private EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //MiddlePoint is the point which triggers the the next platform spawning thats why is published here
        if (other.CompareTag("Player") && parentPlatform != null)
        {
            eventSystem.Publish(GameEventType.PlatformMidpointReached, parentPlatform);
        }
    }
}