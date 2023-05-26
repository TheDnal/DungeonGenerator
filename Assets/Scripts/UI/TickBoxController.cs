using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TickBoxController : MonoBehaviour
{
    public Image tickImage; 
    public bool ticked = false;
    public enum DataType
    {
        Null,
        CanGenerateOverTerrain,
        CorridorsEnabled,
        WallsEnabled,
        WallsGenerateOverTerrain,
        ErosionEnabled,
        CorridorsOverlap,
        CorridorsHaveWalls
    }
    public DataType type = DataType.Null;
    void Start()
    {
        tickImage.gameObject.SetActive(ticked);
    }

    public void ToggleTicked()
    {
        ticked = ticked == true ? false : true;
        tickImage.gameObject.SetActive(ticked);
        switch(type)
        {
            case DataType.Null:
                break;
            case DataType.CanGenerateOverTerrain:
                RoomShapeGenerator.instance.SetRoomsCanGenerateOverTerrain(ticked);
                break;
            case DataType.CorridorsEnabled:
                RoomDetailsGenerator.instance.SetCorridorsEnabled(ticked);
                break;
            case DataType.WallsEnabled:
                RoomDetailsGenerator.instance.SetWallsEnabled(ticked);
                break;
            case DataType.WallsGenerateOverTerrain:
                RoomDetailsGenerator.instance.SetWallsGenThroughTerrainEnabled(ticked);
                break;
            case DataType.ErosionEnabled:
                RoomDetailsGenerator.instance.SetErosionEnabled(ticked);
                break;
            case DataType.CorridorsOverlap:
                RoomDetailsGenerator.instance.SetCorrdorsOverlap(ticked);
                break;
            case DataType.CorridorsHaveWalls:
                RoomDetailsGenerator.instance.SetCorridorsHaveWalls(ticked);
                break;
        }
    }

}
