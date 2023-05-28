using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    /*
        This class represents the Entire grid of tiles. Accessing any information such as dimensions, getting all tiles 
        or some tiles in a specfic location, updating all tiles etc. is all controlled here.
    */
    private Tile[,] tiles; //All tiles on the grid
    private Vector2Int dimensions; //Size of the grid
    public bool highlightTerrain = false; //Whether or not mouse highlighting will effect terrain
    public int highlightRadius; //highlight radius
    public enum TileGridShape //Shape of the grid, dictates what shape the tiles will be, their arrangement and so much more
    {
        SQUARE,
        HEX
    }
    private TileGridShape shape; //The currents shape
    #region Singleton instance
    //Since this is a class that is widely used, a singleton is an appropriate design pattern to make referencing this class easy
    public static TileGrid instance;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                //If there's already an instance, destroy this new instsance. This prevents any new instances for being made
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }
    #endregion
    public void Initialise(Vector3 centre, Vector2Int _dimensions, GameObject _prefab, float tileSize = 1, TileGridShape gridShape = TileGridShape.SQUARE) //Initialise grid
    {
        shape = gridShape; //Set the shape of the grid, i.e. hexagonal or square
        dimensions = _dimensions; //set the dimensions
        
        //Get corner of Grid
        Vector3 corner = new Vector3(-dimensions.x / 2,
                                     centre.y,
                                     -dimensions.y / 2);
        corner *= tileSize; //multiply by tile size
        corner = centre + corner; //offset by the centre
        
        //Destroy any tiles from any previous iteration
        if(tiles != null)
        {
            foreach(Tile currTile in tiles)
            {
                Destroy(currTile.gameObject);
            }
        }

        //Set 2D array size
        tiles = new Tile[dimensions.x,dimensions.y];
        Vector3 offset;

        //Populat 2D array
        for(int j = 0; j < dimensions.y; j++){
            for(int i = 0; i < dimensions.x; i++)
            {
                offset = new Vector3(i * tileSize, 0, j * tileSize); //get the position of the new tile
                if(shape == TileGridShape.HEX && j % 2 == 0) //Hex tiles have an offset on every other row
                {
                    offset += new Vector3(tileSize/2,0,0); //apply offset
                }
                //Instantiate new tile
                GameObject newTile = Instantiate(_prefab,corner + offset,Quaternion.identity);
                if(gridShape == TileGridShape.HEX) // Rotate hex tiles
                {
                    Vector3 euler = newTile.transform.eulerAngles;
                    euler.x = -90;
                    newTile.transform.eulerAngles = euler;
                }
                tiles[i,j] = newTile.GetComponent<Tile>(); //Get new tile
                newTile.GetComponent<Tile>().Init(new Vector2Int(i,j)); //Apply position
                newTile.GetComponent<Tile>().ResetColor(); //Reset its color
                if(i == 0 || j == 0 || i == dimensions.x - 1 || j == dimensions.y - 1) //If an edge tile, set to edge
                {
                    newTile.GetComponent<Tile>().PaintTile(Tile.TileType.Edge);
                }
            }
        }
    }
    public TileGridShape GetTileGridShape(){return shape;} //Get shape of grid
    public Tile GetTile(Vector2Int _coords) //Get specific tile at coordinate
    {
        if(_coords.x < 0 || _coords.y < 0 || _coords.x >= dimensions.x || _coords.y >= dimensions.y){return null;} //Tile does not exist
        return tiles[_coords.x,_coords.y]; //return tile
    }
    public Tile[,] GetTiles(){return tiles;} //Get all tiles
    public List<Tile> GetUnsortedTiles() //Get tiles in an unsorted form
    {
        List<Tile> temp = new List<Tile>();
        foreach(Tile tile in tiles)
        {
            temp.Add(tile);
        }
        return temp;
    }
    public List<Tile> GetTilesInRegion(Vector2Int _corner, Vector2Int _dimensions) //Get tilse in a square region
    {
        if(_dimensions.x >= dimensions.x || _dimensions.y >= dimensions.y){return null;} //If region is outside of grid
        List<Tile> temp = new List<Tile>();

        //Foreach coordinate in square, add tile
        for(int i = 0; i < _dimensions.x; i++){
            for(int j = 0; j < _dimensions.y; j++)
            {
                if(_corner.x + i < 0 || _corner.y + j < 0 || _corner.x + i >= dimensions.x || _corner.y + j >= dimensions.y){continue;} //Tile does not exist
                temp.Add(tiles[_corner.x + i, _corner.y + j]); //add tile
            }
        }
        return temp; //return tile
    }
    public List<Tile> GetTilesWithinDistance(Vector2Int _coords, float _maxDistance) //Gets all tiles within a radius of a coordiante
    {
        //Because hex grids behave differently the square grids, this method is an approximation and not 100% accurate 
        if(_coords.x < 0 || _coords.y < 0 || _coords.x >= dimensions.x || _coords.y >= dimensions.y){return null;} //return if region outside of grid
        List<Tile> nearbyTiles = new List<Tile>();
        foreach(Tile tile in tiles) //Iterate through all tiles
        {
            //If one coord value is larger than the other, use that as the distance, otherwise the sum of the two values (approximation for hex grid)
            int x,y;
            float distance;
            Vector2Int relativeCoords = tile.GetCoords() - _coords;
            x = Mathf.Abs(relativeCoords.x);
            y = Mathf.Abs(relativeCoords.y);
            if(x > y) {distance = x;}
            else if(x < y) {distance = y;}
            else
            {
                if(x == _maxDistance){distance = _maxDistance + 1;}
                else{distance = x;}
            }
            if(distance <= _maxDistance){nearbyTiles.Add(tile);}
        }
        return nearbyTiles; //return tiles
    }
    public void ClearAllRooms() //Clear any tiles that are inside a room
    {
        foreach(Tile tile in tiles)
        {
            if(tile.type != Tile.TileType.room && tile.type != Tile.TileType.Corridor){continue;} //not a room tile
            tile.ResetColor(); //reset room tile
        }
        GridTerrainGenerator.instance.GenerateTerrain(); //Regenerate terrain that was under the rooms
    }
    public void ResetGrid() //Reset the grid
    {
        foreach(Tile tile in tiles) //Foreach tile, reset them
        {
            if(tile.GetCoords().x == 0 || tile.GetCoords().y == 0 || tile.GetCoords().x == dimensions.x - 1 || tile.GetCoords().y == dimensions.y - 1) //Reset the edges
            {
                tile.PaintTile(Tile.TileType.Edge);
            }
            else //reset the tile color
            {
                tile.ResetColor();
            }
        }
    }
    public void RefreshTileColors() //Refresh all tiles, to remove highlighting
    {
        foreach(Tile tile in tiles)
        { 
            tile.PaintTile(tile.type,true); //reset tile color
        }
    }
    public Vector2Int GetDimensions(){return dimensions;} //Get dimensions of grid
}
