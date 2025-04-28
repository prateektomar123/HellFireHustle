using System.Collections.Generic;
using UnityEngine;

public class FireGroundManager : MonoBehaviour
{
    [SerializeField] private GameObject fireGroundPrefab;
    [SerializeField] private Transform player;

    private GenericObjectPool<FireGround> _fireGroundPool;
    private Queue<FireGround> _activeFireGrounds = new();
    private float _lastFireGroundZ;

    private void Awake()
    {
        ValidateSetup();
        InitializePool();
        RegisterServices();
    }

    private void ValidateSetup()
    {
        if (fireGroundPrefab == null)
        {
            Debug.LogError("No fire ground prefab assigned.");
            enabled = false;
        }
        if (player == null)
        {
            Debug.LogError("Player transform not assigned.");
            enabled = false;
        }
    }

    private void InitializePool()
    {
        _fireGroundPool = new GenericObjectPool<FireGround>(
            fireGroundPrefab,
            2,
            2,
            this.transform
        );
    }

    private void Start()
    {
        SpawnInitialFireGrounds();
        _lastFireGroundZ = GameConstants.FIRE_GROUND_LENGTH;
    }

    private void Update()
    {
        if (_activeFireGrounds.Count == 0) return;
        float playerZ = player.position.z;
        if (playerZ > _activeFireGrounds.Peek().transform.position.z + GameConstants.FIRE_GROUND_LENGTH)
        {
            RepositionFireGround();
        }
    }

    private void SpawnInitialFireGrounds()
    {
        // First FireGround at Z = 0
        FireGround firstGround = _fireGroundPool.GetObject();
        firstGround.Initialize(GameConstants.FIRE_GROUND_LENGTH);
        firstGround.transform.position = new Vector3(0, GameConstants.FIRE_GROUND_Y_POSITION, 0);
        _activeFireGrounds.Enqueue(firstGround);

        // Second FireGround at Z = FIRE_GROUND_LENGTH
        FireGround secondGround = _fireGroundPool.GetObject();
        secondGround.Initialize(GameConstants.FIRE_GROUND_LENGTH);
        secondGround.transform.position = new Vector3(0, GameConstants.FIRE_GROUND_Y_POSITION, GameConstants.FIRE_GROUND_LENGTH);
        _activeFireGrounds.Enqueue(secondGround);
    }

    private void RepositionFireGround()
    {
        FireGround oldestFireGround = _activeFireGrounds.Dequeue();
        _lastFireGroundZ += GameConstants.FIRE_GROUND_LENGTH;
        oldestFireGround.transform.position = new Vector3(0, GameConstants.FIRE_GROUND_Y_POSITION, _lastFireGroundZ);
        _activeFireGrounds.Enqueue(oldestFireGround);
    }

    private void RegisterServices()
    {
        //ServiceLocator.Instance.RegisterService(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.RemoveService<FireGroundManager>();
    }
}