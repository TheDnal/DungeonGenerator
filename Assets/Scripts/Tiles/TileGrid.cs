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
    private List<Partition> partitions = new List<Partition>();
    public int parititonDepth = 1;
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
        PartitionGrid(parititonDepth);
        PaintPartitions();
    }
    public void GenerateGrid()
    {
        //Clear tile grid
        DeleteGrid();
        //Re initialise grid
        tiles = new Tile[gridDimensions.x,gridDimensions.y];
        //Generate tiles
        for(int i = 0; i < gridDimensions.x; i++){
            for(int j = 0; j < gridDimensions.y; j++)
            {
                Vector2Int coords = new Vector2Int(i,j);
                Vector3 worldPos = gridCorner + new Vector3(i * tileSpacing, 0, j * tileSpacing);
                GameObject obj = Instantiate(tilePrefab,worldPos,Quaternion.identity);
                Tile newTile = new Tile(coords,obj,false);
                newTile.SetColor(Color.grey);
                tiles[i,j] = newTile;
            }
        }
    }
    public void ResetGrid()
    {
        foreach(Tile tile in tiles)
        {
            tile.SetActive(false);
            tile.SetColor(Color.grey);
        }
    }
    public void DeleteGrid()
    {
        if(tiles == null){return;}
        foreach(Tile tile in tiles)
        {
            tile.DenInitialise();
        }
    }
    public void PlaceRoom()
    {
        int xSize = Random.Range(2,5);
        int ySize = Random.Range(2,5);
        int xCorner = Random.Range(0, gridDimensions.x - xSize);
        int yCorner = Random.Range(0, gridDimensions.y - ySize);
        Vector2Int corner = new Vector2Int(xCorner,yCorner);
        for(int i = 0; i < xSize; i++){
            for(int j = 0; j < ySize; j++)
            {
                Vector2Int pos = corner + new Vector2Int(i,j);
                tiles[pos.x,pos.y].SetActive(true);
                tiles[pos.x,pos.y].SetColor(Color.white);
            }
        }
    }
    public void PartitionGrid(int depth = 2)
    {
        bool partitionDirection = false;
        partitions = new List<Partition>();
        Partition master = new Partition(Vector2Int.zero, gridDimensions);
        List<Partition> parititonsToBisect = new List<Partition>();
        List<Partition> temp = new List<Partition>();
        parititonsToBisect.Add(master);
        partitions.Add(master);
        for(int i = 0; i < depth; i++)
        {
            temp.Clear();
            foreach(Partition currPartition in parititonsToBisect)
            {
                if(!currPartition.isBisectable()){continue;}
                float rnd = Random.Range(0,10);
                partitionDirection = rnd > 5 ? true : false;
                List<Partition> children = currPartition.bisectPartition(partitionDirection);
                currPartition.SetChildren(children);
                temp.AddRange(children);
            }
            parititonsToBisect.Clear();
            parititonsToBisect.AddRange(temp);
            partitions.AddRange(temp);
        }
    }
    public void PaintPartitions()
    {
        foreach(Partition p in partitions)
        {
            if(p.children.Count != 0){continue;}
            Color col = Random.ColorHSV();
            for(int i = 0; i < p.dimensions.x; i++){
            for(int j = 0; j < p.dimensions.y; j++)
            {
                Vector2Int coords = p.corner + new Vector2Int(i,j);
                tiles[coords.x,coords.y].SetColor(col);
            }
            }
        }
    }
}
