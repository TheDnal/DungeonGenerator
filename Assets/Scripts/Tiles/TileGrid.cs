using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private Tile[,] tiles;
    public GameObject tilePrefab;
    public Vector2Int gridDimensions;
    public float tileSpacing = 1;
    private Vector3 gridCorner;
    private enum GridState
    {
        inactive, 
        generating,
        active
    }
    private GridState currState = GridState.inactive;
    void Awake()
    {
        //Clamp dimensions;
        gridDimensions.x = Mathf.Clamp(gridDimensions.x,1,50);
        gridDimensions.y = Mathf.Clamp(gridDimensions.y,1,50);
        gridCorner = Vector3.zero - new Vector3(gridDimensions.x / 2, 0, gridDimensions.y / 2);
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        //Clear tile grid
        if(tiles != null){foreach(Tile currTile in tiles){currTile.DenInitialise();}}
        //Re initialise grid
        tiles = new Tile[gridDimensions.x,gridDimensions.y];
        //Generate tiles
        for(int i = 0; i < gridDimensions.x; i++){
            for(int j = 0; j < gridDimensions.y; j++)
            {
                Vector2Int coords = new Vector2Int(i,j);
                Vector3 worldPos = gridCorner + new Vector3(i * tileSpacing, 0, j * tileSpacing);
                GameObject obj = Instantiate(tilePrefab,worldPos,Quaternion.identity);
                int rnd = Random.Range(0,100);
                bool active = rnd > 50 ? true : false;
                Tile newTile = new Tile(coords,obj,active);
                Color col = active ? Color.white : Color.grey;
                newTile.SetColor(col);
                tiles[i,j] = newTile;
            }
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            GenerateGrid();
    }
}
