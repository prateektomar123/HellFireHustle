using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow: Player transform not assigned.");
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(
            0,
            GameConstants.CAMERA_Y_OFFSET,
            player.position.z + GameConstants.CAMERA_Z_OFFSET
        );
    }
}