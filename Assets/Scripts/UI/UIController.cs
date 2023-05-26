using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject showUIButton, hideUIButton;
    public GameObject MainMenuUI,GenerateGridUI,GenerateTerrainUI, RoomDistributionUI, RoomShapeUI,RoomDetailsUI;
    public enum UIPage
    {
        HIDDEN,
        MAIN_MENU,
        GENERATE_GRID,
        GENERATE_TERRAIN,
        ROOM_DISTRIBUTION,
        ROOM_SHAPE,
        ROOM_DETAILS
    }
    private UIPage currPage;
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
        HideUI();
    }
    public void GoToPage(int _pageNum)
    {
        currPage = (UIPage)_pageNum;
        RefreshPage();
    }
    private void RefreshPage()
    {
        HideUI();
        ShowUI();
        switch(currPage)
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
        }
    }
    public void HideUI()
    {
        showUIButton.SetActive(true);
        hideUIButton.SetActive(false);
        MainMenuUI.SetActive(false);
        GenerateGridUI.SetActive(false);
        GenerateTerrainUI.SetActive(false);
        RoomDistributionUI.SetActive(false);
        RoomShapeUI.SetActive(false);
        RoomDetailsUI.SetActive(false);
    }
    public void ShowUI()
    {
        hideUIButton.SetActive(true);
        showUIButton.SetActive(false);
    }
}
