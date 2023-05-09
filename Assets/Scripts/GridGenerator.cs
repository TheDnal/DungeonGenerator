using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector3 gridCentre;
    public Vector2Int gridDimensions;
    public float tileDimensions;
    public GameObject squareTilePrefab;
    public TileGrid.TileGridShape gridShape = TileGrid.TileGridShape.SQUARE;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TileGrid.instance.Initialise(gridCentre,gridDimensions,squareTilePrefab,tileDimensions,gridShape);
        }
    }
}
