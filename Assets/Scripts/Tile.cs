using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    void Start()
    {
        HighlightTile(false);
    }
    private Vector2Int coords;
    private List<Tile> Neighbours = new List<Tile>();
    private MaterialPropertyBlock block;
    private Color currColor;
    private static Color defaultColor = Color.grey;
    public static Color terrainColor = Color.black;
    public static Color roomColor = Color.white;
    public static Color edgeColor = Color.black;
    public static Color corridorColor = Color.white;

    private Room room;
    public bool insideRoom()
    {
        return room == null ? false : true;
    }
    public void Init(Vector2Int _coords)
    {
        coords = _coords;
    }
    public enum TileType
    {
        blank,
        terrain,
        room,
        Edge,
        hidden,
        Corridor
    }
    public TileType type = TileType.blank;
    public static float GetDistance(Tile a,Tile b, TileGrid.TileGridShape _shape)
    {
        if(_shape == TileGrid.TileGridShape.SQUARE)
        {
            return Vector2.Distance(a.coords,b.coords);
        }
        //Figure out if either tile is offset
        Vector2 aHexCoord,bHexCoord;
        aHexCoord = new Vector2(a.transform.position.x,a.transform.position.z);
        bHexCoord = new Vector2(b.transform.position.x,b.transform.position.z);
        return Vector2.Distance(aHexCoord,bHexCoord);
    }
    public List<Tile> GetNeighbours(bool _includeAll = false, int _radius = 1)
    {
        Neighbours = new List<Tile>();
        Vector2Int searchCorner = new Vector2Int(coords.x - _radius, coords.y - _radius);
        int diameter = (_radius * 2) + 1;
        for(int i = 0; i < diameter; i++){
            for(int j = 0; j < diameter; j++)
            {
                Vector2Int coord;
                if(TileGrid.instance.GetTileGridShape() == TileGrid.TileGridShape.SQUARE)
                {
                    coord = searchCorner + new Vector2Int(i,j);
                }
                else
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
                Tile neighbourTile = TileGrid.instance.GetTile(coord);
                if(neighbourTile == null){continue;}
                if(neighbourTile.type != TileType.blank && !_includeAll){continue;}
                if(neighbourTile == this){continue;}
                Neighbours.Add(neighbourTile);
            }
        }
        return Neighbours;
        // switch(TileGrid.instance.GetTileGridShape())
        // {
        //     case TileGrid.TileGridShape.SQUARE:
        //         AddNeighbourToList(new Vector2Int(-1,-1),_includeAll);
        //         AddNeighbourToList( new Vector2Int(-1,0),_includeAll);
        //         AddNeighbourToList( new Vector2Int(-1,1),_includeAll);

        //         AddNeighbourToList(new Vector2Int(0,-1),_includeAll);
        //         AddNeighbourToList( new Vector2Int(0,1),_includeAll);

        //         AddNeighbourToList( new Vector2Int(1,-1),_includeAll);
        //         AddNeighbourToList( new Vector2Int(1,0),_includeAll);
        //         AddNeighbourToList( new Vector2Int(1,1),_includeAll);
        //         break;
        //     case TileGrid.TileGridShape.HEX:
        //         AddNeighbourToList(new Vector2Int(-1,0),_includeAll);
        //         AddNeighbourToList(new Vector2Int(1,0),_includeAll);

        //         AddNeighbourToList(new Vector2Int(0,-1),_includeAll);
        //         AddNeighbourToList(new Vector2Int(0,1),_includeAll);
        //         if(coords.y % 2 == 0)
        //         {
        //             AddNeighbourToList(new Vector2Int(1,-1),_includeAll);
        //             AddNeighbourToList(new Vector2Int(1,1),_includeAll);
        //         }
        //         else
        //         {
        //             AddNeighbourToList(new Vector2Int(-1,1),_includeAll);
        //             AddNeighbourToList(new Vector2Int(-1,-1),_includeAll);
        //         }
        //         break;
        //     default:
        //         break;
        // }
        // return Neighbours;
        //Prune list
    }
    private void AddNeighbourToList(Vector2Int offset, bool _includeAll = false)
    {
        Vector2Int coord = coords + offset;
        Tile newTile = TileGrid.instance.GetTile(coord);
        if(newTile == null){return;}
        if(!TileGrid.instance.highlightTerrain && newTile.type == TileType.terrain && !_includeAll){return;}
        if(Neighbours.Contains(newTile)){return;}
        Neighbours.Add(newTile);
    }
    public void HighlightTile(bool _highLight)
    {
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        Color col = _highLight ? Color.white : currColor;
        block.SetColor("_BaseColor", col);
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    void OnMouseDown()
    {
        if(Input.GetMouseButton(0))
        {
            if(TileGrid.instance.highlightTerrain || type != TileType.terrain)
            {
                HighlightTile(true);
            }
            foreach(Tile tile in TileGrid.instance.GetTilesWithinDistance(coords,TileGrid.instance.highlightRadius))
            {
                tile.HighlightTile(true);
            }
        }
    }
    void OnMouseUp()
    {
        foreach(Tile tile in TileGrid.instance.GetTilesWithinDistance(coords,TileGrid.instance.highlightRadius))
        {
            tile.HighlightTile(false);
        }
        HighlightTile(false);
    }
    public void PaintTile(TileType _type)
    {
        if(type == TileType.Edge){return;}
        this.GetComponent<Renderer>().enabled = true;
        Vector3 pos = transform.position;
        Color col = defaultColor;
        switch(_type)
        {
            case TileType.blank:
                pos.y = 0;
                col = defaultColor;
                break;
            case TileType.terrain:
                pos.y = 1;
                col = terrainColor;
                break;
            case TileType.room:
                pos.y = .5f;
                col = roomColor;
                break;
            case TileType.Edge:
                pos.y = 1;
                col = edgeColor;
                break;
            case TileType.hidden:
                pos.y = 0;
                this.GetComponent<Renderer>().enabled = false;
                break;
            case TileType.Corridor:
                pos.y = 0;
                col = corridorColor;
                break;
        }
        transform.position = pos;
        type = _type;
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", col);
        currColor = col;
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public void ResetColor()
    {
        this.GetComponent<Renderer>().enabled = true;
        //Check if edge
        Vector3 pos = transform.position;
        pos.y = 0;
        transform.position = pos;
        type = TileType.blank;
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", defaultColor);
        currColor = defaultColor;
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public Vector2Int GetCoords()
    {
        return coords;
    }
}

