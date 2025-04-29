using UnityEngine;

public class FireGround : MonoBehaviour
{
    public float Length { get; private set; }

    public void Initialize(float length)
    {
        Length = length;

        //in future scenario for harder levels, we can change the width of the fire ground
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