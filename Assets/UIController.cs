using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject showUIButton, hideUIButton;
    public GameObject MainMenuUI,GenerateGridUI,GenerateTerrainUI;
    public enum UIPage
    {
        HIDDEN,
        MAIN_MENU,
        GENERATE_GRID,
        GENERATE_TERRAIN,
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
        switch(currPage)
        {
            case UIPage.HIDDEN:
                break;
            case UIPage.MAIN_MENU:
                ShowUI();
                MainMenuUI.SetActive(true);
                break;
            case UIPage.GENERATE_GRID:
                ShowUI();
                GenerateGridUI.SetActive(true);
                break;
            case UIPage.GENERATE_TERRAIN:
                ShowUI();   
                GenerateTerrainUI.SetActive(true);
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
    }
    public void ShowUI()
    {
        hideUIButton.SetActive(true);
        showUIButton.SetActive(false);
    }
}
