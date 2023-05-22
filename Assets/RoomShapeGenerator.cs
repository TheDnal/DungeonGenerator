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
    public int corridorSeed = 0;
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
        List<Tile> temp = new List<Tile>();
        switch(shape)
        {
            case RoomShape.SQUARES:
                int radius = 1 + (roomSize * 2);
                foreach(Room room in rooms)
                {
                    temp.Clear();
                    room.rootTile.PaintTile(Tile.TileType.room);
                    Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize;
                    Vector2Int dimensions = radius * Vector2Int.one;
                    foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
                    {
                        neighbour.PaintTile(Tile.TileType.room);
                        temp.Add(neighbour);
                    }
                }
                GenerateCorridors();
                break;
            case RoomShape.CIRCLES:
                int circleRadius = 1 + (roomSize * 3);
                foreach(Room room in rooms)
                {
                    temp.Clear();
                    room.rootTile.PaintTile(Tile.TileType.room);
                    Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize;
                    Vector2Int dimensions = circleRadius * Vector2Int.one;
                    foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
                    {
                        if(Vector3.Distance(neighbour.transform.position, room.rootTile.transform.position) > roomSize){continue;}
                        neighbour.PaintTile(Tile.TileType.room);
                        temp.Add(neighbour);
                    }
                }
                break;
        }
    }
    private void GenerateCorridors()
    {
        if(rooms == null){return;}
        if(rooms.Count < 1){return;}
        //Corridors are L shape, meaning they're a straight vertical line and horizontal line

        //Iterate throughout each room
        //Check if it has a room connection
        //Randomly decide whether to generate the corridor horizontally or vertically
        Random.seed = corridorSeed;
        int height = 0,width = 0;
        bool heightFirst;
        foreach(Room room in rooms)
        {
            if(room.GetConnectedRooms().Count < 1){continue;}
            foreach(Room _connectedRoom in room.GetConnectedRooms())
            {
                height = _connectedRoom.rootTile.GetCoords().y - 
                         room.rootTile.GetCoords().y;
                width = _connectedRoom.rootTile.GetCoords().x -
                        room.rootTile.GetCoords().x;
                heightFirst = Random.Range(0,2) == 0 ? true : false;
                Room leftMostRoom = room.rootTile.GetCoords().x < _connectedRoom.rootTile.GetCoords().x ?  room : _connectedRoom;
                DrawHorizontalCorridor(leftMostRoom.rootTile.GetCoords(),Mathf.Abs(width));
                Room bottomMostRoom = room.rootTile.GetCoords().y < _connectedRoom.rootTile.GetCoords().y ?  room : _connectedRoom;
                DrawVerticalCorridor(bottomMostRoom.rootTile.GetCoords(),Mathf.Abs(height));
            }
        }
    }
    private void DrawHorizontalCorridor(Vector2Int _startPos, int _length)
    {
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        for(int i =0; i <= _length; i++)
        {
            if(_startPos.x + i < 0 || _startPos.x + i >= dimensions.x){continue;}
            Tile currTile = TileGrid.instance.GetTile(_startPos + Vector2Int.right * i);
            if(currTile.type == Tile.TileType.Corridor || currTile.type == Tile.TileType.room){continue;}
            currTile.PaintTile(Tile.TileType.Corridor);
        }   
    }
    private void DrawVerticalCorridor(Vector2Int _startPos, int _length)
    {
        Vector2Int dimensions = TileGrid.instance.GetDimensions();
        for(int i =0; i <= _length; i++)
        {
            if(_startPos.y + i < 0 || _startPos.y + i >= dimensions.y){continue;}
            Tile currTile = TileGrid.instance.GetTile(_startPos + Vector2Int.up * i);
            //if(currTile.type == Tile.TileType.Corridor || currTile.type == Tile.TileType.room){continue;}
            currTile.PaintTile(Tile.TileType.Corridor);
        } 
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
}
