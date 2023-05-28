using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorSetter : MonoBehaviour
{
    //Class that gets the color and varience of the color ui and
    //applies it to the correct tile type
    private int tileType; //Current selected tile type
    private Color col; //current selected color
    public TMPro.TMP_Dropdown dropdownUI; //Dropdown that displays the tile tye
    private float varience = 0; //color varience
    public static TileColorSetter instance; //Singleton instance
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(instance);
            }
        }
        instance = this;
    }
    public void SetTileColor() //Set color for tile type
    {
        tileType = dropdownUI.value; //Get type from dropdown
        Tile.SetColor(tileType,varience); //Set color of Tile type
    }
    public void SetTileColorVarience(float _varience) //Sets the varience (used by other color scripts)
    {
        if(_varience == varience){return;}
        varience = _varience;
        //SetTileColor();
    }
}
