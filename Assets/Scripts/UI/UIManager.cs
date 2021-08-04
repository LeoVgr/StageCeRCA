using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region "Attributs"
    public Canvas Canvas;
    public GameObject ReadyUI;
    public GameObject EndGameUI;
    public GameObject TimerUI;
    public GameObject ScoreUI;
    public GameObject PauseUI;
    public GameObject CrosshairUI;
    public GameObject LoadingUI;
    public GameObject HurtScreen;

    public Sprite SplashSprite;
    public GameObject ReadyAnswerText;
    public GameObject CountDownText;   
    public GameObject MainPauseUI;
    public GameObject OptionsPauseUI;
    public GameObject ParametersPauseUI;
    public Slider SensibilitySlider;

    private float _lastFullSeconds = 4f;

    #endregion

    #region "Events"
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
        OptionsPauseUI.SetActive(displayUI);
    }
    public void ShowOptionsPauseUI(bool displayUI)
    {
        //MainPauseUI.SetActive(!displayUI);
        OptionsPauseUI.SetActive(displayUI);
        ParametersPauseUI.SetActive(!displayUI);

        //Update slider's value
        SensibilitySlider.value = InputManager.instance.GetSensibility();
    }
    public void ShowParametersPauseUI(bool displayUI)
    {
        //MainPauseUI.SetActive(!displayUI);
        OptionsPauseUI.SetActive(!displayUI);
        ParametersPauseUI.SetActive(displayUI);
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
    public void AddImageToHurtScreen(int x, int y)
    {
        GameObject splash = new GameObject();
        splash.AddComponent<CanvasRenderer>();
        Image splashImage = splash.AddComponent<Image>();
        splash.AddComponent<Ink>();

        splashImage.sprite = SplashSprite;

        //Random color
        Color[] colors = { Color.red, Color.blue, Color.green };
        splashImage.color = colors[Random.Range(0, 3)];

        splash.transform.parent = HurtScreen.transform;
        splash.transform.position = Vector3.zero;
        splash.transform.rotation = Quaternion.identity;
        splash.transform.localScale = Vector3.one;
        splash.transform.localScale = new Vector3(5,5,5);
        splash.transform.localPosition = new Vector3(x, y, 0);
        
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
