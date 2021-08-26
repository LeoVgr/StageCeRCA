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
    public GameObject TipUI;

    public GameObject KeyImage;
    public GameObject ButtonImage;
    public TextMeshProUGUI TipText;
    public Sprite SplashSprite;
    public GameObject CountDownText;   
    public GameObject MainPauseUI;
    public GameObject OptionsPauseUI;
    public GameObject ParametersPauseUI;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SfxVolumeText;
    public Slider SensibilitySlider;
    public Slider MusicVolumeSlider;
    public Slider SfxVolumeSlider;

    [Header("Value parameters")]
    public TextMeshProUGUI CorridorLenghtText;
    public TextMeshProUGUI CorridorWidthText;
    public TextMeshProUGUI WallHeightText;
    public TextMeshProUGUI TurnNumberText;
    public TextMeshProUGUI SeedText;
    public TextMeshProUGUI DisplayScoreText;
    public TextMeshProUGUI DisplayEndTimerText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ImageTimeText;
    public TextMeshProUGUI ImageSizeText;
    public TextMeshProUGUI RandomImageText;
    public TextMeshProUGUI ShotActivatedText;
    public TextMeshProUGUI FpsViewText;
    public TextMeshProUGUI ColorizedCrosshairText;
    public TextMeshProUGUI MovementModeText;
    public TextMeshProUGUI CharacterText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI BreakText;

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
        UpdateParametersPauseMenu();

        if (displayUI)
        {
            MainPauseUI.GetComponent<Animator>().SetBool("IsOpen",true);
        }
              
    }
    public void ShowOptionsPauseUI(bool displayUI)
    {
        //MainPauseUI.SetActive(!displayUI);
        OptionsPauseUI.SetActive(displayUI);
        ParametersPauseUI.SetActive(!displayUI);

        //Update slider's value
        MusicVolumeSlider.value = DataManager.instance.MusicVolume.Value;
        SfxVolumeSlider.value = DataManager.instance.SfxVolume.Value;
        MusicVolumeText.text = DataManager.instance.MusicVolume.Value.ToString("0.00");
        SfxVolumeText.text = DataManager.instance.SfxVolume.Value.ToString("0.00");
        SensibilitySlider.value = InputManager.instance.GetSensibility();      
    }
    public void ShowParametersPauseUI(bool displayUI)
    {
        //MainPauseUI.SetActive(!displayUI);
        OptionsPauseUI.SetActive(!displayUI);
        ParametersPauseUI.SetActive(displayUI);
    }
    public void ShowTipUI(bool displayUI)
    {
        TipUI.SetActive(displayUI);
        //Enable right image depends on what controller player is using
        ButtonImage.SetActive(InputManager.instance.IsUsingGamepad());
        KeyImage.SetActive(!InputManager.instance.IsUsingGamepad());

    }

    public void UpdateParametersPauseMenu()
    {
        CorridorLenghtText.text = "" + DataManager.instance.CorridorLength.Value;
        CorridorWidthText.text = "" + DataManager.instance.CorridorWidth.Value;
        WallHeightText.text = "" + DataManager.instance.WallHeight.Value;
        TurnNumberText.text = "" + DataManager.instance.TurnNumber.Value;
        SeedText.text = "" + DataManager.instance.Seed.Value;
        DisplayScoreText.text = "" + (DataManager.instance.DisplayScore.Value? "Oui" : "Non");
        DisplayEndTimerText.text = "" + (DataManager.instance.ShowEndTime.Value? "Oui" : "Non");
        TimeText.text = "" + DataManager.instance.Timer.Value;
        ImageTimeText.text = "" + DataManager.instance.ImageTime.Value;
        ImageSizeText.text = "" + DataManager.instance.ImageSize.Value;
        RandomImageText.text = "" + (DataManager.instance.RandomizeImage.Value ? "Oui" : "Non");
        ShotActivatedText.text = "" + (DataManager.instance.IsShootActivated.Value ? "Oui" : "Non");
        FpsViewText.text = "" + (DataManager.instance.FpsCamera.Value ? "Oui" : "Non");
        ColorizedCrosshairText.text = "" + (DataManager.instance.IsCrosshairColorized.Value ? "Oui" : "Non");
        MovementModeText.text = "" + (DataManager.instance.IsAutoMode.Value ? "Auto" : (DataManager.instance.IsManualMode.Value ? "Manuel" : "Semi-Auto"));
        CharacterText.text = "" + (DataManager.instance.IsMeganSelected.Value? "Megan" : (DataManager.instance.IsRemySelected.Value? "Remy" : "Dog"));
        SpeedText.text = "" + DataManager.instance.Speed.Value;
        BreakText.text = "" + DataManager.instance.BreakForce.Value;
    }
    public void HideCountDownScreen()
    {
        ReadyUI.SetActive(false);
    }
    public void StartCountDown()
    {
        TipUI.SetActive(false);
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
        Color[] colors = { new Color(0.25f,0.89f,0.67f), new Color(0.89f, 0.83f, 0.21f), new Color(1, 0.48f, 0.36f) };
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
