using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShapeGenerator : MonoBehaviour
{
    public enum RoomShape
    {
        SQUARES,
        CIRCLES,
        CELLULAR_AUTOMATA,
        ALL
    }
    private RoomShape shape = RoomShape.CIRCLES;
    private List<Tile> roomCentres = new List<Tile>();
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
        roomCentres = RoomDistributor.instance.GetRoomCentres();
        if(roomCentres == null){return;}
        if(roomCentres.Count < 1){return;}
        TileGrid.instance.ClearAllRooms();
        foreach(Tile roomCentre in roomCentres)
        {
            roomCentre.PaintTile(Tile.TileType.room);
            foreach(Tile neighbour in roomCentre.GetNeighbours())
            {
                neighbour.PaintTile(Tile.TileType.room);
            }
        }
    }
    public void SetRoomShape(int _RoomShapeIndex)
    {
        shape = (RoomShape)_RoomShapeIndex;
        Generate();
    }
}
public class Room
{
    public List<Tile> tiles = new List<Tile>();
    private List<Room> connectedRooms = new List<Room>();
    public Room(List<Tile> _tiles)
    {
        tiles = _tiles;
    }
    public void ConnectRoom(Room _room)
    {
        if(connectedRooms.Contains(_room)){return;}
        connectedRooms.Add(_room);
    }
    public void DisconnectRoom(Room _room)
    {
        if(!connectedRooms.Contains(_room)){return;}
        connectedRooms.Remove(_room);
    }
}
