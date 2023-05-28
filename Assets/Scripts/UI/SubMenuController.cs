using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuController : MonoBehaviour
{
    //Class that controls any sub-menus that the dropdowns display
    public List<GameObject> subMenus = new List<GameObject>(); //All the submenu's that can be displayed
    public void ChangeSubMenu(int _index) //Show a certain sub menu
    {
        if(subMenus.Count == 0){return;} //return if no sub menus
        HideSubMenus(); //Hide all submenus
        if(_index < 0 || _index > subMenus.Count) //if the attempted index is too high, show submenu 0 and return
        {
            if(subMenus[0] == null){return;}
            subMenus[0].SetActive(true);
            return;
        }
        if(subMenus[_index] == null){return;}
        subMenus[_index].SetActive(true); //Show indexed sub menu
    }
    private void HideSubMenus() //hide all sub menus
    {
        foreach(GameObject subMenu in subMenus)
        {
            if(subMenu == null){continue;}
            subMenu.SetActive(false);
        }
    }
}
