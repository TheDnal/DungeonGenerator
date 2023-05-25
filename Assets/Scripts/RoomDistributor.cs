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
    private int accretionMaxRange = 1,accretionMinRange = 5;
    public int minPartitionWidth = 3, minPartitionHeight = 3;
    public int minPartitionSize = 9;
    public int maxPartitions = 8;
    private roomDistributionType type;
    private List<Tile> tiles = new List<Tile>();
    private List<Tile> roomCentres = new List<Tile>();
    private List<Room> rooms = new List<Room>();
    private List<Partition> divisiblePartitions = new List<Partition>();
    private List<Partition> partitions = new List<Partition>();
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
    #region Setters
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
    public void SetAccretionMaxRange(int _maxRange)
    {
        accretionMaxRange = _maxRange;
        Generate();
    }
    public void SetAccretionMinRange(int _minRange)
    {
        accretionMinRange = _minRange;
        Generate();
    }
    #endregion
    public List<Room> GetRooms(){return rooms;}
    private void Generate()
    {
        TileGrid.instance.ClearAllRooms();
        roomCentres.Clear();
        rooms.Clear();
        tiles.Clear();
        int iter = 0;
        switch(type)
        {
            case roomDistributionType.RANDOM:
                int successfulRooms = 0;
                tiles = TileGrid.instance.GetUnsortedTiles();
                iter = 0;
                while(tiles.Count > 0 && rooms.Count != roomCount && iter < 100)
                {
                    iter ++;
                    Tile newRoomCentre = TryCreateRoomCentre(successfulRooms, tiles);
                    if(newRoomCentre != null)
                    {
                        roomCentres.Add(newRoomCentre);
                        Room newRoom = new Room(newRoomCentre);
                        rooms.Add(newRoom);
                        if(successfulRooms > 0)
                        {
                            newRoom.ConnectRoom(rooms[successfulRooms - 1]);
                        }
                        successfulRooms++;
                    }
                }
                if(iter >= 100){Debug.Log("Warning: Potential infinite loop");}
                break;
            case roomDistributionType.ACCRETION:
                tiles = TileGrid.instance.GetUnsortedTiles();
                GenerateAccretion(roomCount);
                break;
            case roomDistributionType.BSP:
                partitions.Clear();
                divisiblePartitions.Clear();
                GenerateBinarySpacialPartition();   
                foreach(Partition p in partitions)
                {
                    if(p.childPartitions.Count > 0){continue;}
                    Color col = Random.ColorHSV();
                    foreach(Tile tile in p.GetTiles())
                    {
                        if(tile.type != Tile.TileType.blank){continue;}
                        tile.TempSetColor(col * 0.25f);
                    }
                    Tile centre = GetTileOfTypeInPartition(p,Tile.TileType.blank);
                    tiles = p.GetTiles();
                    if(centre != null)
                    {
                        centre.PaintTile(Tile.TileType.room);
                    }
                    else
                    {
                        centre = GetPartitionCentreTile(p);
                    }
                    centre.PaintTile(Tile.TileType.room);
                    Room newRoom = new Room(centre);
                    rooms.Add(newRoom);
                    p.room = newRoom;
                    //Get centre of BSP
                }
                ConnectBSPRooms();
                break;
        }
    }
    
    private Tile TryCreateRoomCentre(int _currRoomCount, List<Tile> _sample)
    {
        Random.seed = seed;
        int index = Random.Range(0,_sample.Count);
        List<Tile> potentialRoom = _sample[index].GetNeighbours(true);
        potentialRoom.Add(_sample[index]);
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
        _sample[index].PaintTile(Tile.TileType.room);
        return _sample[index];
    }
    
    #region BSP's
    private void GenerateBinarySpacialPartition()
    {
        //1 - Add all Tiles to starting bsp
        //2 - Add bsp to list of divisible bsp's
        //3 - If divisible list is empty or max bsp's exist, got to 9
        //4 - Iterate through divisible list, select biggest bsp
        //5 - Pick randomly whether to divide horizontally or vertically
        //6 - Division is the midpoint of the bsp
        //7 - If division results in sizes too small, try other division (go back to 5), if 
        // both fail then remove from divisible list (go to 3)
        //8 - If successfully divied, create two new bsp's from the division, set the parent bsp's children to 
        // the two new created, make sure the "greater than" bsp is the first child. Also set childrens parent
        // to the parent bsp. Set the childrens generation to the parent + 1. Go to 3
        //9 - For each last generation bsp, Find a valid spot to make its room
        //10 - Foreach Last generation bsp, make one connection between each sibling
        //11 - Move up a generation and repeat step 10. To find the rooms to connect, travel down the generations 
        // with the first of each generation, until you get a room
        //Repeat 11 until you reach the first generation.

        //1 - Add all Tiles to starting bsp
        Partition originPartition = new Partition(null,null,TileGrid.instance.GetUnsortedTiles(), 0);
        Random.seed = seed;

        //2 - Add bsp to list of divisible bsp's
        divisiblePartitions.Add(originPartition);
        partitions.Add(originPartition);
        int iter = 0; //Infintie loop breaker
        //3 - If divisible list is empty or max bsp's exist, got to 9
        while(divisiblePartitions.Count > 0 && partitions.Count < maxPartitions && iter < 100)
        {
            iter ++;
            //4 - Iterate through divisible list, select biggest bsp
            Partition partitionToDivide = GetLargestPartition(divisiblePartitions);
            divisiblePartitions.Remove(partitionToDivide);

            List<Partition> childPartitions = AttemptSplitPartiton(partitionToDivide);
            if(childPartitions == null)
            {
                continue;
            }
            Debug.Log(childPartitions.Count);
            partitionToDivide.childPartitions = childPartitions;
            foreach(Partition p in childPartitions)
            {
                divisiblePartitions.Add(p);
                partitions.Add(p);
            }
        }
        if(iter >= 100){Debug.Log("Warning : Potential infinite loop");}
    }
    private Partition GetLargestPartition(List<Partition> _sample)
    {
        if(_sample.Count == 0)
        {
            Debug.Log("Couldn't find largest parititon");
            return null;
        }
        int currSize = 0,largestSize = 0;
        Partition biggestPartition = null;
        foreach(Partition p in _sample)
        {
            currSize = p.GetTiles().Count;
            if(p.childPartitions.Count > 0){continue;}
            if(currSize > largestSize)
            {
                largestSize = currSize;
                biggestPartition = p;
            }
        }
        return biggestPartition;
    }
    private List<Partition> AttemptSplitPartiton(Partition _partitionToSplit)
    {
        //Get tile dimensions
        int lowestX = 0,highestX = 0,lowestY = 0,highestY = 0;
        foreach(Tile tile in _partitionToSplit.GetTiles())
        {
            if(tile.GetCoords().x < lowestX)
            {
                lowestX = tile.GetCoords().x;
            }
            else if(tile.GetCoords().x > highestX)
            {
                highestX = tile.GetCoords().x;
            }

            if(tile.GetCoords().y < lowestY)
            {
                lowestY = tile.GetCoords().y;
            }
            else if(tile.GetCoords().y > highestY)
            {
                highestY = tile.GetCoords().y;
            }
        }
        //Return if too small to divide
        int width = highestX - lowestX;
        int height = highestY - lowestY;
        if(width < minPartitionWidth * 2 && height < minPartitionHeight * 2){return null;}
        bool verticalSplitFirst = Random.Range(0,10) > 5 ? true : false;
        Partition firstChild, secondChild;
        List<Tile> tilesGreaterThan = new List<Tile>(),tilesLessThan = new List<Tile>();
        float rndSplitPercentage = 2;//Random.Range(1.5f,3);
        if(verticalSplitFirst)
        {
            //Vertical split
            if(width > minPartitionWidth * 2)
            {
                foreach(Tile tile in _partitionToSplit.GetTiles())
                {
                    if(tile.GetCoords().x >= lowestX + (width / rndSplitPercentage))
                    {
                        tilesGreaterThan.Add(tile);
                    }
                    else
                    {
                        tilesLessThan.Add(tile);
                    }
                }
            }
        }
        else
        {
            //Horizontal Split
            if(height > minPartitionHeight * 2)
            {
                foreach(Tile tile in _partitionToSplit.GetTiles())
                {
                    if(tile.GetCoords().y >= lowestY + (height / rndSplitPercentage))
                    {
                        tilesGreaterThan.Add(tile);
                    }
                    else
                    {
                        tilesLessThan.Add(tile);
                    }
                }
            }
        }
        if(tilesGreaterThan.Count == 0 || tilesLessThan.Count == 0){return null;}
        firstChild = new Partition(_partitionToSplit,null,tilesGreaterThan,_partitionToSplit.generation + 1);
        secondChild = new Partition(_partitionToSplit,null,tilesLessThan,_partitionToSplit.generation + 1);
        List<Partition> temp = new List<Partition>();
        temp.Add(firstChild);
        temp.Add(secondChild);
        return temp;
    }
    private Tile GetPartitionCentreTile(Partition _partition)
    {
        int lowestX = 999,highestX = -999,lowestY = 999,highestY = -999;
        foreach(Tile tile in _partition.GetTiles())
        {
            if(tile.GetCoords().x < lowestX)
            {
                lowestX = tile.GetCoords().x;
            }
            else if(tile.GetCoords().x > highestX)
            {
                highestX = tile.GetCoords().x;
            }

            if(tile.GetCoords().y < lowestY)
            {
                lowestY = tile.GetCoords().y;
            }
            else if(tile.GetCoords().y > highestY)
            {
                highestY = tile.GetCoords().y;
            }
        }
        int width = highestX - lowestX, height = highestY - lowestY;
        Vector2Int coord = new Vector2Int(lowestX + width /2, lowestY + height /2);
        return TileGrid.instance.GetTile(coord);
    }
    private void ConnectBSPRooms()
    {
        foreach(Partition currPartition in partitions)
        {
            //if(currPartition.generation != 0){continue;}
            Room roomA,roomB;
            //At the bottom of the tree, connect to sibling
            if(currPartition.childPartitions.Count == 0){continue;}
            roomA = GetPartitionRootRoom(currPartition.childPartitions[0]);
            roomB = GetPartitionRootRoom(currPartition.childPartitions[1]);
            if(roomA == null || roomB == null){Debug.Log("unable to find root room"); continue;}
            //Connect children
            roomA.ConnectRoom(roomB);
        }
    }
    private Room GetPartitionRootRoom(Partition _partition){
        bool bottomOfTree = false;
        Partition _currPartition = _partition;
        while(!bottomOfTree)
        {
            //Not bottom of tree
            if(_currPartition.childPartitions.Count != 0)
            {
                _currPartition = _currPartition.childPartitions[0];
                continue;
            }
            bottomOfTree = true;
        }
        return _currPartition.room;
    }
    private Tile GetTileOfTypeInPartition(Partition _sample, Tile.TileType _targetType)
    {
        List<Tile> validTiles = new List<Tile>();
        foreach(Tile currTile in _sample.GetTiles())
        {
            if(currTile.type != _targetType){continue;}
            validTiles.Add(currTile);
        }
        if(validTiles.Count == 0){Debug.Log("Couldn't find valid tile to make a room"); return null;}
        int index = Random.Range(0,validTiles.Count);
        return validTiles[index];
    }
    #endregion
    #region RoomAccretion
    private void GenerateAccretion(int _MaxRoomCount, int _maxTriesPerRoom = 5)
    {
        //Room accretion is the method by which new rooms are generated by "offshooting" them from its old rooms.
        //This process is repeated until no rooms have space to generate offshoots or the number of desired rooms
        //has been reached
        //This will be started by creating the first room at random and then repeating the accretion process until
        //the prior conditions have been met

        Tile RootRoomRootTile = null;
        int index = 0; //prevents infinite loop
        while(RootRoomRootTile == null)
        {
            RootRoomRootTile = TryCreateRoomCentre(0, tiles);
            index ++;
            if(index > 100){return;}
        }
        int roomCount = 0;
        Room rootRoom = new Room(RootRoomRootTile);
        rooms.Add(rootRoom);
        //Create list of valid rooms that can offshoot new rooms
        List<Room> ValidAccretionRooms = new List<Room>();
        ValidAccretionRooms.Add(rootRoom);
        int counter = 0; //prevents infinite loops
        while(rooms.Count < _MaxRoomCount && ValidAccretionRooms.Count > 0)
        {
            counter ++;
            if(counter > 100)
            {
                Debug.Log("room accretion forced stop after 100 iterations");
                return;
            }
            //Get current Room to accrete
            Room currRoom = ValidAccretionRooms[ValidAccretionRooms.Count - 1];
            //Try and generate new roomcentre
            Tile newRoomCentre = null;
            for(int i = 0; i < _maxTriesPerRoom; i ++)
            {
                newRoomCentre = TryAccreteRoom(currRoom,accretionMaxRange,accretionMinRange);
                if(newRoomCentre != null){i = 1000;}
            }
            if(newRoomCentre == null) //If failed, remove the current room from valid list
            {
                Debug.Log("failed to accrete new room");
                ValidAccretionRooms.Remove(currRoom);
            }
            else    //if succeeded, add new room
            {
                Debug.Log("new room accreted");
                Room newRoom = new Room(newRoomCentre);
                newRoomCentre.PaintTile(Tile.TileType.room);
                //Connect old room to child room
                currRoom.ConnectRoom(newRoom);
                ValidAccretionRooms.Add(newRoom);
                rooms.Add(newRoom);
            }

            if(rooms.Count >= _MaxRoomCount){Debug.Log("room count reached");}
            else if(ValidAccretionRooms.Count <= 0){Debug.Log("ran out of valid rooms");}
        }
    }
    private Tile TryAccreteRoom(Room _rootRoom,int _accretionMaxRadius, int _accretionMinRadius)
    {
        List<Tile> validTiles = new List<Tile>();
        Vector2Int cornerOffset = new Vector2Int(-1,-1) * _accretionMaxRadius / 2;
        Vector2Int dimensions = new Vector2Int(1,1) * (_accretionMaxRadius + 1);
        validTiles = TileGrid.instance.GetTilesInRegion(_rootRoom.rootTile.GetCoords() + cornerOffset,dimensions);
        
        List<Tile> invalidTiles = new List<Tile>();
        cornerOffset = new Vector2Int(-1,-1) * _accretionMinRadius/2;
        dimensions = new Vector2Int(1,1) * (_accretionMinRadius + 1);
        invalidTiles = TileGrid.instance.GetTilesInRegion(_rootRoom.rootTile.GetCoords() + cornerOffset, dimensions);
        foreach(Tile invalidTile in invalidTiles)
        {
            if(validTiles.Contains(invalidTile))
            {
                validTiles.Remove(invalidTile);
            }
        }
        if(validTiles.Count == 0){return null;}
        int rndIndex = Random.Range(0,validTiles.Count);
        foreach(Tile neighbour in validTiles[rndIndex].GetNeighbours())
        {
            if(neighbour.type == Tile.TileType.room){return null;}
        }
        if(validTiles[rndIndex].type == Tile.TileType.blank){return validTiles[rndIndex];}
        return null;
    }
    #endregion
    private void RemoveTilesFromCache(List<Tile> _tilesToRemove)
    {
        foreach(Tile tile in _tilesToRemove)
        {
            if(!tiles.Contains(tile)){continue;}
            tiles.Remove(tile);
        }
    }

    void OnDrawGizmos()
    {
        if(rooms == null){return;}
        if(rooms.Count < 1){return;}
        foreach(Room currRoom in rooms)
        {
            if(currRoom.GetConnectedRooms().Count < 1){continue;}
            foreach(Room connectedRoom in currRoom.GetConnectedRooms())
            {
                if(connectedRoom == null){continue;}
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currRoom.rootTile.transform.position,connectedRoom.rootTile.transform.position);
            }
        }
        // if(partitions.Count > 0)
        // {
        //     foreach(Partition p in partitions)
        //     {
        //         if(p.room == null){continue;}
        //         foreach(Room connection in p.room.GetConnectedRooms())
        //         {
        //             Gizmos.color = Color.yellow;
        //             Gizmos.DrawLine(p.room.rootTile.transform.position,connection.rootTile.transform.position);
        //         }
        //     }
        // }
    }
}

