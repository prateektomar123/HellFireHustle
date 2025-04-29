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
            GameManager.Instance.GameConfig.initialPoolSize,
            GameManager.Instance.GameConfig.initialPoolSize,
            this.transform
        );
    }
    private void Start()
    {
        SpawnInitialFireGrounds();
        _lastFireGroundZ = GameManager.Instance.GameConfig.fireGroundLength;
    }
    private void Update()
    {
        if (_activeFireGrounds.Count == 0) return;
        float playerZ = player.position.z;
        if (playerZ > _activeFireGrounds.Peek().transform.position.z + GameManager.Instance.GameConfig.fireGroundLength)
        {
            RepositionFireGround();
        }
    }
    private void SpawnInitialFireGrounds()
    {
        FireGround firstGround = _fireGroundPool.GetObject();
        firstGround.Initialize(GameManager.Instance.GameConfig.fireGroundLength);
        firstGround.transform.position = new Vector3(0, GameManager.Instance.GameConfig.fireGroundYPosition, 0);
        _activeFireGrounds.Enqueue(firstGround);
        //----------------------------------------
        FireGround secondGround = _fireGroundPool.GetObject();
        secondGround.Initialize(GameManager.Instance.GameConfig.fireGroundLength);
        secondGround.transform.position = new Vector3(0, GameManager.Instance.GameConfig.fireGroundYPosition, GameManager.Instance.GameConfig.fireGroundLength);
        _activeFireGrounds.Enqueue(secondGround);
    }

    private void RepositionFireGround()
    {
        FireGround oldestFireGround = _activeFireGrounds.Dequeue();
        _lastFireGroundZ += GameManager.Instance.GameConfig.fireGroundLength;
        oldestFireGround.transform.position = new Vector3(0, GameManager.Instance.GameConfig.fireGroundYPosition, _lastFireGroundZ);
        _activeFireGrounds.Enqueue(oldestFireGround);
    }
}