using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public static TileGrid instance;
    private Tile[,] tiles;
    private Vector2Int dimensions;
    public bool highlightTerrain = false;
    public int highlightRadius;
    public enum TileGridShape
    {
        SQUARE,
        HEX,
        TRIANGLE
    }
    private TileGridShape shape;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }
    public void Initialise(Vector3 centre, Vector2Int _dimensions, GameObject _prefab, float tileSize = 1, TileGridShape gridShape = TileGridShape.SQUARE)
    {
        shape = gridShape;
        dimensions = _dimensions;
        //Get corner of Grid
        Vector3 corner = new Vector3(-dimensions.x / 2,
                                     centre.y,
                                     -dimensions.y / 2);
        corner *= tileSize;
        corner = centre + corner;
        if(tiles != null)
        {
            foreach(Tile currTile in tiles)
            {
                Destroy(currTile.gameObject);
            }
        }
        tiles = new Tile[dimensions.x,dimensions.y];
        Vector3 offset;
        for(int j = 0; j < dimensions.y; j++){
            for(int i = 0; i < dimensions.x; i++)
            {
                offset = new Vector3(i * tileSize, 0, j * tileSize);
                if(shape == TileGridShape.HEX && j % 2 == 0)
                {
                    offset += new Vector3(tileSize/2,0,0);
                }
                GameObject newTile = Instantiate(_prefab,corner + offset,Quaternion.identity);
                if(gridShape == TileGridShape.HEX)
                {
                    Vector3 euler = newTile.transform.eulerAngles;
                    euler.x = -90;
                    newTile.transform.eulerAngles = euler;
                }
                tiles[i,j] = newTile.GetComponent<Tile>();
                newTile.GetComponent<Tile>().Init(new Vector2Int(i,j));
                newTile.GetComponent<Tile>().ResetColor();
                if(i == 0 || j == 0 || i == dimensions.x - 1 || j == dimensions.y - 1)
                {
                    newTile.GetComponent<Tile>().PaintTile(Tile.TileType.Edge);
                }
            }
        }
    }
    public TileGridShape GetTileGridShape(){return shape;}
    public Tile GetTile(Vector2Int _coords)
    {
        if(_coords.x < 0 || _coords.y < 0 || _coords.x >= dimensions.x || _coords.y >= dimensions.y){return null;}
        return tiles[_coords.x,_coords.y];
    }
    public Tile[,] GetTiles(){return tiles;}
    public List<Tile> GetUnsortedTiles()
    {
        List<Tile> temp = new List<Tile>();
        foreach(Tile tile in tiles)
        {
            temp.Add(tile);
        }
        return temp;
    }
    public List<Tile> GetTilesInRegion(Vector2Int _corner, Vector2Int _dimensions)
    {
        if(_dimensions.x >= dimensions.x || _dimensions.y >= dimensions.y){return null;}
        List<Tile> temp = new List<Tile>();
        for(int i = 0; i < _dimensions.x; i++){
            for(int j = 0; j < _dimensions.y; j++)
            {
                if(_corner.x + i < 0 || _corner.y + j < 0 || _corner.x + i >= dimensions.x || _corner.y + j >= dimensions.y){continue;}
                temp.Add(tiles[_corner.x + i, _corner.y + j]);
            }
        }
        return temp;
    }
    public List<Tile> GetTilesWithinDistance(Vector2Int _coords, float _maxDistance)
    {
        if(_coords.x < 0 || _coords.y < 0 || _coords.x >= dimensions.x || _coords.y >= dimensions.y){return null;}
        List<Tile> nearbyTiles = new List<Tile>();
        foreach(Tile tile in tiles)
        {
            //If one coord value is larger than the other, use that as the distance, otherwise the sum of the two values
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
        return nearbyTiles;
    }
    public void ClearAllRooms()
    {
        foreach(Tile tile in tiles)
        {
            if(tile.type != Tile.TileType.room && tile.type != Tile.TileType.Corridor){continue;}
            tile.ResetColor();
        }
        GridTerrainGenerator.instance.GenerateTerrain();
    }
    public void ResetGrid()
    {
        foreach(Tile tile in tiles)
        {
            if(tile.GetCoords().x == 0 || tile.GetCoords().y == 0 || tile.GetCoords().x == dimensions.x - 1 || tile.GetCoords().y == dimensions.y - 1)
            {
                tile.PaintTile(Tile.TileType.Edge);
            }
            else
            {
                tile.ResetColor();
            }
        }
    }
    public void RefreshTileColors()
    {
        foreach(Tile tile in tiles)
        {
            tile.PaintTile(tile.type,true);
        }
    }
    public Vector2Int GetDimensions(){return dimensions;}
}
