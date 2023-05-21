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
    private int roomCount = 5;
    private int seed = 0;
    private roomDistributionType type;
    private List<Tile> tiles = new List<Tile>();
    private List<Tile> roomCentres = new List<Tile>();
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
        roomCount = _count;
        Generate();
    }
    public void SetSeed(int _seed)
    {
        seed = _seed;
        Generate();
    }
    public List<Tile> GetRoomCentres(){return roomCentres;}
    private void Generate()
    {
        TileGrid.instance.ClearAllRooms();
        tiles = TileGrid.instance.GetUnsortedTiles();
        roomCentres.Clear();
        switch(type)
        {
            case roomDistributionType.RANDOM:
                int successfulRooms = 0;
                while(tiles.Count > 0 && successfulRooms != roomCount)
                {
                    Tile newRoomCentre = TryCreateRoomCentre(successfulRooms);
                    if(newRoomCentre != null)
                    {
                        roomCentres.Add(newRoomCentre);
                        successfulRooms++;
                    }

                }
                break;
            case roomDistributionType.ACCRETION:
                break;
            case roomDistributionType.BSP:
                PartitionAxes newAxis = new PartitionAxes(Vector2.up * 10, true);
                Partition p = new Partition(null,null,newAxis);
                List<Tile> partitionTiles = p.GetTilesInParitition();
                foreach(Tile tile in partitionTiles)
                {
                    tile.PaintTile(Tile.TileType.room);
                }
                break;
        }
    }
    private Tile TryCreateRoomCentre(int _currRoomCount)
    {
        Random.seed = seed;
        int index = Random.Range(0,tiles.Count);
        List<Tile> potentialRoom = tiles[index].GetNeighbours(true);
        potentialRoom.Add(tiles[index]);
        foreach(Tile tile in potentialRoom)
        {
            if(tile.type != Tile.TileType.blank)
            {
                RemoveTilesFromCache(potentialRoom);
                return null;
            }
        }
        // foreach(Tile tile in potentialRoom)
        // {
        //     // float a =_currRoomCount;
        //     // float b = roomCount;
        //     // Color col = Color.Lerp(Color.white,Color.yellow, a / b);
        //     // tile.PaintTile(Tile.TileType.room);
        // }
        tiles[index].PaintTile(Tile.TileType.room);
        return tiles[index];
    }
    private Partition GenerateBinarySpacialPartition()
    {
        return null;
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
public class Partition
{
    public Partition parentPartition = null;
    public List<Partition> childPartitions = new List<Partition>();
    public PartitionAxes partitionAxes;
    public Partition(Partition _parent, List<Partition> _children, PartitionAxes _partitionAxes)
    {
        parentPartition = _parent;
        childPartitions = _children;
        if(childPartitions != null)
        {
            if(childPartitions.Count > 0)
            {
                foreach(Partition p in childPartitions)
                {
                    p.parentPartition = this;
                }
            }
        }
        partitionAxes = _partitionAxes;
    }
    public List<Tile> GetTilesInParitition()
    {
        //Get all division axes from parent
        List<PartitionAxes> divisionAxes = new List<PartitionAxes>();
        divisionAxes.Add(partitionAxes);
        Partition currPartition = this;
        Partition parent = currPartition.parentPartition;
        while(parent != null)
        {
            divisionAxes.Add(parent.partitionAxes);
            parent = parent.parentPartition;
        }
        Tile[,] tiles = TileGrid.instance.GetTiles();
        List<Tile> validTiles = new List<Tile>();
        foreach(Tile tile in tiles)
        {
            bool isValid = true;
            foreach(PartitionAxes _axes in divisionAxes)
            {
                if(!TileWithinAxes(tile.GetCoords(),_axes.greaterThanAxis,_axes.axis)){isValid = false;}
            }
            if(isValid){validTiles.Add(tile);}
        }
        return validTiles;
    }
    private bool TileWithinAxes(Vector2Int _coords, bool _greaterThanAxis, Vector2 _axes)
    {
        int x = _coords.x;
        int y = _coords.y;
        if(_greaterThanAxis)
        {
            if(x < _axes.x || y < _axes.y){return false;}
            return true;
        }
        else
        {
            if(x > _axes.x || y > _axes.y){return false;}
            return true;
        }
    }
}
public struct PartitionAxes
{
    public bool greaterThanAxis;
    public Vector2 axis;
    public PartitionAxes(Vector2 _axis, bool _greaterThanAxis)
    {
        axis = _axis;
        greaterThanAxis = _greaterThanAxis;
    }
}
