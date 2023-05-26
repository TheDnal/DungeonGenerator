using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HelpButtonController : MonoBehaviour
{
    public Color activeColor, idleColor;
    private bool displaying = false;
    public GameObject HelpBox;
    void Awake()
    {
        displaying = false;
        if(HelpBox != null){HelpBox.SetActive(displaying);}
        this.GetComponent<Image>().color = displaying == true ? activeColor : idleColor;
    }
    public void HelpButtonClicked()
    {
        displaying = displaying == true ? false : true;
        if(HelpBox != null){HelpBox.SetActive(displaying);}
        this.GetComponent<Image>().color = displaying == true ? activeColor : idleColor;
    }
}
