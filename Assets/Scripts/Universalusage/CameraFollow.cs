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
            GameManager.Instance.GameConfig.cameraYOffset,
            player.position.z + GameManager.Instance.GameConfig.cameraZOffset
        );
    }
}