using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector3 gridCentre;
    public Vector2Int gridDimensions;
    public float squareTileDimensions = 1f;
    public float hexTileDimensions = 0.9f;
    public GameObject squareTilePrefab, hexTilePrefab;
    public TileGrid.TileGridShape gridShape = TileGrid.TileGridShape.SQUARE;
    public static GridGenerator instance;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        switch(gridShape)
        {
            case TileGrid.TileGridShape.SQUARE:
                TileGrid.instance.Initialise(gridCentre,gridDimensions,squareTilePrefab,squareTileDimensions,gridShape);
                break;
            case TileGrid.TileGridShape.HEX:
                TileGrid.instance.Initialise(gridCentre,gridDimensions,hexTilePrefab,hexTileDimensions,gridShape);
                break;
        }
    }
    public void SetHeight(int _height)
    {
        gridDimensions.y = _height;
        GenerateGrid();
    }
    public void SetWidth(int _width)
    {
        gridDimensions.x = _width;
        GenerateGrid();
    }
    public void SetGridShape(int _index )
    {
        gridShape = (TileGrid.TileGridShape)_index;
        GenerateGrid();
    }
}
