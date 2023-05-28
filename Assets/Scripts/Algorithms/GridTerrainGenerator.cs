using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class GridTerrainGenerator : MonoBehaviour
{
    /*
        Class that controls the generation of terrain on the grid
    */
    public enum TerrainType //The type of terrain
    {
        PERLIN,
        VORONOI,
        RANDOM,
        NONE
    }
    public TerrainType terrainType = TerrainType.RANDOM; //the current terrain
    //Rand
    private int rndSeed = 0; //Seed for the random generation
    public float rndChance = 0.1f; //The chance for a tile to become terrain in random terrain
    //Perlin
    private int perlinSeed = 0; //The seed for perlin generation
    public float scale = 25; //The scale of the perlin noise
    public float threshold = 0.3f; //The threshold perin noise has to be below for the tile to become terrain
    public Vector2 perlinOffset = new Vector2(); //Offset of the noise

    //voronoi 
    private int voronoiSeed = 0; //Seed of the voronoi noise
    private float voronoiRadius = 1; //Radius of the voronoi cells
    private int voronoiDensity = 5; //Density of the voronoi cells
    private Vector2 voronoiOffset = new Vector2(); //Offset of the voronoi cells
    public static GridTerrainGenerator instance; //Singleong
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
    public void SetTerrainType(int _index) //Set terrain type
    {
        terrainType = (TerrainType)_index;
        GenerateTerrain();
    }
    public void GenerateTerrain() //Generate the currently selected terrain
    {
        ResetTerrain(); //reset all terrain
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
                ApplyVoronoiNoise();
                break;
            default:
                ResetTerrain(); 
                break;
        }
    }
    private void ApplyRandNoise() //Apply terrain randomly based on random.range
    {
        Tile[,] tiles = TileGrid.instance.GetTiles(); //Get all tiles
        UnityEngine.Random.seed = rndSeed; //set seed
        foreach(Tile currTile in tiles) //iterate through the grid
        {
            //Apply the random noise if a random value of 0-100 is below the rndChance
            float chance = UnityEngine.Random.Range(0,100);
            if(chance <= rndChance * 100)
            {
                currTile.PaintTile(Tile.TileType.terrain); //apply terrain
            }
        }
    }
    private void ApplyPerlinNoise() //Applies perlin noise to the grid
    {
        Tile[,] tiles = TileGrid.instance.GetTiles(); //Get all tiles
        Vector2Int dimensions = TileGrid.instance.GetDimensions(); // Get the dimensions
        if(scale <= 0) //Clamp the scale
        {
            scale = 0.01f;
        }
        for(int y = 0; y < dimensions.y; y++){
            for(int x = 0; x < dimensions.x; x++) //Iterate through each tile on the grid
            {
                float sampleX = (x + perlinOffset.x) / scale; //Perlin X value
                float sampleY = (y + perlinOffset.y) / scale; //Perlin Y value
                float val = Mathf.PerlinNoise(sampleX,sampleY); //Value of perlin noise at the cooridante
                if(val <= threshold) //If below the threshold, apply perlin noise
                {
                    tiles[x,y].PaintTile(Tile.TileType.terrain);
                }
            }
        }
    }
    private void ApplyVoronoiNoise() //Apply Voronoi noise to the grid
    {
        Tile[,] tiles = TileGrid.instance.GetTiles(); //Get all tiles
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        UnityEngine.Random.seed = voronoiSeed; //Set seed
        List<Vector2> voronoiPoints = new List<Vector2>(); //Cache all Voronoi cells
        for(int i = 0; i < voronoiDensity; i++) //Randomly distribute cells on the grid
        {
            float x = UnityEngine.Random.Range(0,dimensions.x);
            float y = UnityEngine.Random.Range(0,dimensions.y);
            voronoiPoints.Add(new Vector2(x,y));
        }
        for(int x = 0; x < dimensions.x; x++){
            for(int y = 0; y < dimensions.y; y++) //Iterate through each tile on the grid
            {
                foreach(Vector2 point in voronoiPoints) //If time within distance of any voronoi cell, become terrain
                {
                    float distance = Vector2.Distance(point, new Vector2(x + voronoiOffset.x,y + voronoiOffset.y));
                    if(distance <= voronoiRadius)
                    {
                        tiles[x,y].PaintTile(Tile.TileType.terrain);
                        break;
                    }
                }
            }
        }
    }
    private void ResetTerrain() //Reset all terrain
    {
        TileGrid.instance.ResetGrid();
    }
    #region PerlinSetters
    //Set all the values for Perlin Noise generation
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
    //Sets all the values for rand noise
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
    //sets all the values for voronoi noise
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
