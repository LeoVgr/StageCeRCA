using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region "Attributs"
    public GameObject EndGameUI;
    public GameObject TimerUI;
    public GameObject ScoreUI;
    public GameObject PauseUI;
    public GameObject CrosshairUI;
    #endregion

    #region "Events"
    private void Start()
    {
        EndGameUI.SetActive(false);
        TimerUI.SetActive(true);
        ScoreUI.SetActive(false);
        PauseUI.SetActive(false);
        CrosshairUI.SetActive(true);
    }
    #endregion"

    #region "Methods"
    public void ShowPauseUI()
    {
        PauseUI.SetActive(true);
        TimerUI.SetActive(false);
        CrosshairUI.SetActive(false);
    }
    public void HidePauseUI()
    {
        PauseUI.SetActive(false);
        TimerUI.SetActive(true);
        CrosshairUI.SetActive(true);
    }
    public void DisplayEndGameScreen(bool isLost)
    {
        //Display end screen
        EndGameUI.SetActive(true);
        EndGameUI.GetComponent<VictoryScreen>().ShowScreen(isLost);

        //Hide cursor and timer 
        TimerUI.SetActive(false);
        CrosshairUI.SetActive(false);
    }
    public void HideEndGameScreen()
    {
        //Remove end screen
        EndGameUI.SetActive(false);

        //Hide cursor and timer 
        TimerUI.SetActive(true);
        CrosshairUI.SetActive(true);
    }
    public void HidePauseScreen()
    {
        //Remove end screen
        PauseUI.SetActive(false);
    }
    #endregion
}
