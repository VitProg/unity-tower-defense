using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<GameObject> enemiesObjects = new List<GameObject>();
    private List<GameObject> towersObjects = new List<GameObject>();
    private List<GameObject> platformsObjects = new List<GameObject>();
    private List<Enemy> _enemies;
    private List<Tower> _towers;
    private List<GameObject> _spawnPoints;
    public float gridSize = 0.5f;
    [SerializeField]
    private GameObject _platformPref;
    [SerializeField]
    private GameObject _towerPref;
    [SerializeField]
    private GameObject _platformContainer;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlacePlatform(Vector3 position)
    {
        Debug.Log("PlacePlatform");
        GameObject platform = Instantiate(_platformPref, position, Quaternion.identity, _platformContainer.transform);
        platformsObjects.Add(platform);
    }

    
}
