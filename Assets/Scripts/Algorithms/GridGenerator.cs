using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    /*
        This class controls the generation of the main tile grid
    */
    public Vector3 gridCentre; //Centre of the grid
    public Vector2Int gridDimensions; //Dimensions of the grid
    public float squareTileDimensions = 1f; //Dimensions of square tiles
    public float hexTileDimensions = 0.9f; //dimensions of hex tiles
    public GameObject squareTilePrefab, hexTilePrefab; //tile prefabs
    public TileGrid.TileGridShape gridShape = TileGrid.TileGridShape.SQUARE; //grid shape
    public static GridGenerator instance; //Singleton instance
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
        GenerateGrid(); //Generate on awake
    }
    public void GenerateGrid() //Depending on the grid shape, intiialise the tile grid with the relevant info
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
    public void SetHeight(int _height) //Set the height of the grid
    {
        gridDimensions.y = _height;
        GenerateGrid();
    }
    public void SetWidth(int _width) //Set the width of the grid
    {
        gridDimensions.x = _width;
        GenerateGrid();
    }
    public void SetGridShape(int _index ) //Set the tile shape of the grid
    {
        gridShape = (TileGrid.TileGridShape)_index; //cast the correct shape
        GenerateGrid();
    }
}
