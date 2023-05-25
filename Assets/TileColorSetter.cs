using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorSetter : MonoBehaviour
{
    private int tileType;
    private Color col;
    public TMPro.TMP_Dropdown dropdownUI;
    public void SetTileColor()
    {
        tileType = dropdownUI.value;
        Tile.SetColor(tileType);
    }
}
