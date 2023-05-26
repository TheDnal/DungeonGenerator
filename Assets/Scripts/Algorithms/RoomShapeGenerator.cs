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
    public int automataSeed = 0;
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
            case RoomShape.CELLULAR_AUTOMATA:
                Random.seed = automataSeed;
                foreach(Room room in rooms)
                {
                    GenerateCellularAutomataRoom(room);
                }
                break;
            case RoomShape.ALL:
                Random.seed = randomSeed;
                foreach(Room room in rooms)
                {
                    int rnd = Random.Range(0,3);
                    if(rnd == 0){GenerateCircleRoom(room);}
                    else if(rnd == 1){GenerateSquareRoom(room);}
                    else{GenerateCellularAutomataRoom(room);}
                }
                break;
        }
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
        room.AddTiles(temp);
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
        room.AddTiles(temp);
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
            foreach(Tile tile in nextIterationBlankTiles)
            {
                tile.PaintTile(Tile.TileType.blank);
            }
            foreach(Tile tile in nextIterationRoomTiles)
            {
                tile.PaintTile(Tile.TileType.room);
            }
        }
        room.AddTiles(temp);
    }
    
    #region Setters
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
    public void SetAutomataSeed(int _seed)
    {
        automataSeed = _seed;
        Generate();
    }
    #endregion
    public List<Room> GetRooms(){return rooms;}
}
