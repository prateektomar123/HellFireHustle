//this script manages the spawning and recycling of fire ground objects in a game.
// it uses a generic object pool to manage fire ground instances and handles player movement.
// the script also subscribes to game events to trigger fire ground spawning and recycling based on player position and game state.
// it ensures that fire ground objects are recycled when they are no longer needed, optimizing memory usage and performance.
using System.Collections.Generic;
using UnityEngine;
public class FireGroundManager : MonoBehaviour
{
    [SerializeField] private GameObject fireGroundPrefab;
    [SerializeField] private Transform player;
    private GenericObjectPool<FireGround> fireGroundPool;
    private Queue<FireGround> activeFireGrounds = new();
    private float lastFireGroundZ;
    private GameConfig config;

    private void Awake()
    {
        if (fireGroundPrefab == null || player == null)
        {
            Debug.LogError("FireGroundManager: Missing prefab or player reference.", this);
            enabled = false;
            return;
        }
        config = ServiceLocator.Instance.GetService<GameConfig>();
        InitializePool();
    }

    private void InitializePool()
    {
        fireGroundPool = new GenericObjectPool<FireGround>(
            fireGroundPrefab,
            config.initialPoolSize,
            config.initialPoolSize,
            transform
        );
    }

    private void Start()
    {
        SpawnInitialFireGrounds();
        lastFireGroundZ = config.fireGroundLength;
    }

    private void Update()
    {
        if (activeFireGrounds.Count == 0) return;
        float playerZ = player.position.z;
        if (playerZ > activeFireGrounds.Peek().transform.position.z + config.fireGroundLength)
        {
            RepositionFireGround();
        }
    }

    private void SpawnInitialFireGrounds()
    {
        FireGround firstGround = fireGroundPool.GetObject();
        firstGround.Initialize(config.fireGroundLength);
        firstGround.transform.position = new Vector3(0, config.fireGroundYPosition, 0);
        activeFireGrounds.Enqueue(firstGround);
        // SpawnSecondFireGround();
        FireGround secondGround = fireGroundPool.GetObject();
        secondGround.Initialize(config.fireGroundLength);
        secondGround.transform.position = new Vector3(0, config.fireGroundYPosition, config.fireGroundLength);
        activeFireGrounds.Enqueue(secondGround);
    }

    private void RepositionFireGround()
    {
        FireGround oldestFireGround = activeFireGrounds.Dequeue();
        lastFireGroundZ += config.fireGroundLength;
        oldestFireGround.transform.position = new Vector3(0, config.fireGroundYPosition, lastFireGroundZ);
        activeFireGrounds.Enqueue(oldestFireGround);
    }
}