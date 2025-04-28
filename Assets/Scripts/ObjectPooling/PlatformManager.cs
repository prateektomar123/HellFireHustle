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
        _lastPlatformZ = _activePlatforms.Peek().GetHalfwayPointZ();
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
            bool stayLeft = Random.value < 0.5f;
            nextX = stayLeft ? -GameManager.Instance.GameConfig.laneDistance : 0;
            isAdjacent = stayLeft;
            nextZ = _lastPlatformZ + (stayLeft ? GameManager.Instance.GameConfig.platformLength : GameManager.Instance.GameConfig.platformLength / 2);
        }
        else if (_currentPlayerLane is RightLaneState)
        {
            bool stayRight = Random.value < 0.5f;
            nextX = stayRight ? GameManager.Instance.GameConfig.laneDistance : 0;
            isAdjacent = stayRight;
            nextZ = _lastPlatformZ + (stayRight ? GameManager.Instance.GameConfig.platformLength : GameManager.Instance.GameConfig.platformLength / 2);
        }
        else // MiddleLaneState or null
        {
            int laneChoice = Random.Range(0, 3);
            nextX = laneChoice switch
            {
                0 => -GameManager.Instance.GameConfig.laneDistance, // Middle-to-Left
                1 => 0,                           // Middle-to-Middle
                _ => GameManager.Instance.GameConfig.laneDistance  // Middle-to-Right
            };
            isAdjacent = (laneChoice == 1);
            nextZ = _lastPlatformZ + (laneChoice == 1 ? GameManager.Instance.GameConfig.platformLength : GameManager.Instance.GameConfig.platformLength / 2);
        }

        Debug.Log($"Spawning platform at X={nextX}, Z={nextZ}, Lane={_currentPlayerLane?.GetType().Name}, StartZ={nextZ - GameManager.Instance.GameConfig.platformLength/2}, Adjacent={isAdjacent}");
        return new Vector3(nextX, 0, nextZ);
    }

    private void RecycleOldestPlatform(float playerZ)
    {
        if (_activePlatforms.Count == 0) return;
        Platform oldestPlatform = _activePlatforms.Peek();
        if (playerZ > oldestPlatform.transform.position.z + GameManager.Instance.GameConfig.platformLength)
        {
            _platformPool.ReturnObject(oldestPlatform);
            _activePlatforms.Dequeue();
        }
    }

    private void RegisterServices()
    {
        ServiceLocator.Instance.RegisterService(this);
        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Subscribe(GameEventType.PlayerMoved, OnPlayerMoved);
            eventSystem.Subscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        }
        else
        {
            Debug.LogError("EventSystem not found.", this);
        }
    }

    private void OnDestroy()
    {
        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
            eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        }
        ServiceLocator.Instance.RemoveService<PlatformManager>();
    }
}