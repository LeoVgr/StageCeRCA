using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region "Attributs"
    public GameObject ReadyUI;
    public GameObject EndGameUI;
    public GameObject TimerUI;
    public GameObject ScoreUI;
    public GameObject PauseUI;
    public GameObject CrosshairUI;
    public GameObject LoadingUI;

    public GameObject ReadyAnswerText;
    public GameObject CountDownText;   
    public GameObject MainPauseUI;
    public GameObject OptionsPauseUI;
    public Slider SensibilitySlider;

    private float _lastFullSeconds = 4f;

    #endregion

    #region "Events"
    private void Start()
    {
        //EndGameUI.SetActive(false);
        //TimerUI.SetActive(true);
        //ScoreUI.SetActive(false);
        //PauseUI.SetActive(false);
        //CrosshairUI.SetActive(true);
    }
    #endregion"

    #region "Methods"
    /* Main UI elements */
    public void ShowLoadingUI(bool displayUI)
    {
        LoadingUI.SetActive(displayUI);
    }
    public void ShowInGameUI(bool displayUI)
    {
        CrosshairUI.SetActive(displayUI);
        TimerUI.SetActive(displayUI);
        ScoreUI.SetActive(displayUI);
      
    }
    public void ShowReadyUI(bool displayUI)
    {
        ReadyUI.SetActive(displayUI);
        ReadyAnswerText.SetActive(displayUI);
        CountDownText.SetActive(!displayUI);
    }
    public void ShowEndGameUI(bool displayUI)
    {
        EndGameUI.SetActive(displayUI);
    }
    public void ShowPauseUI(bool displayUI)
    {
        PauseUI.SetActive(displayUI);
        MainPauseUI.SetActive(displayUI);
        OptionsPauseUI.SetActive(!displayUI);
    }
    public void ShowOptionsPauseUI(bool displayUI)
    {
        MainPauseUI.SetActive(!displayUI);
        OptionsPauseUI.SetActive(displayUI);

        //Update slider's value
        SensibilitySlider.value = InputManager.instance.GetSensibility();
    }

    public void HideCountDownScreen()
    {
        ReadyUI.SetActive(false);
    }
    public void StartCountDown()
    {
        ReadyAnswerText.SetActive(false);
        CountDownText.SetActive(true);
    }
    public void UpdateCountDown(string value)
    {
        if(float.Parse(value) < _lastFullSeconds)
        {
            _lastFullSeconds -= 1;

            CountDownText.GetComponent<TextMeshProUGUI>().text = value;
            CountDownText.transform.DOPunchScale(CountDownText.transform.localScale * 0.2f, 0.3f);
            //CountDownText.DOColor(Color.red, 0.3f).OnComplete(() => TimerText.DOColor(Color.white, 0.3f));
        }
        
    }
    
    public void HideOptionsPauseUI()
    {
        MainPauseUI.SetActive(true);
        OptionsPauseUI.SetActive(false);
        PauseUI.SetActive(true);
        TimerUI.SetActive(false);
        CrosshairUI.SetActive(false);
    }
    public void ShowEndGameScreen(bool isLost)
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
    #endregion
}
