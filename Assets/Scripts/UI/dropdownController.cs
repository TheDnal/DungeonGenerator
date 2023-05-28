using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class dropdownController : MonoBehaviour
{
    //Class that controls the various dropdown UI's in the program
    public enum dropdownType //What the dropdown effects
    {
        GridShape,  //Shape of the tile grid
        TerrainType, // Type of noise used for the terrain
        RoomDistributionType, // How the rooms are distributed
        RoomShapeType, //Shape of the rooms
        None, 
        SetTileTypesColor //Set the color for a type of tile
    }
    public dropdownType type = dropdownType.GridShape; //Default is grid shape
    public SubMenuController subMenu; //Reference to the submenu controller, that will bring up any new UI
    //That the dropdown leads to.
    public void Initialise()
    {
        int index = this.GetComponent<TMP_Dropdown>().value; //Get the value of the dropdown UI
        switch(type)
        {
            case dropdownType.GridShape:
                GridGenerator.instance.SetGridShape(index); //Set the grid shape
                break;
            case dropdownType.TerrainType:
                GridTerrainGenerator.instance.SetTerrainType(index); //Set the terrain type
                break;
            case dropdownType.RoomDistributionType:
                RoomDistributor.instance.SetDistributionType(index); //Set the distribution type
                break;
            case dropdownType.RoomShapeType: 
                RoomShapeGenerator.instance.SetRoomShape(index); //Set the rooms shape type
                break;
            case dropdownType.SetTileTypesColor:
                break;
            default:
                break;
        }
        if(subMenu == null){return;}
        subMenu.ChangeSubMenu(index); //Change the submenu
    }
    public int GetIndex(){return this.GetComponent<TMP_Dropdown>().value;} //Get the index
}
