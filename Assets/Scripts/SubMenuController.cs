using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuController : MonoBehaviour
{
    public List<GameObject> subMenus = new List<GameObject>();
    public void ChangeSubMenu(int _index)
    {
        if(subMenus.Count == 0){return;}
        HideSubMenus();
        if(_index < 0 || _index > subMenus.Count)
        {
            if(subMenus[0] == null){return;}
            subMenus[0].SetActive(true);
            return;
        }
        if(subMenus[_index] == null){return;}
        subMenus[_index].SetActive(true);
    }
    private void HideSubMenus()
    {
        foreach(GameObject subMenu in subMenus)
        {
            if(subMenu == null){continue;}
            subMenu.SetActive(false);
        }
    }
}
