using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    public Vector3 offset;

    
    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z + offset.z);
    }
}