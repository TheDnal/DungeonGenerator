using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetailsGenerator : MonoBehaviour
{
    public static RoomDetailsGenerator instance;
    private bool corridorsEnabled = false;
    private bool corridorsOverlap = false;
    private bool wallsEnabled = false;
    private bool erosionEnabled = false;
    private bool wallsGenerateThroughTerrain = false;
    private bool corridorsHaveWalls = false;
    private float wallErosion = 0;
    private int corridorSeed = 0;
    private float erosionVal = .05f;
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
    public void SetCorridorsEnabled(bool _enabled)
    {
        corridorsEnabled = _enabled;
        Generate();
    }
    public void SetCorrdorsOverlap(bool _enabled)
    {
        corridorsOverlap = _enabled;
        Generate();
    }
    public void SetCorridorSeed(int _seed)
    {
        corridorSeed = _seed;
        Generate();
    }
    public void SetWallsEnabled(bool _enabled)
    {
        wallsEnabled = _enabled;
        Generate();
    }
    public void SetErosionEnabled(bool _enabled)
    {
        erosionEnabled = _enabled;
        Generate();
    }
    public void SetErosionValue(float _val)
    {
        erosionVal = _val;
        Generate();
    }
    public void SetCorridorsHaveWalls(bool _haveWalls)
    {
        corridorsHaveWalls = _haveWalls;
        Generate();
    }
    public void SetWallsGenThroughTerrainEnabled(bool _enabled)
    {
        wallsGenerateThroughTerrain = _enabled;
        Generate();
    }
    public void Generate()
    {
        RoomShapeGenerator.instance.Generate();
        if(corridorsEnabled){GenerateAllCorridors();}
        if(wallsEnabled){GenerateWalls();}
        if(erosionEnabled){GenerateErosion();}
    }
    #region Corridor Generation
    private void GenerateAllCorridors()
    {
        List<Room> rooms = RoomShapeGenerator.instance.GetRooms();
        if(rooms == null){return;}
        if(rooms.Count < 1){return;}
        //Corridors are L shape, meaning they're a straight vertical line and horizontal line

        //Iterate throughout each room
        //Check if it has a room connection
        //Randomly decide whether to generate the corridor horizontally or vertically
        int height = 0,width = 0;
        Random.seed = corridorSeed;
        foreach(Room room in rooms)
        {
            if(room.GetConnectedRooms().Count == 0){continue;}
            foreach(Room _connectedRoom in room.GetConnectedRooms())
            {
                height = _connectedRoom.rootTile.GetCoords().y - 
                         room.rootTile.GetCoords().y;
                width = _connectedRoom.rootTile.GetCoords().x -
                        room.rootTile.GetCoords().x;
                bool heightFirst = Random.Range(0,10) > 5 ? true : false;
                GenerateCorridor(room.rootTile.GetCoords(),_connectedRoom.rootTile.GetCoords(), heightFirst);
            }
        }
    }
    private void GenerateCorridor(Vector2Int _startCoord, Vector2Int _endCoord, bool heightFirst)
    {
        //Random check for whether to generate the corridor horizontally or vertically first
        //Get the lengths of the corridors
        int height = _endCoord.y - _startCoord.y;
        int width = _endCoord.x - _startCoord.x;
        if(heightFirst) //Generate the vertical corridor first
        {
            if(height != 0)
            {
                int magnitude = height > 0 ? 1 : -1;
                Vector2Int direction = new Vector2Int(0,magnitude);
                if(!DrawCorridorLine(_startCoord,Mathf.Abs(height),direction)){return;}
            }
            if(width != 0)
            {
                int magnitude = width > 0 ? 1 : -1;
                Vector2Int direction = new Vector2Int(magnitude,0);
                DrawCorridorLine(_startCoord + new Vector2Int(0,height),Mathf.Abs(width),direction);
            }
        }
        else    //Generate the horizontal corridor first
        {
            if(width != 0)
            {
                int magnitude = width > 0 ? 1 : -1;
                Vector2Int direction = new Vector2Int(magnitude,0);
                if(!DrawCorridorLine(_startCoord,Mathf.Abs(width),direction)){return;}
            }
            if(height != 0)
            {
                int magnitude = height > 0 ? 1 : -1;
                Vector2Int direction = new Vector2Int(0,magnitude);
                DrawCorridorLine(_startCoord + new Vector2Int(width,0),Mathf.Abs(height),direction);
            }
        }
    }
    private bool DrawCorridorLine(Vector2Int _startCoord, int _length, Vector2Int _direction)
    {
        //Return false bool if corridor connects to another corridor, preventing second part of the corridor to generate 
        if(_length == 0){return true;}
        //Iterate along the direction for length steps, starting from the start coord. 
        Vector2Int currCoord = _startCoord;
        for(int i = 0; i < _length; i ++)
        {
            currCoord = _startCoord + (i * _direction);
            Tile currTile = TileGrid.instance.GetTile(currCoord);
            if(currTile == null){continue;}
            if(currTile.type == Tile.TileType.Corridor && !corridorsOverlap){return false;}
            if(currTile.type == Tile.TileType.room){continue;}
            currTile.PaintTile(Tile.TileType.Corridor);
        }
        return true;
    }
    #endregion
    #region Wall Generation
    private void GenerateWalls()
    {
        List<Tile> wallTiles = new List<Tile>();
        int x = 0;
        foreach(Tile tile in TileGrid.instance.GetUnsortedTiles())
        {
            if(corridorsHaveWalls)
            {
                if(tile.type != Tile.TileType.room && tile.type != Tile.TileType.Corridor){continue;}
            }
            else
            {
                if(tile.type != Tile.TileType.room){continue;}
            }
            foreach(Tile neighbourTile in tile.GetNeighbours(wallsGenerateThroughTerrain))
            {
                if(neighbourTile.type == Tile.TileType.room || neighbourTile.type == Tile.TileType.Corridor){continue;}
                wallTiles.Add(neighbourTile);
            }
        }
        foreach(Tile wallTile in wallTiles)
        {
            wallTile.PaintTile(Tile.TileType.Wall);
        }
    }
    #endregion
    #region Erosion Generation
    private void GenerateErosion()
    {
        List<Tile> erodedRoomTiles = new List<Tile>();
        List<Tile> erodedCorridorTiles = new List<Tile>();
        foreach(Tile tile in TileGrid.instance.GetTiles())
        {
            if(tile.type != Tile.TileType.room && tile.type != Tile.TileType.Corridor){continue;}
            foreach(Tile neighbour in tile.GetNeighbours(true))
            {
                if(neighbour.type == Tile.TileType.room || neighbour.type == Tile.TileType.Corridor || neighbour.type == Tile.TileType.Edge){continue;}
                if(Random.Range(0,10) >= erosionVal * 10){continue;}
                if(erodedRoomTiles.Contains(neighbour)){continue;}
                if(erodedCorridorTiles.Contains(neighbour)){continue;}
                if(tile.type == Tile.TileType.room){erodedRoomTiles.Add(neighbour);}
                else{erodedCorridorTiles.Add(neighbour);}
            }
        }
        foreach(Tile tile in erodedRoomTiles)
        {
            tile.PaintTile(Tile.TileType.room);
        }
        foreach(Tile tile in erodedCorridorTiles)
        {
            tile.PaintTile(Tile.TileType.Corridor);
        }
    }
    #endregion
}
