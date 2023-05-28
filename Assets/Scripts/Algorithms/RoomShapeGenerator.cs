using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShapeGenerator : MonoBehaviour
{
    /*
        Class that generates the shape of each room
    */
    public enum RoomShape //Shape that rooms will take
    {
        CIRCLES,
        SQUARES,
        CELLULAR_AUTOMATA,
        ALL //Will randomly cycle between all other room shapes 
    }
    private RoomShape shape = RoomShape.CIRCLES; //Default room shape
    private List<Room> rooms = new List<Room>(); //List of rooms
    public int roomSize = 1; //Size of rooms
    public bool roomsCanGenerateIntoTerrain = true; //Controls whether or not rooms can override terrain tiles
    public int randomSeed = 0; //Seed for all shapes
    public int automataSeed = 0; //seed for cellular automata rooms
    public static RoomShapeGenerator instance; //Singleton
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
    public void Generate() //Generates room shapes
    {
        rooms = RoomDistributor.instance.GetRooms(); //gets all rooms from the room distributor
        if(rooms == null){return;}
        if(rooms.Count < 1){return;}
        TileGrid.instance.ClearAllRooms(); //Clears all room shapes that may have existed
        switch(shape)
        {
            case RoomShape.SQUARES:
                foreach(Room room in rooms) //Generate squares for each room
                {
                    GenerateSquareRoom(room);
                }
                break;
            case RoomShape.CIRCLES:
                foreach(Room room in rooms) //Generate circles for each room
                {
                    GenerateCircleRoom(room);
                }
                break;
            case RoomShape.CELLULAR_AUTOMATA:
                Random.seed = automataSeed;
                foreach(Room room in rooms) //Generate automata shapes for each room
                {
                    GenerateCellularAutomataRoom(room);
                }
                break;
            case RoomShape.ALL:
                Random.seed = randomSeed;
                foreach(Room room in rooms) //For each room, randomly pick a room generation type
                {
                    int rnd = Random.Range(0,3);
                    if(rnd == 0){GenerateCircleRoom(room);}
                    else if(rnd == 1){GenerateSquareRoom(room);}
                    else{GenerateCellularAutomataRoom(room);}
                }
                break;
        }
    }
    private void GenerateCircleRoom(Room room) //Sets all tiles within a radius of the rooms root to room tiles
    {
        List<Tile> temp = new List<Tile>();
        int circleRadius = 1 + (roomSize * 3); //radius of circle
        temp.Clear();
        room.rootTile.PaintTile(Tile.TileType.room); //paint root tile
        Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize; //get region of affected tiles
        Vector2Int dimensions = circleRadius * Vector2Int.one; 
        foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions)) //Iterate through each tile in area
        {
            if(!roomsCanGenerateIntoTerrain) //If cant generate over terrain, and the tile type is hidden or terrain, continue
            {
                if(neighbour.type == Tile.TileType.hidden || neighbour.type == Tile.TileType.terrain){continue;}
            }
            if(Vector3.Distance(neighbour.transform.position, room.rootTile.transform.position) > roomSize){continue;} //if distance too great, continue
            neighbour.PaintTile(Tile.TileType.room); //paint tile and add it to list
            temp.Add(neighbour);
        }
        room.AddTiles(temp); //add all tiles in list to room 
    }
    private void GenerateSquareRoom(Room room) //Adds all tiles within square region to room tiles
    {
        List<Tile> temp = new List<Tile>();
        int radius = 1 + (roomSize * 2); //radius of area
        temp.Clear();
        room.rootTile.PaintTile(Tile.TileType.room); //paint root tile
        Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize; //get corner of region
        Vector2Int dimensions = radius * Vector2Int.one;
        foreach(Tile neighbour in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions)) //Get all tiles in region
        {
            if(!roomsCanGenerateIntoTerrain) //If cant generate over terrain, and the tile type is hidden or terrain, continue 
            {
                if(neighbour.type == Tile.TileType.hidden || neighbour.type == Tile.TileType.terrain){continue;}
            }
            neighbour.PaintTile(Tile.TileType.room); //paint tile and add to list
            temp.Add(neighbour);
        }
        room.AddTiles(temp); //add all tiles in list to room
    }
    private void GenerateCellularAutomataRoom(Room room)
    {
        //Uses as 5/4 rule,
        //a tile becomes a blank tile if it was a blank tile and and 4 or more of its neighbours are
        //blank tiles
        //a tile becomes a blank tile if it is not a blank tile and 5 or more its neighbours are blank tiles
        //Got the rule from : https://roguebasin.com/index.php/Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
        int iterations = 15; //Number of iterations to condense noise into room
        List<Tile> temp = new List<Tile>();
        temp.Clear();
        int radius = 1 + (roomSize * 2);
        room.rootTile.PaintTile(Tile.TileType.room);
        Vector2Int regionCorner = room.rootTile.GetCoords() - Vector2Int.one * roomSize;
        Vector2Int dimensions = radius * Vector2Int.one;
        //Tiles must all be changed at the same time for cellular automata to work, so tiles that need to change are cached
        List<Tile> nextIterationBlankTiles = new List<Tile>(), nextIterationRoomTiles = new List<Tile>();
        
        //Randomly set tiles
        foreach(Tile tile in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
        {
            if(tile.type == Tile.TileType.terrain && !roomsCanGenerateIntoTerrain){continue;}
            if(Random.Range(0,100) > 45){tile.PaintTile(Tile.TileType.room);}
        }
        for(int i =0 ; i < iterations; i++)
        {
            nextIterationBlankTiles.Clear();
            nextIterationRoomTiles.Clear();
            //Iterate over each tile 
            foreach(Tile tile in TileGrid.instance.GetTilesInRegion(regionCorner,dimensions))
            {
                int blankNeighbours = 0;
                foreach(Tile neighbourTile in tile.GetNeighbours())
                {
                    if(neighbourTile.type == Tile.TileType.blank){blankNeighbours++;}
                }
                //Check if can become blank tile
                if(tile.type == Tile.TileType.terrain && !roomsCanGenerateIntoTerrain){continue;}
                if(blankNeighbours >= 5 && tile.type != Tile.TileType.blank)
                {
                    nextIterationBlankTiles.Add(tile);
                }
                else if(blankNeighbours >= 4 && tile.type == Tile.TileType.blank)
                {
                    nextIterationBlankTiles.Add(tile);
                }
                else
                {
                    nextIterationRoomTiles.Add(tile);
                }
            }
            //Set up the next iteration of Cellular automata tiles at the same time
            foreach(Tile tile in nextIterationBlankTiles)
            {
                tile.PaintTile(Tile.TileType.blank);
            }
            foreach(Tile tile in nextIterationRoomTiles)
            {
                tile.PaintTile(Tile.TileType.room);
            }
        }
        room.AddTiles(temp); //add all valid tiles to room
    }
    
    #region Setters
    //Set values
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
    public void SetRandomSeed(int _seed)
    {
        randomSeed = _seed;
        Generate();
    }
    public void SetAutomataSeed(int _seed)
    {
        automataSeed = _seed;
        Generate();
    }
    #endregion
    public List<Room> GetRooms(){return rooms;} //Get all rooms 
}
