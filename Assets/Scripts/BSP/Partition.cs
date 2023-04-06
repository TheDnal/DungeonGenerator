using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partition
{
    public Vector2Int corner { get; private set; }
    public Vector2Int dimensions { get; private set; }
    public Partition parent{get; private set;}
    public List<Partition> children = new List<Partition>();
    public Partition(Vector2Int _corner, Vector2Int _dimensions, Partition _parent = null)
    {
        corner = _corner;
        dimensions = _dimensions;
        parent =_parent;
    }
    public void SetChildren(List<Partition> _children){children.Clear(); children.AddRange(_children);}
    public bool isBisectable()
    {
        return dimensions.x > 6 && dimensions.y > 6 ? true : false;
    }
    public List<Partition> bisectPartition(bool _isVertical = false)
    {
        return _isVertical ? verticalBisect() : horizontalBisect();
    }
    private List<Partition> horizontalBisect()
    {
        //Vertical only for now 
        Partition a,b;
        Vector2Int aDimensions = Vector2Int.zero, bDimensions = Vector2Int.zero, aCorner = Vector2Int.zero, bCorner = Vector2Int.zero;
        //Test for odd or even
        //x
        #region dimensions
        if(dimensions.y / 2 % 2 != 0)
        {
            aDimensions.y = Mathf.CeilToInt(dimensions.y / 2);
            bDimensions.y = Mathf.FloorToInt(dimensions.y / 2);
        }
        else
        {
            aDimensions.y = dimensions.y / 2;
            bDimensions.y = dimensions.y / 2;
        }
        //y
        aDimensions.x = dimensions.x;
        bDimensions.x = dimensions.x;
        #endregion
        #region corners
        aCorner = corner;
        bCorner.x = corner.x;
        //Test for odd or even
        if(dimensions.y / 2 % 2 != 0)
        {
            bCorner.y = corner.y + Mathf.CeilToInt(dimensions.y / 2);
        }
        else
        {
            bCorner.y = corner.y + Mathf.RoundToInt(dimensions.y / 2);
        }
        #endregion
        a = new Partition(aCorner, aDimensions);
        b = new Partition(bCorner, bDimensions);
        List<Partition> temp = new List<Partition>();
        temp.Add(a);
        temp.Add(b);
        return temp;
    }
    private List<Partition> verticalBisect()
    {
        //Vertical only for now 
        Partition a,b;
        Vector2Int aDimensions = Vector2Int.zero, bDimensions = Vector2Int.zero, aCorner = Vector2Int.zero, bCorner = Vector2Int.zero;
        //Test for odd or even
        //x
        #region dimensions
        if(dimensions.x / 2 % 2 != 0)
        {
            aDimensions.x = Mathf.CeilToInt(dimensions.x / 2);
            bDimensions.x = Mathf.FloorToInt(dimensions.x / 2);
        }
        else
        {
            aDimensions.x = dimensions.x / 2;
            bDimensions.x = dimensions.x / 2;
        }
        //y
        aDimensions.y = dimensions.y;
        bDimensions.y = dimensions.y;
        #endregion
        #region corners
        aCorner = corner;
        bCorner.y = corner.y;
        //Test for odd or even
        if(dimensions.x / 2 % 2 != 0)
        {
            bCorner.x = corner.x + Mathf.CeilToInt(dimensions.x / 2);
        }
        else
        {
            bCorner.x = corner.x + Mathf.RoundToInt(dimensions.x / 2);
        }
        #endregion
        a = new Partition(aCorner, aDimensions);
        b = new Partition(bCorner, bDimensions);
        List<Partition> temp = new List<Partition>();
        temp.Add(a);
        temp.Add(b);
        return temp;
    }
}
