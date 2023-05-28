using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HelpButtonController : MonoBehaviour
{
    //Class that controls the help buttons found in the program
    public Color activeColor, idleColor; //Active and idle color of the button
    private bool displaying = false; //Whether or not this class is displaying its helpbox
    public GameObject HelpBox; //help box reference
    void Awake() //Hide the helpbox and set the color to idle
    {
        displaying = false;
        if(HelpBox != null){HelpBox.SetActive(displaying);}
        this.GetComponent<Image>().color = displaying == true ? activeColor : idleColor;
    }
    public void HelpButtonClicked() //Switch between having the active color and showing the helpbox, and having the idle color and hiding the helpbox
    {
        displaying = displaying == true ? false : true;
        if(HelpBox != null){HelpBox.SetActive(displaying);}
        this.GetComponent<Image>().color = displaying == true ? activeColor : idleColor;
    }
}
