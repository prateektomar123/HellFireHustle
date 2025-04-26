using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlatformPrefab
{
    public GameObject prefab;
}

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private PlatformPrefab[] platformPrefabs;
    [SerializeField] private Transform player;
    private GenericObjectPool<Platform> platformPool;
    private Queue<Platform> activePlatforms;
    private LaneState currentPlayerLane;
    private float lastSpawnZ;

    private void Awake()
    {
        if (platformPrefabs.Length == 0 || platformPrefabs[0].prefab == null)
        {
            Debug.LogError("EnvironmentManager: No platform prefabs assigned.");
            enabled = false;
            return;
        }
        if (player == null)
        {
            Debug.LogError("EnvironmentManager: Player transform not assigned.");
            enabled = false;
            return;
        }

        platformPool = new GenericObjectPool<Platform>(
            platformPrefabs[0].prefab,
            GameConstants.INITIAL_POOL_SIZE,
            transform
        );
        activePlatforms = new Queue<Platform>();
        ServiceLocator.Instance.RegisterService(this);

        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Subscribe("PlayerMoved", OnPlayerMoved);
        }
        else
        {
            Debug.LogError("EventSystem not found in EnvironmentManager.Awake.");
        }
    }

    private void Start()
    {
        currentPlayerLane = new MiddleLaneState(null);
        SpawnInitialPlatform();
        lastSpawnZ = activePlatforms.Peek().GetHalfwayPointZ();
    }

    private void Update()
    {
        if (activePlatforms.Count == 0) return;

        Platform oldestPlatform = activePlatforms.Peek();
        float playerZ = player.position.z;
        if (playerZ >= oldestPlatform.GetHalfwayPointZ())
        {
            SpawnNextPlatform();
            RecycleOldestPlatform(playerZ);
        }
    }

    private void OnPlayerMoved(object data)
    {
        currentPlayerLane = data as LaneState;
        Debug.Log($"Player lane updated: {currentPlayerLane?.GetType().Name ?? "Null"}");
    }

    private void SpawnInitialPlatform()
    {
        Platform platform = platformPool.GetObject();
        platform.Initialize(GameConstants.PLATFORM_LENGTH, 0);
        platform.transform.position = Vector3.zero;
        activePlatforms.Enqueue(platform);
        Debug.Log("Spawned initial platform at X: 0, Z: 0");
    }

    private void SpawnNextPlatform()
    {
        float nextZ = lastSpawnZ + GameConstants.PLATFORM_LENGTH;
        float nextX = 0;

        switch (currentPlayerLane)
        {
            case MiddleLaneState:
                int laneChoice = Random.Range(0, 3);
                Debug.Log($"Middle lane choice: {laneChoice}");
                nextX = laneChoice switch
                {
                    0 => -GameConstants.LANE_DISTANCE,
                    1 => 0,
                    _ => GameConstants.LANE_DISTANCE
                };
                if (laneChoice != 1)
                {
                    nextZ = lastSpawnZ + (GameConstants.PLATFORM_LENGTH / 2f);
                }
                break;

            case LeftLaneState:
                nextX = Random.value < 0.5f ? -GameConstants.LANE_DISTANCE : 0;
                if (nextX == 0) nextZ -= GameConstants.SPAWN_DISTANCE;
                break;

            case RightLaneState:
                nextX = Random.value < 0.5f ? GameConstants.LANE_DISTANCE : 0;
                if (nextX == 0) nextZ -= GameConstants.SPAWN_DISTANCE;
                break;

            default:
                nextX = 0;
                Debug.LogWarning("Fallback to Middle lane due to null lane state");
                break;
        }

        Platform platform = platformPool.GetObject();
        platform.Initialize(GameConstants.PLATFORM_LENGTH, nextX);
        platform.transform.position = new Vector3(nextX, 0, nextZ);
        activePlatforms.Enqueue(platform);
        Debug.Log($"Spawned platform at X: {nextX}, Z: {nextZ}");
        lastSpawnZ = nextZ;
    }

    private void RecycleOldestPlatform(float playerZ)
    {
        if (activePlatforms.Count == 0) return;

        Platform oldestPlatform = activePlatforms.Peek();
        if (playerZ > oldestPlatform.transform.position.z + GameConstants.PLATFORM_LENGTH)
        {
            platformPool.ReturnObject(oldestPlatform);
            activePlatforms.Dequeue();
        }
    }

    private void OnDestroy()
    {
        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Unsubscribe("PlayerMoved", OnPlayerMoved);
        }
        ServiceLocator.Instance.RemoveService<EnvironmentManager>();
    }
}