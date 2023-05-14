using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTerrainGenerator : MonoBehaviour
{
    public enum TerrainType
    {
        PERLIN,
        VORONOI,
        RANDOM
    }
    public TerrainType terrainType = TerrainType.RANDOM;
    public float rndChance = 0.1f;
    [Header("Perlin noise")]
    public float frequency = 1;
    public float scale = 1;
    public float threshold = 0.3f;
    public static GridTerrainGenerator instance;
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
    }
    public void GenerateTerrain()
    {
        ResetTerrain();
        switch(terrainType)
        {
            case TerrainType.RANDOM:
                ApplyRandNoise();
                break;
            case TerrainType.PERLIN:
                ApplyPerlinNoise();
                break;
            default:
                break;
        }
    }
    private void ApplyRandNoise()
    {
        Tile[,] tiles = TileGrid.instance.GetTiles();
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        foreach(Tile currTile in tiles)
        {
            float chance = Random.Range(0,100);
            if(chance <= rndChance * 100)
            {
                currTile.PaintTile(Color.red, Tile.TileType.terrain);
            }
        }
    }
    private void ApplyPerlinNoise()
    {
        Tile[,] tiles = TileGrid.instance.GetTiles();
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        foreach(Tile currTile in tiles)
        {
            Vector2 coords = currTile.GetCoords();
            float x,y;
            x = coords.x / scale * frequency;
            y = coords.y / scale * frequency;
            float val = Mathf.PerlinNoise(x,y);
            if(val <= threshold)
            {
                currTile.PaintTile(Color.red,Tile.TileType.terrain);
            }
        }
    }
    private void ResetTerrain()
    {
        Tile[,] tiles = TileGrid.instance.GetTiles();
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        foreach(Tile currTile in tiles)
        {
            currTile.ResetColor();
        }
    }
}
