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
    private EventSystem _eventSystem;
    private GameStateManager _gameStateManager;
    private bool _isInitialized = false;

    private void Awake()
    {
        ValidateSetup();
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
        // Check if GameManager is ready before initializing pool
        if (GameManager.Instance == null || GameManager.Instance.GameConfig == null)
        {
            Debug.LogError("GameManager or GameConfig not available for platform pool initialization");
            return;
        }

        Debug.Log("Initializing platform pool");
        _platformPool = new GenericObjectPool<Platform>(
            platformPrefab,
            GameManager.Instance.GameConfig.initialPoolSize,
            10,
            transform
        );
    }

    private void Start()
    {
        // Subscribe to game start event to initialize properly
        _eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (_eventSystem != null)
        {
            _eventSystem.Subscribe(GameEventType.GameStarted, OnGameStarted);
        }

        _gameStateManager = ServiceLocator.Instance.GetService<GameStateManager>();

        // Set default lane
        _currentPlayerLane = new MiddleLaneState(null);

        // Initialize pool
        InitializePool();
    }

    private void OnGameStarted(object data)
    {
        Debug.Log("Game started event received, initializing platforms");
        // Clear any existing platforms from previous runs
        ClearActivePlatforms();

        // Initialize with first platform
        SpawnInitialPlatform();

        _isInitialized = true;
    }

    private void ClearActivePlatforms()
    {
        if (_platformPool == null)
        {
            InitializePool();
        }

        if (_platformPool == null)
        {
            Debug.LogError("Platform pool initialization failed");
            return;
        }

        while (_activePlatforms.Count > 0)
        {
            Platform platform = _activePlatforms.Dequeue();
            if (platform != null)
            {
                _platformPool.ReturnObject(platform);
            }
        }

        _lastPlatformZ = 0f;
    }

    private void OnPlayerMoved(object data)
    {
        _currentPlayerLane = data as LaneState ?? _currentPlayerLane;
        Debug.Log($"Player lane: {_currentPlayerLane?.GetType().Name ?? "Null"}");
    }

    private void OnPlatformMidpointReached(object data)
    {
        // Check if game is currently in playing state
        if (_gameStateManager != null && _gameStateManager.CurrentState != GameState.Playing)
        {
            return;
        }

        if (_activePlatforms.Count == 0) return;
        SpawnNextPlatform();
        RecycleOldestPlatform(player.position.z);
    }

    private void SpawnInitialPlatform()
    {
        if (_platformPool == null)
        {
            Debug.LogError("Platform pool not initialized");
            return;
        }

        Platform platform = _platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }

        platform.Initialize(GameManager.Instance.GameConfig.platformLength, 0);
        platform.transform.position = Vector3.zero;
        _activePlatforms.Enqueue(platform);

        Debug.Log("Initial platform spawned");
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

        Debug.Log($"Next platform spawned at {nextPosition}");
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

        if (isAdjacent)
        {
            // For same lane, we want gap between platforms
            nextZ = _lastPlatformZ + GameManager.Instance.GameConfig.platformGap;
        }
        else
        {
            // For lane change, use half gap
            nextZ = _lastPlatformZ + GameManager.Instance.GameConfig.platformHalfGap;
        }

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
            Debug.Log("Recycled old platform");
        }
    }

    private void RegisterServices()
    {
        ServiceLocator.Instance.RegisterService(this);
        _eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (_eventSystem != null)
        {
            _eventSystem.Subscribe(GameEventType.PlayerMoved, OnPlayerMoved);
            _eventSystem.Subscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        }
    }

    private void OnDestroy()
    {
        if (_eventSystem != null)
        {
            _eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
            _eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
            _eventSystem.Unsubscribe(GameEventType.GameStarted, OnGameStarted);
        }
        ServiceLocator.Instance?.RemoveService<PlatformManager>();
    }
}