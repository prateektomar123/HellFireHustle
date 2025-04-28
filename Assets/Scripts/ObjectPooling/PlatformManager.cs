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
            GameConstants.INITIAL_POOL_SIZE,
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
        platform.Initialize(GameConstants.PLATFORM_LENGTH, 0);
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
        platform.Initialize(GameConstants.PLATFORM_LENGTH, nextPosition.x);
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
            nextX = stayLeft ? -GameConstants.LANE_DISTANCE : 0;
            isAdjacent = stayLeft;
            nextZ = _lastPlatformZ + (stayLeft ? GameConstants.PLATFORM_LENGTH : GameConstants.PLATFORM_LENGTH / 2);
        }
        else if (_currentPlayerLane is RightLaneState)
        {
            bool stayRight = Random.value < 0.5f;
            nextX = stayRight ? GameConstants.LANE_DISTANCE : 0;
            isAdjacent = stayRight;
            nextZ = _lastPlatformZ + (stayRight ? GameConstants.PLATFORM_LENGTH : GameConstants.PLATFORM_LENGTH / 2);
        }
        else // MiddleLaneState or null
        {
            int laneChoice = Random.Range(0, 3);
            nextX = laneChoice switch
            {
                0 => -GameConstants.LANE_DISTANCE, // Middle-to-Left
                1 => 0,                           // Middle-to-Middle
                _ => GameConstants.LANE_DISTANCE  // Middle-to-Right
            };
            isAdjacent = (laneChoice == 1);
            nextZ = _lastPlatformZ + (laneChoice == 1 ? GameConstants.PLATFORM_LENGTH : GameConstants.PLATFORM_LENGTH / 2);
        }

        Debug.Log($"Spawning platform at X={nextX}, Z={nextZ}, Lane={_currentPlayerLane?.GetType().Name}, StartZ={nextZ - GameConstants.PLATFORM_LENGTH/2}, Adjacent={isAdjacent}");
        return new Vector3(nextX, 0, nextZ);
    }

    private void RecycleOldestPlatform(float playerZ)
    {
        if (_activePlatforms.Count == 0) return;
        Platform oldestPlatform = _activePlatforms.Peek();
        if (playerZ > oldestPlatform.transform.position.z + GameConstants.PLATFORM_LENGTH)
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
            eventSystem.Subscribe("PlayerMoved", OnPlayerMoved);
            eventSystem.Subscribe("PlatformMidpointReached", OnPlatformMidpointReached);
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
            eventSystem.Unsubscribe("PlayerMoved", OnPlayerMoved);
            eventSystem.Unsubscribe("PlatformMidpointReached", OnPlatformMidpointReached);
        }
        ServiceLocator.Instance.RemoveService<PlatformManager>();
    }
}