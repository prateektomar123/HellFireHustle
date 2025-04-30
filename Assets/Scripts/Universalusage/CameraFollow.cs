using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private GameConfig config;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow: Player transform not assigned.");
            enabled = false;
        }
        config = ServiceLocator.Instance.GetService<GameConfig>();
    }
    private void LateUpdate()
    {
        transform.position = new Vector3(
            0,
            config.cameraYOffset,
            player.position.z + config.cameraZOffset
        );
    }
}