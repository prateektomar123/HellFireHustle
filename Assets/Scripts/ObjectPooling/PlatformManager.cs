using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform player;

    private GenericObjectPool<Platform> _platformPool;
    private Queue<Platform> _activePlatforms = new();
    private LaneState _currentPlayerLane;
    private float _lastPlatformZ;
    private EventSystem eventSystem;
    private void Awake()
    {
        ValidateSetup();
        InitializePool();
        RegisterServices();
    }

    private void ValidateSetup()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("No platform prefab assigned.", this);
            enabled = false;
        }
        if (player == null)
        {
            Debug.LogError("Player transform not assigned.", this);
            enabled = false;
        }
    }

    private void InitializePool()
    {
        _platformPool = new GenericObjectPool<Platform>(
            platformPrefab,
            GameManager.Instance.GameConfig.initialPoolSize,
            10,
            transform
        );
    }

    private void Start()
    {
        _currentPlayerLane = new MiddleLaneState(null);
        SpawnInitialPlatform();

    }

    private void OnPlayerMoved(object data)
    {
        _currentPlayerLane = data as LaneState ?? _currentPlayerLane;
        Debug.Log($"Player lane: {_currentPlayerLane?.GetType().Name ?? "Null"}");
    }

    private void OnPlatformMidpointReached(object data)
    {
        if (_activePlatforms.Count == 0) return;
        SpawnNextPlatform();
        RecycleOldestPlatform(player.position.z);
    }

    private void SpawnInitialPlatform()
    {
        Platform platform = _platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }
        platform.Initialize(GameManager.Instance.GameConfig.platformLength, 0);
        platform.transform.position = Vector3.zero;
        _activePlatforms.Enqueue(platform);
    }

    private void SpawnNextPlatform()
    {
        Vector3 nextPosition = CalculateNextPlatformPosition();
        Platform platform = _platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }
        platform.Initialize(GameManager.Instance.GameConfig.platformLength, nextPosition.x);
        platform.transform.position = nextPosition;
        _activePlatforms.Enqueue(platform);
        _lastPlatformZ = nextPosition.z;
    }

    private Vector3 CalculateNextPlatformPosition()
    {
        float nextZ;
        float nextX = 0;
        bool isAdjacent = false;

        if (_currentPlayerLane is LeftLaneState)
        {
            // When in left lane, next platform can only be left or middle
            bool stayLeft = Random.value < 0.5f;
            nextX = stayLeft ? -GameManager.Instance.GameConfig.laneDistance : 0;
            isAdjacent = stayLeft; // Adjacent only if staying in left lane
        }
        else if (_currentPlayerLane is RightLaneState)
        {
            // When in right lane, next platform can only be right or middle
            bool stayRight = Random.value < 0.5f;
            nextX = stayRight ? GameManager.Instance.GameConfig.laneDistance : 0;
            isAdjacent = stayRight; // Adjacent only if staying in right lane
        }
        else // MiddleLaneState
        {
            // When in middle lane, next platform can be left, middle, or right
            int laneChoice = Random.Range(0, 3);
            nextX = laneChoice switch
            {
                0 => -GameManager.Instance.GameConfig.laneDistance, // Left lane
                1 => 0,                                             // Middle lane
                _ => GameManager.Instance.GameConfig.laneDistance   // Right lane
            };
            isAdjacent = (laneChoice == 1); // Adjacent only if staying in middle lane
        }

        // _lastPlatformZ is the END of the previous platform

        float platformHalfLength = GameManager.Instance.GameConfig.platformLength / 2;

        if (isAdjacent)
        {
            // For same lane, we want gap between platforms
            // Platform center = last platform end + gap + half of current platform length
            nextZ = _lastPlatformZ + GameManager.Instance.GameConfig.platformGap;
        }
        else
        {
            // For lane change, use half gap
            // Platform center = last platform end + half gap + half of current platform length
            nextZ = _lastPlatformZ + GameManager.Instance.GameConfig.platformHalfGap;
        }

        // The platform center is now correctly positioned with the appropriate gap

        Debug.Log($"New platform position calculated: Lane={nextX}, Z={nextZ}, isAdjacent={isAdjacent}");
        Debug.Log($"Gap used: {(isAdjacent ? GameManager.Instance.GameConfig.platformGap : GameManager.Instance.GameConfig.platformHalfGap)}");

        return new Vector3(nextX, 0, nextZ);
    }

    private void RecycleOldestPlatform(float playerZ)
    {
        if (_activePlatforms.Count == 0) return;
        Platform oldestPlatform = _activePlatforms.Peek();
        if (playerZ > oldestPlatform.transform.position.z + GameManager.Instance.GameConfig.platformGap)
        {
            _platformPool.ReturnObject(oldestPlatform);
            _activePlatforms.Dequeue();
        }
    }

    private void RegisterServices()
    {
        ServiceLocator.Instance.RegisterService(this);
        eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        eventSystem.Subscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        eventSystem.Subscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);

    }

    private void OnDestroy()
    {
        eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        ServiceLocator.Instance.RemoveService<PlatformManager>();
    }
}