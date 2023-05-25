using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorSetter : MonoBehaviour
{
    private int tileType;
    private Color col;
    public TMPro.TMP_Dropdown dropdownUI;
    private float varience = 0;
    public static TileColorSetter instance;
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
    public void SetTileColor()
    {
        tileType = dropdownUI.value;
        Tile.SetColor(tileType,varience);
    }
    public void SetTileColorVarience(float _varience)
    {
        if(_varience == varience){return;}
        varience = _varience;
        SetTileColor();
    }
}
