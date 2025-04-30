using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Runner Game/Game Configuration")]
public class GameConfig : ScriptableObject
{
    [Header("Player Settings")]
    public float playerForwardSpeed = 5f;
    public float laneDistance = 2f;
    public float laneSwitchDuration = 0.2f;
    [Header("Platform Settings")]
    public float platformLength = 10f;
    public float platformGap = 10f;
    public float platformHalfGap = 5f;
    public float platformWidth = 2f;
    public float platformHeight = 0.5f;
    [Header("Pooling Settings")]
    public int initialPoolSize = 5;
    [Header("Fire Ground Settings")]
    public float fireGroundYPosition = 0f;
    public float fireGroundLength = 112.5f;
    [Header("Camera Settings")]
    public float cameraYOffset = 2f;
    public float cameraZOffset = -5f;
}