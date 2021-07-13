using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : MazeManager is used to manage MazeGenerator parameters thanks to the UI
 * @date : 07/2020
 */
public class MazeManager : MonoBehaviour
{
    #region "Attributs"
    [Header("GameObject")]
    public GameObject LengthCorridorSlider;
    public GameObject WidthCorridorSlider;
    public GameObject ImageSizeSlider;
    public GameObject WallHeightSlider;
    public GameObject TurnNumberSlider;
    public GameObject ImageTimeSlider;
    public GameObject SeedField;
    public GameObject TimerField;
    public GameObject RandomImageToggle;
    public GameObject DisplayScoreToggle;
    public GameObject FpsCameraToggle;
    public GameObject PlayerCanFireToggle;
    public GameObject ShowEndTimeToggle;
    public GameObject SpeedField;
    public GameObject IsRemySelectedToggle;
    public GameObject IsMeganSelectedToggle;
    public GameObject IsMouseySelectedToggle;
    public GameObject IsAutoModeToggle;
    public GameObject IsSemiAutoModeToggle;
    public GameObject IsManualModeToggle;
    public GameObject IdPlayerField;   
    //player selection ?
    //preset input filed ?
    //pour lui size = largeur

    [Header("Scriptable object references")]
    public IntVariable Seed;
    public IntVariable CorridorLength;
    public IntVariable TurnNumber;
    public FloatVariable CorridorWidth;
    public FloatVariable WallHeight;
    public FloatVariable ImageSize;
    public FloatVariable ImageTime;
    public FloatVariable Timer;
    public BoolVariable RandomizeImage;
    public BoolVariable DisplayScore;
    public BoolVariable FpsCamera;
    public BoolVariable IsShootActivated;
    public BoolVariable ShowEndTime;
    public FloatVariable Speed;
    public BoolVariable IsRemySelected;
    public BoolVariable IsMeganSelected;
    public BoolVariable IsMouseySelected;
    public BoolVariable IsAutoMode;
    public BoolVariable IsSemiAutoMode;
    public BoolVariable IsManualMode;
    public StringVariable IdPlayer;

    [Header("Min max")]
    public Vector2Constant MinMaxLength;
    public Vector2Constant MinMaxSize;
    public Vector2Constant MinMaxImageSizeSlider;
    public Vector2Constant MinMaxHeightSlider;
    public Vector2Variable MinMaxTurnSlider;
    public Vector2Constant MinMaxImagesTime;

    private List<UIDataManager> _dataManagers;
    
    #endregion

