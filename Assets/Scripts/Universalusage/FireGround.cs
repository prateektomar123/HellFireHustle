using UnityEngine;

public class FireGround : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the fire ground!");
        }
    }
}