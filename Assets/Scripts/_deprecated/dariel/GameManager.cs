using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<GameObject> _enemiesObjects = new List<GameObject>();
    private List<GameObject> _towersObjects = new List<GameObject>();
    private List<GameObject> _platformsObjects = new List<GameObject>();
    private List<Enemy> _enemies;
    private List<Tower> _towers;
    private List<GameObject> _spawnPoints;
    public float gridSize = 0.5f;
    [SerializeField]
    private GameObject _platformPref;
    [SerializeField]
    private GameObject _towerPref;
    [SerializeField]
    private GameObject _enemyPref;
    [SerializeField]
    private GameObject _platformContainer;
    [SerializeField]
    private GameObject _towerContainer;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _spawnPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        UIManager.instance.InitUI();
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(_spawnPoint.transform.position.x, _spawnPoint.transform.position.y + i * 0.2f, _spawnPoint.transform.position.z);
            GameObject enemy = Instantiate(_enemyPref, pos, Quaternion.identity, _enemyContainer.transform);
            _enemiesObjects.Add(enemy);
        }
    }

    public List<GameObject> GetAllEnemies()
    {
        return _enemiesObjects;
    }

    public GameObject GetPlatformContainer()
    {
        return _platformContainer;
    }

    public GameObject GetTowerContainer()
    {
        return _towerContainer;
    }

    public void PlaceStructures(Vector3 position, Structures structToPlace)
    {
        switch (structToPlace)
        {
            case Structures.Platform:
                Debug.Log("PlacePlatform");
                GameObject platform = Instantiate(_platformPref, position, Quaternion.identity, _platformContainer.transform);
                _platformsObjects.Add(platform);
                break;
            case Structures.SimpleTower:
                Debug.Log("Place SimpleTower");
                GameObject tower = Instantiate(_towerPref, position, Quaternion.identity, _towerContainer.transform);
                _towersObjects.Add(tower);
                break;
            case Structures.Construction:
                Debug.Log("Place Construction");
                break;
            default:
                break;
        }
        
    }
}

public enum Structures 
{
    Platform = 0,
    SimpleTower = 1,
    Construction = 2
}
