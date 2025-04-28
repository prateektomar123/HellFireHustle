using UnityEngine;

public class FireGround : MonoBehaviour
{
    public float Length { get; private set; }

    public void Initialize(float length)
    {
        Length = length;
        //transform.localScale = new Vector3(GameConstants.PLATFORM_WIDTH, 0.1f, length);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ServiceLocator.Instance.GetService<EventSystem>()?
                .Publish(GameEventType.PlayerHitFireGround);
        }
    }
}