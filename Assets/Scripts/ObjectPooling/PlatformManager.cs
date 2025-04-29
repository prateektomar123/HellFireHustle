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
    private bool _isSubscribed = false;

    private void Awake()
    {
        Debug.Log("PlatformManager: Awake called");
        ValidateSetup();
        _activePlatforms = new Queue<Platform>(); // Ensure we start with a fresh queue
        _currentPlayerLane = new MiddleLaneState(null);
    }

    private void ValidateSetup()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("No platform prefab assigned.", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player transform not assigned.", this);
            enabled = false;
            return;
        }

        Debug.Log("PlatformManager: Validation successful");
    }

    private void InitializePool()
    {
        // Check if GameManager is ready before initializing pool
        if (GameManager.Instance == null || GameManager.Instance.GameConfig == null)
        {
            Debug.LogError("GameManager or GameConfig not available for platform pool initialization");
            return;
        }

        // Ensure pool is only created once
        if (_platformPool == null)
        {
            Debug.Log("Initializing platform pool");
            _platformPool = new GenericObjectPool<Platform>(
                platformPrefab,
                GameManager.Instance.GameConfig.initialPoolSize,
                10,
                transform
            );
            Debug.Log($"Platform pool initialized with {GameManager.Instance.GameConfig.initialPoolSize} objects");
        }
        else
        {
            Debug.Log("Platform pool already initialized");
        }
    }

    private void Start()
    {
        Debug.Log("PlatformManager: Start called");
        RegisterServices();
    }

    private void RegisterServices()
    {
        Debug.Log("PlatformManager: Registering services");
        ServiceLocator.Instance.RegisterService(this);

        // Initialize EventSystem reference
        _eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (_eventSystem == null)
        {
            Debug.LogError("EventSystem not found in ServiceLocator");
            return;
        }

        SubscribeToEvents();

        _gameStateManager = ServiceLocator.Instance.GetService<GameStateManager>();
        if (_gameStateManager == null)
        {
            Debug.LogWarning("GameStateManager not found in ServiceLocator");
        }
        else
        {
            Debug.Log($"Current game state: {_gameStateManager.CurrentState}");
        }

        // Initialize pool
        InitializePool();
        Debug.Log("PlatformManager: Services registered");
    }

    private void SubscribeToEvents()
    {
        if (_eventSystem == null || _isSubscribed)
        {
            return;
        }

        // Unsubscribe first to prevent duplicate subscriptions
        _eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        _eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        _eventSystem.Unsubscribe(GameEventType.GameStarted, OnGameStarted);

        // Re-subscribe to events
        _eventSystem.Subscribe(GameEventType.PlayerMoved, OnPlayerMoved);
        _eventSystem.Subscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
        _eventSystem.Subscribe(GameEventType.GameStarted, OnGameStarted);

        _isSubscribed = true;
        Debug.Log("PlatformManager successfully subscribed to events");
    }

    private void OnGameStarted(object data)
    {
        Debug.Log("PlatformManager: Game started event received, initializing platforms");
        // Initialize with fresh state
        _isInitialized = false;
        _lastPlatformZ = 0f;
        _currentPlayerLane = new MiddleLaneState(null);

        // Clear any existing platforms from previous runs
        ClearActivePlatforms();

        // Initialize with first platform
        SpawnInitialPlatform();

        _isInitialized = true;
        Debug.Log("Platform Manager initialized: " + _isInitialized);
    }

    private void ClearActivePlatforms()
    {
        Debug.Log($"Clearing {_activePlatforms.Count} active platforms");

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

        Debug.Log("All active platforms cleared");
    }

    private void OnPlayerMoved(object data)
    {
        _currentPlayerLane = data as LaneState ?? _currentPlayerLane;
        Debug.Log($"Player lane: {_currentPlayerLane?.GetType().Name ?? "Null"}");
    }

    private void OnPlatformMidpointReached(object data)
    {
        // Debug log to verify this event is being received
        Debug.Log("Platform midpoint reached event received with data: " + (data != null ? data.GetType().Name : "null"));

        Platform platform = data as Platform;
        if (platform != null)
        {
            Debug.Log($"Platform at position {platform.transform.position} reached midpoint");
        }

        // Check if game is currently in playing state
        if (_gameStateManager != null && _gameStateManager.CurrentState != GameState.Playing)
        {
            Debug.Log("Game not in playing state, ignoring platform midpoint event. Current state: " + _gameStateManager.CurrentState);
            return;
        }

        if (!_isInitialized)
        {
            Debug.LogWarning("PlatformManager not initialized yet, cannot spawn platform");
            return;
        }

        if (_activePlatforms.Count == 0)
        {
            Debug.LogWarning("No active platforms, cannot spawn next platform");
            return;
        }

        Debug.Log("Spawning next platform from midpoint event");
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

        Debug.Log($"Initial platform spawned at {platform.transform.position}");
    }

    private void SpawnNextPlatform()
    {
        Debug.Log("SpawnNextPlatform called");
        Vector3 nextPosition = CalculateNextPlatformPosition();

        if (_platformPool == null)
        {
            Debug.LogError("Platform pool is null when trying to spawn next platform");
            return;
        }

        Platform platform = _platformPool.GetObject();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool.", this);
            return;
        }

        Debug.Log($"Next platform position calculated: {nextPosition}");
        platform.Initialize(GameManager.Instance.GameConfig.platformLength, nextPosition.x);
        platform.transform.position = nextPosition;
        _activePlatforms.Enqueue(platform);
        _lastPlatformZ = nextPosition.z;

        Debug.Log($"Next platform spawned at {nextPosition}, active platforms: {_activePlatforms.Count}");
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
            Debug.Log($"Player in LEFT lane, stayLeft={stayLeft}");
        }
        else if (_currentPlayerLane is RightLaneState)
        {
            // When in right lane, next platform can only be right or middle
            bool stayRight = Random.value < 0.5f;
            nextX = stayRight ? GameManager.Instance.GameConfig.laneDistance : 0;
            isAdjacent = stayRight; // Adjacent only if staying in right lane
            Debug.Log($"Player in RIGHT lane, stayRight={stayRight}");
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
            Debug.Log($"Player in MIDDLE lane, laneChoice={laneChoice}");
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
        float recycleThreshold = oldestPlatform.transform.position.z + GameManager.Instance.GameConfig.platformGap;

        Debug.Log($"Checking if platform should be recycled. Player Z: {playerZ}, Recycle threshold: {recycleThreshold}");

        if (playerZ > recycleThreshold)
        {
            _platformPool.ReturnObject(oldestPlatform);
            _activePlatforms.Dequeue();
            Debug.Log($"Recycled platform at {oldestPlatform.transform.position}, remaining platforms: {_activePlatforms.Count}");
        }
    }

    private void OnDestroy()
    {
        if (_eventSystem != null && _isSubscribed)
        {
            _eventSystem.Unsubscribe(GameEventType.PlayerMoved, OnPlayerMoved);
            _eventSystem.Unsubscribe(GameEventType.PlatformMidpointReached, OnPlatformMidpointReached);
            _eventSystem.Unsubscribe(GameEventType.GameStarted, OnGameStarted);
            _isSubscribed = false;
        }

        ServiceLocator.Instance?.RemoveService<PlatformManager>();
        Debug.Log("PlatformManager destroyed and unsubscribed from events");
    }

    // Check initialization status in Update for the first few frames
    private float _checkTime = 0f;
    private bool _hasCheckedInitialization = false;

    private void Update()
    {
        // Only run these checks for the first few seconds
        if (_hasCheckedInitialization) return;

        _checkTime += Time.deltaTime;
        if (_checkTime > 5f)
        {
            _hasCheckedInitialization = true;
            return;
        }

        // Check if we have necessary references
        if (_eventSystem == null)
        {
            Debug.LogWarning("EventSystem reference is missing, trying to get it again");
            _eventSystem = ServiceLocator.Instance?.GetService<EventSystem>();
            if (_eventSystem != null)
            {
                SubscribeToEvents();
            }
        }

        if (!_isInitialized && _gameStateManager != null && _gameStateManager.CurrentState == GameState.Playing)
        {
            Debug.Log("Game is playing but PlatformManager not initialized, forcing initialization");
            OnGameStarted(null);
        }
    }
}