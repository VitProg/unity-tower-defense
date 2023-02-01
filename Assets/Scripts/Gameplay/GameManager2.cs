using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager2 : MonoBehaviour
{
    static GameManager2 instance;

    [SerializeField] private GameObject ground;

    private bool[,] map = new bool[,] {};
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    string mapStr = "";
    
    // Start is called before the first frame update
    void Start()
    {
        var tilemap = ground.GetComponent<Tilemap>();
        Debug.Log(tilemap.size);
        
        var spots = new Vector3Int[tilemap.size.x, tilemap.size.y];
        for (int x = tilemap.origin.x, i = 0; i < (tilemap.size.x); x++, i++)
        {
            for (int y = tilemap.origin.y, j = 0; j < (tilemap.size.y); y++, j++)
            {
                spots[i, j] = new Vector3Int(x, y, tilemap.HasTile(new Vector3Int(x, y, 0)) ? 0 : 1);
                mapStr += spots[i, j].z == 1 ? '1' : '0';
            }
            mapStr += "\n";
        }
        
        Debug.Log(spots);

        // map = new bool[grid.size.x, grid.size.y];
        //
        // for (int y = 0; y < grid.size.y; y++)
        // {
        //     for (int x = 0; x < grid.size.x; x++)
        //     {
        //         map[x, y] = grid.HasTile(new Vector3Int(x, y));
        //     }
        // }
        //
        // string mapStr = "";
        // for (int y = 0; y < grid.size.y; y++)
        // {
        //     for (int x = 0; x < grid.size.x; x++)
        //     {
        //         mapStr += map[x, y] ? '_' : '#';
        //     }
        //
        //     mapStr += "\n";
        // }
        //
        Debug.Log(mapStr);

        //
        // levelGrid
        //
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