    #region "Events"
    private void Start()
    {
        //Set the cursor available
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _dataManagers = new List<UIDataManager>
        {
            LengthCorridorSlider.GetComponent<UIDataManager>(),
            WidthCorridorSlider.GetComponent<UIDataManager>(),
            ImageSizeSlider.GetComponent<UIDataManager>(),
            WallHeightSlider.GetComponent<UIDataManager>(),
            TurnNumberSlider.GetComponent<UIDataManager>(),
            SeedField.GetComponent<UIDataManager>(),
            RandomImageToggle.GetComponent<UIDataManager>(),
            DisplayScoreToggle.GetComponent<UIDataManager>(),
            ImageTimeSlider.GetComponent<UIDataManager>(),
            IdPlayerField.GetComponent<InputUIManager>(),
            PlayerCanFireToggle.GetComponent<UIDataManager>(),
            ShowEndTimeToggle.GetComponent<UIDataManager>(),
            SpeedField.GetComponent<UIDataManager>(),
            FpsCameraToggle.GetComponent<UIDataManager>(),
            TimerField.GetComponent<UIDataManager>(),
            IsRemySelectedToggle.GetComponent<UIDataManager>(),
            IsMeganSelectedToggle.GetComponent<UIDataManager>(),
            IsMouseySelectedToggle.GetComponent<UIDataManager>(),
            IsAutoModeToggle.GetComponent<UIDataManager>(),
            IsSemiAutoModeToggle.GetComponent<UIDataManager>(),
            IsManualModeToggle.GetComponent<UIDataManager>()
        };

        //Corridor Length
        LengthCorridorSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxLength.Value.x;
        LengthCorridorSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxLength.Value.y;
        LengthCorridorSlider.GetComponent<UIDataManager>().AtomVariableInt = CorridorLength;
        LengthCorridorSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Corridor Width
        WidthCorridorSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxSize.Value.x;
        WidthCorridorSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxSize.Value.y;
        WidthCorridorSlider.GetComponent<UIDataManager>().AtomVariableFloat = CorridorWidth;
        WidthCorridorSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Image Size
        ImageSizeSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxImageSizeSlider.Value.x;
        ImageSizeSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxImageSizeSlider.Value.y;
        ImageSizeSlider.GetComponent<UIDataManager>().AtomVariableFloat = ImageSize;
        ImageSizeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Wall Height Slider
        WallHeightSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxHeightSlider.Value.x;
        WallHeightSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxHeightSlider.Value.y;
        WallHeightSlider.GetComponent<UIDataManager>().AtomVariableFloat = WallHeight;
        WallHeightSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Turn number
        TurnNumberSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxTurnSlider.Value.x;
        TurnNumberSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxTurnSlider.Value.y;
        TurnNumberSlider.GetComponent<UIDataManager>().AtomVariableInt = TurnNumber;
        TurnNumberSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Toggle random image
        RandomImageToggle.GetComponent<UIDataManager>().minInt = 0;
        RandomImageToggle.GetComponent<UIDataManager>().maxInt = 0;
        RandomImageToggle.GetComponent<UIDataManager>().AtomVariableBool = RandomizeImage;
        RandomImageToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle Fire
        PlayerCanFireToggle.GetComponent<UIDataManager>().minInt = 0;
        PlayerCanFireToggle.GetComponent<UIDataManager>().maxInt = 0;
        PlayerCanFireToggle.GetComponent<UIDataManager>().AtomVariableBool = IsShootActivated;
        PlayerCanFireToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Show end time
        ShowEndTimeToggle.GetComponent<UIDataManager>().minInt = 0;
        ShowEndTimeToggle.GetComponent<UIDataManager>().maxInt = 0;
        ShowEndTimeToggle.GetComponent<UIDataManager>().AtomVariableBool = ShowEndTime;
        ShowEndTimeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Speed
        SpeedField.GetComponent<UIDataManager>().minInt = 0;
        SpeedField.GetComponent<UIDataManager>().maxInt = 0;
        SpeedField.GetComponent<UIDataManager>().AtomVariableFloat = Speed;
        SpeedField.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Display Score
        DisplayScoreToggle.GetComponent<UIDataManager>().minInt = 0;
        DisplayScoreToggle.GetComponent<UIDataManager>().maxInt = 0;
        DisplayScoreToggle.GetComponent<UIDataManager>().AtomVariableBool = DisplayScore;
        DisplayScoreToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Image time
        ImageTimeSlider.GetComponent<UIDataManager>().minInt = (int) MinMaxImagesTime.Value.x;
        ImageTimeSlider.GetComponent<UIDataManager>().maxInt = (int) MinMaxImagesTime.Value.y;
        ImageTimeSlider.GetComponent<UIDataManager>().AtomVariableFloat = ImageTime;
        ImageTimeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Toggle Camera
        FpsCameraToggle.GetComponent<UIDataManager>().minInt = 0;
        FpsCameraToggle.GetComponent<UIDataManager>().maxInt = 0;
        FpsCameraToggle.GetComponent<UIDataManager>().AtomVariableBool = FpsCamera;
        FpsCameraToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; //false
        
        //Timer 
        TimerField.GetComponent<UIDataManager>().minInt = 0;
        TimerField.GetComponent<UIDataManager>().maxInt = 0;
        TimerField.GetComponent<UIDataManager>().AtomVariableFloat = Timer;
        TimerField.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Seed
        SeedField.GetComponent<UIDataManager>().minInt = 0;
        SeedField.GetComponent<UIDataManager>().maxInt = 0;
        SeedField.GetComponent<UIDataManager>().AtomVariableInt = Seed;
        SeedField.GetComponent<UIDataManager>().RegisterThisEvent = true; 

        //Toggle remy selected
        IsRemySelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsRemySelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsRemySelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = IsRemySelected;
        IsRemySelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle megan selected
        IsMeganSelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = IsMeganSelected;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle mousey selected
        IsMouseySelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsMouseySelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsMouseySelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = IsMouseySelected;
        IsMouseySelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle auto mode
        IsAutoModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsAutoModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsAutoModeToggle.GetComponent<UIDataManager>().AtomVariableBool = IsAutoMode;
        IsAutoModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle semi auto mode
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().AtomVariableBool = IsSemiAutoMode;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle manual mode
        IsManualModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsManualModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsManualModeToggle.GetComponent<UIDataManager>().AtomVariableBool = IsManualMode;
        IsManualModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        IdPlayerField.GetComponent<InputUIManager>().atomVariableString = IdPlayer;       
        
        ResetVariable();
        
        InitUI();
    }
    private void OnDestroy()
    {
        _dataManagers.Clear();
    }
    #endregion

    #region "Methods"
    void InitUI()
    {
        foreach (UIDataManager dataManager in _dataManagers)
        {
            if (dataManager)
            {
                dataManager.Init();
                dataManager.SetMinMaxInit();
            }
        }
    }
    private void ResetVariable()
    {
        Seed.Reset();
        Speed.Reset();
        ShowEndTime.Reset();
        CorridorLength.Reset();
        TurnNumber.Reset();
        CorridorWidth.Reset();
        WallHeight.Reset();
        ImageSize.Reset();
        ImageTime.Reset();
        Timer.Reset();
        RandomizeImage.Reset();
        DisplayScore.Reset();
        FpsCamera.Reset();
        IsShootActivated.Reset();
        IdPlayer.Reset();
        IsRemySelected.Reset();
        IsMeganSelected.Reset();
        IsMouseySelected.Reset();
        IsAutoMode.Reset();
        IsSemiAutoMode.Reset();
        IsManualMode.Reset();
    }
    #endregion
}
