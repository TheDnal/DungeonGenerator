using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTerrainGenerator : MonoBehaviour
{
    public enum TerrainType
    {
        PERLIN,
        VORONOI,
        RANDOM,
        NONE
    }
    public TerrainType terrainType = TerrainType.RANDOM;
    public float rndChance = 0.1f;
    [Header("Perlin noise")]
    public float frequency = 1;
    public float scale = 1;
    public float threshold = 0.3f;
    public Vector2 perlinOffset = new Vector2();
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
        perlinOffset = Vector2.zero;
        instance = this;
    }
    public void SetTerrainType(int _index)
    {
        terrainType = (TerrainType)_index;
        GenerateTerrain();
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
            case TerrainType.NONE:
                ResetTerrain();
                break;
            default:
                ResetTerrain(); 
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
        if(scale <= 0)
        {
            scale = 0.01f;
        }
        for(int y = 0; y < dimensions.y; y++){
            for(int x = 0; x < dimensions.x; x++)
            {
                float sampleX = (x + perlinOffset.x) / scale;
                float sampleY = (y + perlinOffset.y) / scale;
                float val = Mathf.PerlinNoise(sampleX,sampleY);
                if(val <= threshold)
                {
                    tiles[x,y].PaintTile(Color.red,Tile.TileType.terrain);
                }
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
    #region PerlinSetters
    public void SetPerlinFrequency(float _frequency)
    {
        //frequency = _frequency;
        //GenerateTerrain();
    }
    public void SetPerlinScale(float _scale)
    {
        scale = _scale;
        GenerateTerrain();
    }
    public void SetPerlinThreshold(float _val)
    {
        threshold = _val;
        GenerateTerrain();
    }
    public void SetPerlinOffsetX(float _val)
    {
        perlinOffset.x = _val;
        GenerateTerrain();    
    }
    public void SetPerlinOffsetY(float _val)
    {
        perlinOffset.y = _val;
        GenerateTerrain();    
    }
    #endregion

    public void SetRandThreshold(float _val)
    {
        rndChance = _val;
        GenerateTerrain();
    }

}
