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
    private static Color defaultColor = Color.black;
    public void Init(Vector2Int _coords)
    {
        coords = _coords;
    }
    public enum TileType
    {
        blank,
        terrain
    }
    public TileType type = TileType.blank;
    public List<Tile> GetNeighbours()
    {
        Neighbours = new List<Tile>();
        switch(TileGrid.instance.GetTileGridShape())
        {
            case TileGrid.TileGridShape.SQUARE:
                AddNeighbourToList(new Vector2Int(-1,-1));
                AddNeighbourToList( new Vector2Int(-1,0));
                AddNeighbourToList( new Vector2Int(-1,1));

                AddNeighbourToList(new Vector2Int(0,-1));
                AddNeighbourToList( new Vector2Int(0,1));

                AddNeighbourToList( new Vector2Int(1,-1));
                AddNeighbourToList( new Vector2Int(1,0));
                AddNeighbourToList( new Vector2Int(1,1));
                break;
            case TileGrid.TileGridShape.HEX:
                AddNeighbourToList(new Vector2Int(-1,0));
                AddNeighbourToList(new Vector2Int(1,0));

                AddNeighbourToList(new Vector2Int(0,-1));
                AddNeighbourToList(new Vector2Int(0,1));
                if(coords.y % 2 == 0)
                {
                    AddNeighbourToList(new Vector2Int(1,-1));
                    AddNeighbourToList(new Vector2Int(1,1));
                }
                else
                {
                    AddNeighbourToList(new Vector2Int(-1,1));
                    AddNeighbourToList(new Vector2Int(-1,-1));
                }
                break;
            default:
                break;
        }
        return Neighbours;
        //Prune list
    }
    private void AddNeighbourToList(Vector2Int offset)
    {
        Vector2Int coord = coords + offset;
        Tile newTile = TileGrid.instance.GetTile(coord);
        if(newTile == null){return;}
        if(!TileGrid.instance.highlightTerrain && newTile.type == TileType.terrain){return;}
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
            foreach(Tile tile in GetNeighbours())
            {
                tile.HighlightTile(true);
            }
        }
    }
    void OnMouseUp()
    {
        foreach(Tile tile in GetNeighbours())
        {
            tile.HighlightTile(false);
        }
        HighlightTile(false);
    }
    public void PaintTile(Color _paintColor, TileType _type)
    {
        type = _type;
        block = new MaterialPropertyBlock();
        this.GetComponent<Renderer>().GetPropertyBlock(block);
        block.SetColor("_BaseColor", _paintColor);
        currColor = _paintColor;
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
    public void ResetColor()
    {
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