public class Partition
{
    public Partition parentPartition = null;
    public List<Partition> childPartitions = new List<Partition>();
    private List<Tile> tiles = new List<Tile>();
    public int generation = 0;
    public Room room;
    public Partition(Partition _parent, List<Partition> _children, List<Tile> _tiles, int _generation)
    {
        parentPartition = _parent;
        room = null;
        if(_children == null)
        {
            childPartitions = new List<Partition>();
        }
        else
        {
            childPartitions = _children;
        }
        tiles = _tiles;
        generation = _generation;
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
    public List<Tile> GetTiles(){return tiles;}
    public Partition GetSisterPartition(Partition _partition)
    {
        if(childPartitions.Count == 0){return null;}
        foreach(Partition p in childPartitions)
        {
            if(p != _partition){return p;}
        }
        return null;
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

public class Room
{
    public List<Tile> tiles = new List<Tile>();
    private List<Room> connectedRooms = new List<Room>();
    public Tile rootTile;
    public Room(Tile _rootTile)
    {
        rootTile = _rootTile;
    }
    public void AddTiles(List<Tile> _tiles)
    {
        foreach(Tile tile in _tiles)
        {
            if(tiles.Contains(tile)){continue;}
            tiles.Add(tile);
        }
    }
    public void ConnectRoom(Room _room)
    {
        if(_room == null){return;}
        if(_room == this){return;}
        if(_room.connectedRooms.Contains(this)){return;}
        if(connectedRooms.Contains(_room)){return;}
        connectedRooms.Add(_room);
    }
    public void DisconnectRoom(Room _room)
    {
        if(!connectedRooms.Contains(_room)){return;}
        connectedRooms.Remove(_room);
    }
    public List<Room> GetConnectedRooms()
    {
        return connectedRooms;
    }
}