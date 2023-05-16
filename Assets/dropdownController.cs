using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class dropdownController : MonoBehaviour
{
    public enum dropdownType
    {
        GridShape,
        TerrainType,
        roomDistributionType
    }
    public dropdownType type = dropdownType.GridShape;
    public SubMenuController subMenu;
    public void Initialise()
    {
        int index = this.GetComponent<TMP_Dropdown>().value;
        switch(type)
        {
            case dropdownType.GridShape:
                GridGenerator.instance.SetGridShape(index);
                break;
            case dropdownType.TerrainType:
                GridTerrainGenerator.instance.SetTerrainType(index);
                break;
            case dropdownType.roomDistributionType:
                RoomDistributor.instance.SetDistributionType(index);
                break;
        }
        if(subMenu == null){return;}
        subMenu.ChangeSubMenu(index);
    }
}
