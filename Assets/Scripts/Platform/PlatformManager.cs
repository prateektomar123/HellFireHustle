using UnityEngine;
using System;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    private ObjectPool platformPool;
    private LaneState currentPlayerLane;
    private Platform currentPlatform;
    private readonly float laneDistance = 2f;
    private readonly float platformLength = 10f;
    private readonly float spawnDistance = 2f;
    private float lastSpawnZ;

    private void Awake()
    {
        platformPool = new ObjectPool(platformPrefab, 10, transform);
        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Subscribe("PlayerMoved", OnPlayerMoved);
        }
        else
        {
            Debug.LogError("EventSystem not found in PlatformManager.Awake. Ensure GameManager initializes first.");
        }
    }

    private void Start()
    {
        currentPlayerLane = new MiddleLaneState(null);
        SpawnPlatform(0, 0);
        currentPlatform = GetActivePlatform();
        lastSpawnZ = currentPlatform.GetHalfwayPointZ();
    }

    private void Update()
    {
        float playerZ = GameObject.FindGameObjectWithTag("Player").transform.position.z;
        if (playerZ >= currentPlatform.GetHalfwayPointZ())
        {
            SpawnNextPlatform();
            currentPlatform = GetActivePlatform();
        }
        RecycleOldPlatforms(playerZ);
    }

    private void OnPlayerMoved(object data)
    {
        currentPlayerLane = data as LaneState;
    }

    private void SpawnNextPlatform()
    {
        float nextZ = lastSpawnZ + platformLength;
        float nextX = 0;

        switch (currentPlayerLane)
        {
            case MiddleLaneState:
                int laneChoice = UnityEngine.Random.Range(0, 3);
                nextX = laneChoice switch
                {
                    0 => -laneDistance,
                    1 => 0,
                    _ => laneDistance
                };
                if (laneChoice != 1)
                {
                    nextZ = lastSpawnZ + (platformLength / 1.5f);
                }
                break;

            case LeftLaneState:
                nextX = UnityEngine.Random.value < 0.5f ? -laneDistance : 0;
                if (nextX == 0) nextZ -= spawnDistance;
                break;

            case RightLaneState:
                nextX = UnityEngine.Random.value < 0.5f ? laneDistance : 0;
                if (nextX == 0) nextZ -= spawnDistance;
                break;

            default:
                nextX = 0;
                break;
        }

        SpawnPlatform(nextX, nextZ);
        lastSpawnZ = nextZ;
    }

    private void SpawnPlatform(float x, float z)
    {
        GameObject platformObj = platformPool.GetObject();
        Platform platform = platformObj.GetComponent<Platform>();
        platform.Initialize(platformLength, x);
        platformObj.transform.position = new Vector3(x, -0.5f, z);
    }

    private Platform GetActivePlatform()
    {
        foreach (var platformObj in platformPool.GetPooledObjects())
        {
            if (platformObj.activeInHierarchy)
            {
                Platform platform = platformObj.GetComponent<Platform>();
                if (platform.GetHalfwayPointZ() > GameObject.FindGameObjectWithTag("Player").transform.position.z)
                {
                    return platform;
                }
            }
        }
        return currentPlatform;
    }

    private void RecycleOldPlatforms(float playerZ)
    {
        foreach (var platformObj in platformPool.GetPooledObjects())
        {
            if (platformObj.activeInHierarchy)
            {
                float platformZ = platformObj.transform.position.z;
                if (playerZ > platformZ + platformLength)
                {
                    platformPool.ReturnObject(platformObj);
                }
            }
        }
    }

    private void OnDestroy()
    {
        var eventSystem = ServiceLocator.Instance.GetService<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.Unsubscribe("PlayerMoved", OnPlayerMoved);
        }
    }
}