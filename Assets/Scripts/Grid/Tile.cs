using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /*
        The tile class represents each and every tile on the grid used by the program.
    */
    #region Static Variables
    //These static variables control the color of the tile. Each TileType enum has an associated color.
    private static Color defaultColor = Color.grey;
    public static Color terrainColor = Color.black;
    public static Color roomColor = Color.white;
    public static Color edgeColor = Color.black;
    public static Color corridorColor = Color.white;
    public static Color WallColor = new Color(0.25f,0.25f,0.25f,1);
    
    //Color varience just represents a randomness added to the color that gives tiles a bit of texture
    public static float defaultVarience = 0;
    public static float terrainVarience = 0;
    public static float roomVarience = 0;
    public static float edgeVarience = 0;
    public static float corridorVarience = 0;
    public static float WallVarience = 0;
    #region Variables
    private Vector2Int coords;
    private List<Tile> Neighbours = new List<Tile>();
    private MaterialPropertyBlock block;
    private Color currColor;
    private Room room;
    #endregion
    #endregion
    public enum TileType //The types that the tile can become, crucial for generating the dungeon
    {
        blank,
        terrain, 
        room, 
        Edge,
        hidden,
        Corridor,
        Wall
    }
    public TileType type = TileType.blank; //The type of the current tile
    public static void SetColor(int _typeIndex, float _varience = 0) //Set the color of a tile type, as well as its varience.
    {
        Color col = ColorPickerControl.instance.GetColor(); //Get col from color picker
        switch((TileType)_typeIndex)
        {
            case TileType.blank:
                defaultColor = col;
                defaultVarience = _varience;
                break;
            case TileType.terrain:
                terrainColor = col;
                terrainVarience = _varience;
                break;
            case TileType.room:
                roomColor = col;
                roomVarience = _varience;
                break;
            case TileType.Edge:
                edgeColor = col;
                edgeVarience = _varience;
                break;
            case TileType.hidden:
                break;
            case TileType.Corridor:
                corridorColor = col;
                corridorVarience = _varience;
                break;
            case TileType.Wall:
                WallColor = col;
                WallVarience = _varience;
                break;
            default:
                break;
        }
        TileGrid.instance.RefreshTileColors(); //refresh all tiles so that they take on the new tile colors
    }
    void Start()
    {
        HighlightTile(false); //Set the tile to not be highlighted
    }
    public void Init(Vector2Int _coords) // Set up the coordinates of the tile by the TileGrid class
    {
        coords = _coords;
    }
    public List<Tile> GetNeighbours(bool _includeAll = false, int _radius = 1) //Gets all the neighbours within a radius of the tile
    {
        Neighbours = new List<Tile>(); //Cache the neighbours
        Vector2Int searchCorner = new Vector2Int(coords.x - _radius, coords.y - _radius); //Get all tiles in a square grid around this tile
        int diameter = (_radius * 2) + 1; //Calculate the diamater
        //For each neighbour, add it to the neighbour list, only include blanl tiles, unless _includeAll is enabled
        for(int i = 0; i < diameter; i++){
            for(int j = 0; j < diameter; j++)
            {
                Vector2Int coord;
                if(TileGrid.instance.GetTileGridShape() == TileGrid.TileGridShape.SQUARE) //Get all tilse in a square radius if the tile shape is square
                {
                    coord = searchCorner + new Vector2Int(i,j);
                }
                else    //Else (if the tile shape is hexagon) then offset the searchCorner
                {
                    if(j % 2 == 0)//Even
                    {
                        coord = coord = searchCorner + new Vector2Int(i,j);
                    }   
                    else//Odd
                    {
                        coord = searchCorner + new Vector2Int(i - 1, j);
                    }
                }
                //Get the tile at this coordinate
                Tile neighbourTile = TileGrid.instance.GetTile(coord);
                //Check if the tile is valid
                if(neighbourTile == null){continue;}
                if(neighbourTile.type != TileType.blank && !_includeAll){continue;}
                if(neighbourTile == this){continue;}
                //add tile
                Neighbours.Add(neighbourTile);
            }
        }
        //return all neighbours
        return Neighbours;
    }
    public void HighlightTile(bool _highLight) //This method temporarly highlights a tile when the mouse click on it
    {
        //Get the material property block and change its color to white (highlighted)
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        Color col = _highLight ? Color.white : currColor;
        block.SetColor("_BaseColor", col);
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    void OnMouseDown() //Highlight the tile and its neighbours
    {
        if(Input.GetMouseButton(0))
        {
            if(TileGrid.instance.highlightTerrain || type != TileType.terrain) //If highlighting terrain is enabled or this tile isn't a terrain tile, highlight
            {
                HighlightTile(true);
            }
            foreach(Tile tile in TileGrid.instance.GetTilesWithinDistance(coords,TileGrid.instance.highlightRadius)) //Highlight all neighbours
            {
                tile.HighlightTile(true);
            }
        }
    }
    void OnMouseUp()
    {
        foreach(Tile tile in TileGrid.instance.GetTilesWithinDistance(coords,TileGrid.instance.highlightRadius)) //Reset any highlighted neighbour tiles on mouse up
        {
            tile.HighlightTile(false);
        }
        HighlightTile(false); //unhighlight this tile
    }
    public void PaintTile(TileType _type, bool _overrideAll = false, float _heightOffset = 0) //Painting a tile to change its tile type
    {
        if(type == TileType.Edge && _overrideAll == false){return;} //Edges can't be painted normally unless overriden
        this.GetComponent<Renderer>().enabled = true;
        Vector3 pos = transform.position;
        Color col = defaultColor;
        float varience = 0;
        switch(_type) //The process for changing a type goes as follows : 
        {
            case TileType.blank:    
                pos.y = 0 + _heightOffset; //Change the height offset is there is any
                col = defaultColor; //Update the color 
                varience = defaultVarience; //Update the color varience
                break;
            case TileType.terrain:
                pos.y = 1.25f + _heightOffset;
                col = terrainColor;
                varience = terrainVarience;
                break;
            case TileType.room:
                pos.y = .0f + _heightOffset;
                col = roomColor;
                varience = roomVarience;
                break;
            case TileType.Edge:
                pos.y = 1.5f + _heightOffset;
                col = edgeColor;
                varience = edgeVarience;
                break;
            case TileType.hidden:
                pos.y = 0 + _heightOffset;
                this.GetComponent<Renderer>().enabled = false;
                break;
            case TileType.Corridor:
                pos.y = 0.0f + _heightOffset;
                col = corridorColor;
                varience = corridorVarience;
                break;
            case TileType.Wall:
                pos.y = 1f + _heightOffset;
                col = WallColor;
                varience = WallVarience;
                break;
        }
        if(varience != 0) //If there's any varience, apply it to the color (add random noise to the color)
        {
            col *= Random.Range(1 - varience, 1 + varience);
        }
        transform.position = pos; //apply new position
        type = _type; //apply new type

        //Apply new color to material property block
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", col);
        currColor = col;
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public void TempSetColor(Color col) //This method changes the color of the tile without changing its type, this is useful for debugging and visualising
    {
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", col);
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public void ResetColor() //Reset the type of the tile
    {
        this.GetComponent<Renderer>().enabled = true;
        //Check if edge
        Vector3 pos = transform.position; 
        pos.y = 0; // reset height
        transform.position = pos; 
        type = TileType.blank; //reset type

        //reset color
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", defaultColor);
        currColor = defaultColor;
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public Vector2Int GetCoords() //Get coords of the tile
    {
        return coords;
    }
}

