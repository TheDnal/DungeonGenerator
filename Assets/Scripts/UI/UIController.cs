using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    /*
        Simple UI state machine that controls the UI for the program
    */
    public GameObject showUIButton, hideUIButton; //Button that controls whether the ui is showing or not
    public GameObject MainMenuUI,GenerateGridUI,GenerateTerrainUI, RoomDistributionUI, RoomShapeUI,RoomDetailsUI, MiscUI; //All UI states / pages
    public enum UIPage //UIPages
    {
        HIDDEN,
        MAIN_MENU,
        GENERATE_GRID,
        GENERATE_TERRAIN,
        ROOM_DISTRIBUTION,
        ROOM_SHAPE,
        ROOM_DETAILS,
        MISC
    }
    private UIPage currPage; //current UI page
    public static UIController instance; //Singleton
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }
    void Start()
    {
        HideUI(); //Hide all ui
    }
    public void GoToPage(int _pageNum) //Set UI state / current page
    {
        currPage = (UIPage)_pageNum;
        RefreshPage();
    }
    private void RefreshPage() //Refresh page
    {
        //hide all pages
        HideUI();
        ShowUI();
        switch(currPage) //show only the required page
        {
            case UIPage.HIDDEN:
                HideUI();
                break;
            case UIPage.MAIN_MENU:
                MainMenuUI.SetActive(true);
                break;
            case UIPage.GENERATE_GRID:
                GenerateGridUI.SetActive(true);
                break;
            case UIPage.GENERATE_TERRAIN:
                GenerateTerrainUI.SetActive(true);
                break;
            case UIPage.ROOM_DISTRIBUTION:
                RoomDistributionUI.SetActive(true);
                break;
            case UIPage.ROOM_SHAPE:
                RoomShapeUI.SetActive(true);
                break;
            case UIPage.ROOM_DETAILS:
                RoomDetailsUI.SetActive(true);
                break;
            case UIPage.MISC:
                MiscUI.SetActive(true);
                break;

        }
    }
    public void HideUI() //hides all the pages
    {
        showUIButton.SetActive(true);
        hideUIButton.SetActive(false);
        MainMenuUI.SetActive(false);
        GenerateGridUI.SetActive(false);
        GenerateTerrainUI.SetActive(false);
        RoomDistributionUI.SetActive(false);
        RoomShapeUI.SetActive(false);
        RoomDetailsUI.SetActive(false);
        MiscUI.SetActive(false);
    }
    public void ShowUI() //Shows the correct button
    {
        hideUIButton.SetActive(true);
        showUIButton.SetActive(false);
    }
}
