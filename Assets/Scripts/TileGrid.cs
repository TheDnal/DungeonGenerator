using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public static TileGrid instance;
    private Tile[,] tiles;
    private Vector2Int dimensions;
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
                tiles[i,j] = newTile.GetComponent<Tile>();
                newTile.GetComponent<Tile>().Init(new Vector2Int(i,j));
            }
        }
    }
    public TileGridShape GetTileGridShape(){return shape;}
    public Tile GetTile(Vector2Int _coords)
    {
        if(_coords.x < 0 || _coords.y < 0 || _coords.x > dimensions.x || _coords.y > dimensions.y){return null;}
        return tiles[_coords.x,_coords.y];
    }
}
