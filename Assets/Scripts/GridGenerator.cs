using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector3 gridCentre;
    public Vector2Int gridDimensions;
    public int tileDimensions;
    public GameObject squareTilePrefab;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TileGrid.instance.Initialise(gridCentre,gridDimensions,squareTilePrefab,tileDimensions);
        }
    }
}
