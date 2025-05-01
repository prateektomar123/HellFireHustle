// this script manages the spawning and recycling of platforms in a game.
// it uses a generic object pool to manage platform instances and handles player lane changes.
// the script also subscribes to game events to trigger platform spawning and recycling based on player movement and game state.
// it ensures that platforms are recycled when they are no longer needed, optimizing memory usage and performance.
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform player;
    private GenericObjectPool<Platform> platformPool;
    private Queue<Platform> activePlatforms = new();
    private LaneState currentPlayerLane;
    private float lastPlatformZ;
    private EventSystem eventSystem;
    private GameStateManager gameStateManager;
    private GameConfig config;
    private bool isInitialized;

    private void Awake()
    {
        activePlatforms = new Queue<Platform>();
        currentPlayerLane = new MiddleLaneState(null);
        if (platformPrefab == null)
        {
            Debug.LogError("Platform prefab not assigned.", this);
            enabled = false;
            return;
        }
        var serviceLocator = ServiceLocator.Instance;
        config = serviceLocator.GetService<GameConfig>();
        eventSystem = serviceLocator.GetService<EventSystem>();
        gameStateManager = serviceLocator.GetService<GameStateManager>();
        InitializePool();
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void InitializePool()
    {
        platformPool = new GenericObjectPool<Platform>(
            platformPrefab,
            config.initialPoolSize,
            10,
            transform
        );
        Debug.Log($"Platform pool initialized with {config.initialPoolSize} objects");
    }

    private void SubscribeToEvents()
    {
        eventSystem.Subscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        eventSystem.Subscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        eventSystem.Subscribe(GameEventType.GameStarted, OnGameStarted);
    }

    private void OnGameStarted(object data)
    {
        isInitialized = false;
        lastPlatformZ = 0f;
        currentPlayerLane = new MiddleLaneState(null);
        ClearActivePlatforms();
        SpawnInitialPlatform();
        isInitialized = true;
    }

    private void ClearActivePlatforms()
    {
        while (activePlatforms.Count > 0)
        {
            Platform platform = activePlatforms.Dequeue();
            if (platform != null)
            {
                platformPool.ReturnObject(platform);
            }
        }
    }

    private void OnPlayerMoved(object data)
    {
        currentPlayerLane = data as LaneState ?? currentPlayerLane;
        Debug.Log($"Player lane: {currentPlayerLane?.GetType().Name ?? "Null"}");
    }

    private void OnPlatformMidpointReached(object data)
    {
        if (gameStateManager.CurrentState != GameState.Playing || !isInitialized || activePlatforms.Count == 0)
            return;

        if (data is Platform platform)
        {
            Debug.Log($"Platform at position {platform.transform.position} reached midpoint");
            SpawnNextPlatform();
            RecycleOldestPlatform(player.position.z);
        }
    }

    private void SpawnInitialPlatform()
    {
        Platform platform = platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }
        platform.Initialize(config.platformLength, 0);
        platform.transform.position = Vector3.zero;
        activePlatforms.Enqueue(platform);
    }

    private void SpawnNextPlatform()
    {
        Debug.Log("SpawnNextPlatform called");
        Vector3 nextPosition = CalculateNextPlatformPosition();
        Platform platform = platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }
        platform.Initialize(config.platformLength, nextPosition.x);
        platform.transform.position = nextPosition;
        activePlatforms.Enqueue(platform);
        lastPlatformZ = nextPosition.z;
    }

    private Vector3 CalculateNextPlatformPosition()
    {
        float nextZ;
        float nextX = 0;
        bool isAdjacent = false;
        if (currentPlayerLane is LeftLaneState)
        {
            bool stayLeft = Random.value < 0.5f;
            nextX = stayLeft ? -config.laneDistance : 0;
            isAdjacent = stayLeft;
            Debug.Log($"Player in LEFT lane, stayLeft={stayLeft}");
        }
        else if (currentPlayerLane is RightLaneState)
        {
            bool stayRight = Random.value < 0.5f;
            nextX = stayRight ? config.laneDistance : 0;
            isAdjacent = stayRight;
            Debug.Log($"Player in RIGHT lane, stayRight={stayRight}");
        }
        else
        {
            int laneChoice = Random.Range(0, 3);
            nextX = laneChoice switch
            {
                0 => -config.laneDistance,
                1 => 0,
                _ => config.laneDistance
            };
            isAdjacent = (laneChoice == 1);
            Debug.Log($"Player in MIDDLE lane, laneChoice={laneChoice}");
        }
        nextZ = isAdjacent ? lastPlatformZ + config.platformGap : lastPlatformZ + config.platformHalfGap;
        return new Vector3(nextX, 0, nextZ);
    }

    private void RecycleOldestPlatform(float playerZ)
    {
        if (activePlatforms.Count == 0) return;
        Platform oldestPlatform = activePlatforms.Peek();
        float recycleThreshold = oldestPlatform.transform.position.z + config.platformGap;
        if (playerZ > recycleThreshold)
        {
            platformPool.ReturnObject(oldestPlatform);
            activePlatforms.Dequeue();
            Debug.Log($"Recycled platform at {oldestPlatform.transform.position}, remaining platforms: {activePlatforms.Count}");
        }
    }

    private void OnDestroy()
    {
        eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        eventSystem.Unsubscribe(GameEventType.GameStarted, OnGameStarted);
        Debug.Log("PlatformManager destroyed and unsubscribed from events");
    }
}
