using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
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
    //Rand
    private int rndSeed = 0;
    public float rndChance = 0.1f;
    //Perlin
    private int perlinSeed = 0;
    public float scale = 1;
    public float threshold = 0.3f;
    public Vector2 perlinOffset = new Vector2();

    //voronoi 
    private int voronoiSeed = 0;
    private float voronoiRadius = 1;
    private int voronoiDensity = 5;
    private Vector2 voronoiOffset = new Vector2();
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
        voronoiOffset = Vector2.zero;
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
            case TerrainType.VORONOI:
                ApplyCellularNoise();
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
        UnityEngine.Random.seed = rndSeed;
        foreach(Tile currTile in tiles)
        {
            float chance = UnityEngine.Random.Range(0,100);
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
    private void ApplyCellularNoise()
    {
        Tile[,] tiles = TileGrid.instance.GetTiles();
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        UnityEngine.Random.seed = voronoiSeed;
        List<Vector2> voronoiPoints = new List<Vector2>();
        for(int i = 0; i < voronoiDensity; i++)
        {
            float x = UnityEngine.Random.Range(0,dimensions.x);
            float y = UnityEngine.Random.Range(0,dimensions.y);
            voronoiPoints.Add(new Vector2(x,y));
        }
        for(int x = 0; x < dimensions.x; x++){
            for(int y = 0; y < dimensions.y; y++)
            {
                foreach(Vector2 point in voronoiPoints)
                {
                    float distance = Vector2.Distance(point, new Vector2(x + voronoiOffset.x,y + voronoiOffset.y));
                    if(distance <= voronoiRadius)
                    {
                        tiles[x,y].PaintTile(Color.red,Tile.TileType.terrain);
                        break;
                    }
                }
            }
        }
    }
    private void ResetTerrain()
    {
        TileGrid.instance.ResetGrid();
    }
    #region PerlinSetters
    public void SetPerlinSeed(int _seed)
    {
        perlinSeed = _seed;
        GenerateTerrain();
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
    #region RndSetters
    public void SetRandThreshold(float _val)
    {
        rndChance = _val;
        GenerateTerrain();
    }
    public void SetRandSeed(int _seed)
    {
        rndSeed = _seed;
        GenerateTerrain();
    }
    #endregion
    #region Voronoi Setters
    public void SetVoronoiSeed(int _seed)
    {   
        voronoiSeed = _seed;
        GenerateTerrain();
    }   
    public void SetVoronoiRadius(int _radius)
    {
        voronoiRadius = _radius;
        GenerateTerrain();
    }
    public void SetVoronoiOffsetX(int _x)
    {
        voronoiOffset.x = _x;
        GenerateTerrain();
    }
    public void SetVoronoiOffsetY(int _y)
    {
        voronoiOffset.y = _y;
        GenerateTerrain();
    }
    public void SetVoronoiDensity(int _density)
    {
        voronoiDensity = _density;
        GenerateTerrain();
    }
    #endregion
}
