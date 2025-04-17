using UnityEngine;

public class FireGround : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var gameManager = ServiceLocator.Instance.GetService<GameManager>();
            gameManager.GameOver();
        }
    }
}