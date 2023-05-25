using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShapeGenerator : MonoBehaviour
{
    public enum RoomShape
    {
        CIRCLES,
        SQUARES,
        CELLULAR_AUTOMATA,
        ALL
    }
    private RoomShape shape = RoomShape.CIRCLES;
    private List<Room> rooms = new List<Room>();
    public int roomSize = 1;
    public bool roomsCanGenerateIntoTerrain = true;
    public int corridorSeed = 0;
    public int randomSeed = 0;
    public static RoomShapeGenerator instance;
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
    public void Generate()
    {
        rooms = RoomDistributor.instance.GetRooms();
        if(rooms == null){return;}
        if(rooms.Count < 1){return;}
        TileGrid.instance.ClearAllRooms();
        Random.seed = randomSeed;
        switch(shape)
        {
            case RoomShape.SQUARES:
                foreach(Room room in rooms)
                {
                    GenerateSquareRoom(room);
                }
                break;
            case RoomShape.CIRCLES:
                foreach(Room room in rooms)
                {
                GenerateCircleRoom(room);
                }
                break;
            case RoomShape.ALL:
                foreach(Room room in rooms)
                {
                    int rnd = Random.Range(0,2);
                    if(rnd == 0){GenerateCircleRoom(room);}
                    else if(rnd == 1){GenerateSquareRoom(room);}
                }
                break;
        }
        GenerateAllCorridors();
    }
    private void GenerateCircleRoom(Room room)
    {
        List<Tile> temp = new List<Tile>();
        int circleRadius = 1 + (roomSize * 3);
        temp.Clear();
        room.rootTile.PaintTile(Tile.TileType.room);
        Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize;
        Vector2Int dimensions = circleRadius * Vector2Int.one;
        foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
        {
            if(!roomsCanGenerateIntoTerrain)
            {
                if(neighbour.type == Tile.TileType.hidden || neighbour.type == Tile.TileType.terrain){continue;}
            }
            if(Vector3.Distance(neighbour.transform.position, room.rootTile.transform.position) > roomSize){continue;}
            neighbour.PaintTile(Tile.TileType.room);
            temp.Add(neighbour);
        }
    }
    private void GenerateSquareRoom(Room room)
    {
        List<Tile> temp = new List<Tile>();
        int radius = 1 + (roomSize * 2);
        temp.Clear();
        room.rootTile.PaintTile(Tile.TileType.room);
        Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize;
        Vector2Int dimensions = radius * Vector2Int.one;
        foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
        {
            if(!roomsCanGenerateIntoTerrain)
            {
                if(neighbour.type == Tile.TileType.hidden || neighbour.type == Tile.TileType.terrain){continue;}
            }
            neighbour.PaintTile(Tile.TileType.room);
            temp.Add(neighbour);
        }
    }
    private void GenerateAllCorridors()
    {
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
            //if(currTile.type == Tile.TileType.Corridor){return false;}
            if(currTile.type == Tile.TileType.room){continue;}
            currTile.PaintTile(Tile.TileType.Corridor);
        }
        return true;
    }
    public void SetRoomShape(int _RoomShapeIndex)
    {
        shape = (RoomShape)_RoomShapeIndex;
        Generate();
    }
    public void SetRoomSize(int _size){
        roomSize = _size;
        Generate();
    }
    public void SetRoomsCanGenerateOverTerrain(bool _canGenerateOverTerrain)
    {
        roomsCanGenerateIntoTerrain = _canGenerateOverTerrain;
        Generate();
    }
    public void SetCorridorSeed(int _seed)
    {
        corridorSeed = _seed;
        Generate();
    }
    public void SetRandomSeed(int _seed)
    {
        randomSeed = _seed;
        Generate();
    }
}
