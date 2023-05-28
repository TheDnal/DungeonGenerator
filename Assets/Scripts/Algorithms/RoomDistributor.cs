using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDistributor : MonoBehaviour
{
    /*
        This class controls how rooms are distributed inside the grid
    */
    public enum roomDistributionType //type of room distribution
    {
        RANDOM,
        ACCRETION,
        BSP
    }
    private int roomCount = 5; //Room count
    private int seed = 0; //Distribution seed
    private int accretionMaxRange = 1,accretionMinRange = 5; //accretion settings
    public int minPartitionWidth = 3, minPartitionHeight = 3; //Min BSP dimensions
    public int minPartitionSize = 9; //Min BSP size
    public int maxPartitions = 8; //Max BSP's
    private roomDistributionType type; //current room distribution type
    private List<Tile> tiles = new List<Tile>(); //cached tiles
    private List<Tile> roomCentres = new List<Tile>(); //cached room centres
    private List<Room> rooms = new List<Room>(); //Cached rooms
    private List<Partition> divisiblePartitions = new List<Partition>(); //cached divisible BSP's 
    private List<Partition> partitions = new List<Partition>(); //cached BSP's
    public static RoomDistributor instance; //Singleton 
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
    //Set distribution values
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
    public void SetMaxPartitions(int _partitions)
    {
        maxPartitions = _partitions;
        Generate();
    }
    #endregion
    public List<Room> GetRooms(){return rooms;} //Get any rooms that have been generated
    private void Generate() //Distribute rooms
    {
        TileGrid.instance.ClearAllRooms(); //Clear all room tiles
        
        //Clear caches
        roomCentres.Clear();
        rooms.Clear();
        tiles.Clear();
        
        int iter = 0; //int to prevent infinite loops

        switch(type)
        {
            case roomDistributionType.RANDOM: //Randomly place rooms across grid
                int successfulRooms = 0;
                tiles = TileGrid.instance.GetUnsortedTiles(); //Get all tiles
                iter = 0;
                while(tiles.Count > 0 && rooms.Count != roomCount && iter < 100)
                {
                    iter ++;
                    Tile newRoomCentre = TryCreateRoomCentre(successfulRooms, tiles); //Try and place a room randomly
                    if(newRoomCentre != null)
                    {
                        roomCentres.Add(newRoomCentre); //Add room tile to list
                        Room newRoom = new Room(newRoomCentre); //Create new room
                        rooms.Add(newRoom); //add room to tlist
                        if(successfulRooms > 0) //Connect room to previous room (used for corridors in room details)
                        {
                            newRoom.ConnectRoom(rooms[successfulRooms - 1]);
                        }
                        successfulRooms++;
                    }
                }
                if(iter >= 100){Debug.Log("Warning: Potential infinite loop");} //prevents infinite loops
                break;
            case roomDistributionType.ACCRETION:
                tiles = TileGrid.instance.GetUnsortedTiles(); //Get unsorted tiles
                GenerateAccretion(roomCount); //Generate rooms via accretion
                break;
            case roomDistributionType.BSP:
                partitions.Clear(); //Clear partitions
                divisiblePartitions.Clear(); //Clear divisible partitions
                GenerateBinarySpacialPartition(); //Generate BSP tree
                foreach(Partition p in partitions) //iterate through every partition 
                {
                    if(p.childPartitions.Count > 0){continue;}
                    Color col = Random.ColorHSV(); // random color
                    foreach(Tile tile in p.GetTiles()) //Set all blank tiles in the partition to the random color
                    {
                        if(tile.type != Tile.TileType.blank){continue;}
                        tile.TempSetColor(col * 0.85f); //dull the color a bit
                    }
                    Tile centre = GetTileOfTypeInPartition(p,Tile.TileType.blank); //Try and find a valid tile in the partition to place aroom
                    tiles = p.GetTiles(); //Get all tiles in the partition
                    if(centre != null) //If random pos is valid, make a room there
                    {
                        centre.PaintTile(Tile.TileType.room);
                    }
                    else //otherwise make a room in the centre of the partition
                    {
                        centre = GetPartitionCentreTile(p);
                    }
                    centre.PaintTile(Tile.TileType.room); //paint room tile
                    Room newRoom = new Room(centre); //make new room
                    rooms.Add(newRoom); //add room
                    p.room = newRoom; //set partition room to this
                    //Get centre of BSP
                }
                ConnectBSPRooms(); //Connect partition rooms together
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
            //Partition partitionToDivide = GetLargestPartition(divisiblePartitions);
            Partition partitionToDivide = divisiblePartitions[Random.Range(0,divisiblePartitions.Count)];
            divisiblePartitions.Remove(partitionToDivide);

            List<Partition> childPartitions = AttemptSplitPartiton(partitionToDivide);
            if(childPartitions == null)
            {
                continue;
            }
            partitionToDivide.childPartitions = childPartitions;
            foreach(Partition p in childPartitions)
            {
                divisiblePartitions.Add(p);
                partitions.Add(p);
            }
        }
        if(iter >= 100){Debug.Log("Warning : Potential infinite loop");}
    }
    private Partition GetLargestPartition(List<Partition> _sample) //Gets the largest partition in the sample
    {
        if(_sample.Count == 0) //return if sample empty
        {
            Debug.Log("Couldn't find largest parititon");
            return null;
        }
        int currSize = 0,largestSize = 0;
        Partition biggestPartition = null;
        foreach(Partition p in _sample) //Iterate through each partition and find the largest sample with no children
        {
            currSize = p.GetTiles().Count;
            if(p.childPartitions.Count > 0){continue;}
            if(currSize > largestSize)
            {
                largestSize = currSize;
                biggestPartition = p;
            }
        }
        return biggestPartition; //return largest partition
    }
    private List<Partition> AttemptSplitPartiton(Partition _partitionToSplit) //Attempts to split the partition into two smaller daughter partitions
    {
        //Get tile dimensions
        int lowestX = 0,highestX = 0,lowestY = 0,highestY = 0;
        foreach(Tile tile in _partitionToSplit.GetTiles()) //Iterate through each tile in the partition and compare coords to get dimensions
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

        //Randomly pick to vertical split or horizontal split
        bool verticalSplitFirst = Random.Range(0,10) > 5 ? true : false;
        //Split all tiles into two groups : less than and greater than or equal to the division line
        Partition firstChild, secondChild;
        List<Tile> tilesGreaterThan = new List<Tile>(),tilesLessThan = new List<Tile>();
        
        if(verticalSplitFirst)
        {
            //Vertical split
            if(width > minPartitionWidth * 2)
            {
                foreach(Tile tile in _partitionToSplit.GetTiles()) //iterate through every tile
                {
                    if(tile.GetCoords().x >= lowestX + (width / 2)) //if beyond the split, add to greater than list
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
                foreach(Tile tile in _partitionToSplit.GetTiles()) //iterate through every tile
                {
                    if(tile.GetCoords().y >= lowestY + (height / 2)) //if beyond the split, add to greater than list
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
        if(tilesGreaterThan.Count == 0 || tilesLessThan.Count == 0){return null;} //return null if either list is empty

        //Create new partitions
        firstChild = new Partition(_partitionToSplit,null,tilesGreaterThan,_partitionToSplit.generation + 1);
        secondChild = new Partition(_partitionToSplit,null,tilesLessThan,_partitionToSplit.generation + 1);
        List<Partition> temp = new List<Partition>();
        temp.Add(firstChild);
        temp.Add(secondChild);
        return temp; //return children
    }
    private Tile GetPartitionCentreTile(Partition _partition) //Get centre of partition
    {
        int lowestX = 999,highestX = -999,lowestY = 999,highestY = -999; //Get largest and smallest  tile position in each dimension
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
        int width = highestX - lowestX; //Get width
        int height = highestY - lowestY; //Get height
        Vector2Int coord = new Vector2Int(lowestX + width /2, lowestY + height /2); //Get coord of centre tile
        return TileGrid.instance.GetTile(coord); //return tile closest to centre
    }
    private void ConnectBSPRooms() //Connect bsp rooms together
    {
        foreach(Partition currPartition in partitions) //for every room that has children, connect their root rooms together
        {
            Room roomA,roomB;
            //At the bottom of the tree, connect to sibling
            if(currPartition.childPartitions.Count == 0){continue;}
            roomA = GetPartitionRootRoom(currPartition.childPartitions[0]); //Get root of Room A
            roomB = GetPartitionRootRoom(currPartition.childPartitions[1]); //Get root of Room B
            if(roomA == null || roomB == null){Debug.Log("unable to find root room"); continue;}
            //Connect children
            roomA.ConnectRoom(roomB);
        }
    }
    private Room GetPartitionRootRoom(Partition _partition) //Gets the root room of the partition
    {
        bool bottomOfTree = false;
        Partition _currPartition = _partition;
        while(!bottomOfTree) //Travel down the tree until you hit the bottom
        {
            //Not bottom of tree
            if(_currPartition.childPartitions.Count != 0)
            {
                _currPartition = _currPartition.childPartitions[0];
                continue;
            }
            bottomOfTree = true;
        }
        return _currPartition.room; // return bottom of the tree
    }
    private Tile GetTileOfTypeInPartition(Partition _sample, Tile.TileType _targetType) //Get random tile of type in a partition
    {
        List<Tile> validTiles = new List<Tile>(); //cache list of all tiles of that type
        foreach(Tile currTile in _sample.GetTiles()) //Iterate through every tile
        {
            if(currTile.type != _targetType){continue;}
            validTiles.Add(currTile); //Add tile to list
        }
        if(validTiles.Count == 0){Debug.Log("Couldn't find valid tile to make a room"); return null;} //no tiles of that type
        int index = Random.Range(0,validTiles.Count); //random index
        return validTiles[index]; //return random tile
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
        while(RootRoomRootTile == null)//Try get random pos for first room
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
    private Tile TryAccreteRoom(Room _rootRoom,int _accretionMaxRadius, int _accretionMinRadius) //Tries to accrete new room from root room
    {
        //Get all tiles that are valid accretion tiles
        List<Tile> validTiles = new List<Tile>();
        Vector2Int cornerOffset = new Vector2Int(-1,-1) * _accretionMaxRadius / 2; //Get corner of search area
        Vector2Int dimensions = new Vector2Int(1,1) * (_accretionMaxRadius + 1); //Get dimension of search area
        validTiles = TileGrid.instance.GetTilesInRegion(_rootRoom.rootTile.GetCoords() + cornerOffset,dimensions); //get all tiles in search area
        
        //Get all tiles that are invalid accretion tiles
        List<Tile> invalidTiles = new List<Tile>();
        cornerOffset = new Vector2Int(-1,-1) * _accretionMinRadius/2;
        dimensions = new Vector2Int(1,1) * (_accretionMinRadius + 1);
        invalidTiles = TileGrid.instance.GetTilesInRegion(_rootRoom.rootTile.GetCoords() + cornerOffset, dimensions); //Get all tiles in invalid area
        foreach(Tile invalidTile in invalidTiles) //Remove all invalid tiles from valid tiles
        {
            if(validTiles.Contains(invalidTile))
            {
                validTiles.Remove(invalidTile);
            }
        }
        if(validTiles.Count == 0){return null;} // no valid tiles
        int rndIndex = Random.Range(0,validTiles.Count); 
        foreach(Tile neighbour in validTiles[rndIndex].GetNeighbours()) //Get neighbours from random tile in valid tiles
        {
            if(neighbour.type == Tile.TileType.room){return null;} //return null
        }
        if(validTiles[rndIndex].type == Tile.TileType.blank){return validTiles[rndIndex];} //return valid tile
        return null;
    }
    #endregion
    private void RemoveTilesFromCache(List<Tile> _tilesToRemove) //Clear cache of tiles
    {
        foreach(Tile tile in _tilesToRemove)
        {
            if(!tiles.Contains(tile)){continue;}
            tiles.Remove(tile);
        }
    }

    void OnDrawGizmos() //Draw connections between room roots
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
    }
}

public class Partition //Spacial partition for BSP
{
    public Partition parentPartition = null; //parent in tree
    public List<Partition> childPartitions = new List<Partition>(); //children in tree
    private List<Tile> tiles = new List<Tile>(); //tiles inside partition
    public int generation = 0; //Generation in tree
    public Room room; //room associated with partition
    public Partition(Partition _parent, List<Partition> _children, List<Tile> _tiles, int _generation) //Constructor
    {
        parentPartition = _parent;
        room = null;
        if(_children == null) //init list
        {
            childPartitions = new List<Partition>();
        }
        else
        {
            childPartitions = _children;
        }
        tiles = _tiles;
        generation = _generation;
        if(childPartitions != null) //set child partitions parent to this
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
    public List<Tile> GetTiles(){return tiles;} //Gets all tiles in the partition
}

public class Room //Class that represents the rooms in the dungeon
{
    public List<Tile> tiles = new List<Tile>(); //All the tiles in the room
    private List<Room> connectedRooms = new List<Room>(); //all rooms directly connected to this one
    public Tile rootTile; //root tile for the room
    public Room(Tile _rootTile) //init
    {
        rootTile = _rootTile;
    }
    public void AddTiles(List<Tile> _tiles) //Add list of tiles to the room
    {
        foreach(Tile tile in _tiles)
        {
            if(tiles.Contains(tile)){continue;}
            tiles.Add(tile);
        }
    }
    public void ConnectRoom(Room _room) //Connect room to this room, only new connections allowed
    {
        if(_room == null){return;}
        if(_room == this){return;}
        if(_room.connectedRooms.Contains(this)){return;}
        if(connectedRooms.Contains(_room)){return;}
        connectedRooms.Add(_room);
    }
    public void DisconnectRoom(Room _room) //Disconnect room from this room, must be connected to this room before disconnecting
    {
        if(!connectedRooms.Contains(_room)){return;}
        connectedRooms.Remove(_room);
    }
    public List<Room> GetConnectedRooms() //Get all rooms directly connected to this one
    {
        return connectedRooms;
    }
}