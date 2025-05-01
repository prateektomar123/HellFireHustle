using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Initializer : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "MainScene";
    [SerializeField] private float initializationDelay = 1f;

    void Awake()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}