using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TickBoxController : MonoBehaviour
{
    //Class that controls the Tick boxes found within the program
    public Image tickImage; //Image of the tick
    public bool ticked = false; //Whether or not the tickboxed is ticked
    public enum DataType //The data that the tickbox is in charge of
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
        //Set the tick boxes default state
        tickImage.gameObject.SetActive(ticked);
    }

    public void ToggleTicked() //When the mouse clicks on the tickbox
    {
        ticked = ticked == true ? false : true; //Swap bool value
        tickImage.gameObject.SetActive(ticked); //display or hide the tick
        switch(type) //Update relevant data
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
