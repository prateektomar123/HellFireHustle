
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnvironmentPooling : MonoBehaviour {

    
    private static EnvironmentPooling instance;
    public static EnvironmentPooling Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<EnvironmentPooling>();
            return instance;
        }
    }

    [System.Serializable]
    public class RoadObjects {

        public GameObject roadObject;

    }

    public int roadAmountInPool = 5;      
    private float roadLength;     
    public float manualRoadLength = 60f;       
    public RoadObjects[] roadObjects;
    internal List<GameObject> roads = new List<GameObject>();
    internal GameObject allRoads;      
    private int index = 0;

    private void Awake()
    {
        int environmentType = 0;

        
        environmentType = Mathf.Clamp(environmentType, 0, roadObjects.Length - 1);
        roadLength = manualRoadLength;

        CreateRoads(environmentType);

        foreach (RoadObjects road in roadObjects)
        {
            road.roadObject.SetActive(false);
        }
    }
    private void CreateRoads(int selectedType)
    {
        allRoads = new GameObject("All Roads");
        roads.Clear(); 

        Vector3 nextPosition = new Vector3(0f, -0.63f, 0f); ; 

        for (int i = 0; i < roadAmountInPool; i++)
        {
            GameObject go = Instantiate(roadObjects[selectedType].roadObject, nextPosition, Quaternion.identity);
            go.isStatic = false;
            roads.Add(go);
            go.transform.SetParent(allRoads.transform);
            nextPosition.z += roadLength; 
        }

        index = 0; 
    }
    private void Update() 
    {
        AnimateRoads();
    }
    private void AnimateRoads()
    {
        if (!Camera.main) return;
        
        float viewThreshold = Camera.main.transform.position.z - 10; 

        foreach (GameObject road in roads)
        {
            if (road.transform.position.z + roadLength < viewThreshold)
            {
                float maxZ = roads.Max(r => r.transform.position.z);
                road.transform.position = new Vector3(0f, road.transform.position.y, maxZ + roadLength);
            }
        }
    }



}
