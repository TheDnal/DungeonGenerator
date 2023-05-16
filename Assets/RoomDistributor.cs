using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDistributor : MonoBehaviour
{
    public enum roomDistributionType
    {
        RANDOM,
        ACCRETION,
        BSP
    }
    public static RoomDistributor instance;
    private int roomCount;
    private roomDistributionType type;
    private List<Tile> tiles = new List<Tile>();
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
    public void SetDistributionType(int _typeIndex)
    {
        type = (roomDistributionType)_typeIndex;
        Generate();
    }
    public void SetRoomCount(int _count)
    {
        Debug.Log(_count);
        roomCount = _count;
        Generate();
    }


    private void Generate()
    {
        TileGrid.instance.ClearAllRooms();
        tiles = TileGrid.instance.GetUnsortedTiles();
        switch(type)
        {
            case roomDistributionType.RANDOM:
                int successfulRooms = 0;
                while(tiles.Count > 0 && successfulRooms != roomCount)
                {
                    Room newRoom = TryCreateRoom();
                    if(newRoom != null){successfulRooms++;}
                }
                break;
            case roomDistributionType.ACCRETION:
                break;
            case roomDistributionType.BSP:
                break;
        }
    }
    private Room TryCreateRoom()
    {
        int index = Random.Range(0,tiles.Count);
        List<Tile> potentialRoom = tiles[index].GetNeighbours(true);
        potentialRoom.Add(tiles[index]);
        foreach(Tile tile in potentialRoom)
        {
            if(tile.type == Tile.TileType.terrain || tile.type == Tile.TileType.room || tile.type == Tile.TileType.Edge)
            {
                RemoveTilesFromCache(potentialRoom);
                return null;
            }
        }
        foreach(Tile tile in potentialRoom)
        {
            tile.PaintTile(Color.grey,Tile.TileType.room);
        }
        Room newRoom = new Room(potentialRoom);
        return newRoom;
    }
    private void RemoveTilesFromCache(List<Tile> _tilesToRemove)
    {
        foreach(Tile tile in _tilesToRemove)
        {
            if(!tiles.Contains(tile)){continue;}
            tiles.Remove(tile);
        }
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