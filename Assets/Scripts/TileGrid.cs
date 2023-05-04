using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public static TileGrid instance;
    private Tile[,] tiles;
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
    public void Initialise(Vector3 centre, Vector2Int dimensions, GameObject _prefab, float tileSize = 1)
    {
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
        for(int i = 0; i < dimensions.x; i++){
            for(int j = 0; j < dimensions.y; j++)
            {
                offset = new Vector3(j * tileSize, 0, i * tileSize);
                GameObject newTile = Instantiate(_prefab,corner + offset,Quaternion.identity);
                tiles[i,j] = newTile.GetComponent<Tile>();
            }
        }
    }
}
