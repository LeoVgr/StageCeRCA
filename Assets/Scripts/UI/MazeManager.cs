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
    public GameObject BreakForceField;
    public GameObject IsRemySelectedToggle;
    public GameObject IsMeganSelectedToggle;
    public GameObject IsDogSelectedToggle;
    public GameObject IsAutoModeToggle;
    public GameObject IsSemiAutoModeToggle;
    public GameObject IsManualModeToggle;
    public GameObject IdPlayerField;
    public GameObject MusicVolumeSlider;
    public GameObject SfxVolumeSlider;
    public GameObject IsCrosshairColorizedToggle;

    private List<UIDataManager> _dataManagers;
    private FillPresets _fillPreset;
    private bool _isDone = false;

    #endregion

    #region "Events"
    private void Start()
    {
        _isDone = false;

        _fillPreset = GetComponentInChildren<FillPresets>();

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
            BreakForceField.GetComponent<UIDataManager>(),
            FpsCameraToggle.GetComponent<UIDataManager>(),
            TimerField.GetComponent<UIDataManager>(),
            IsRemySelectedToggle.GetComponent<UIDataManager>(),
            IsMeganSelectedToggle.GetComponent<UIDataManager>(),
            IsDogSelectedToggle.GetComponent<UIDataManager>(),
            IsAutoModeToggle.GetComponent<UIDataManager>(),
            IsSemiAutoModeToggle.GetComponent<UIDataManager>(),
            IsManualModeToggle.GetComponent<UIDataManager>(),
            MusicVolumeSlider.GetComponent<UIDataManager>(),
            SfxVolumeSlider.GetComponent<UIDataManager>(),
            IsCrosshairColorizedToggle.GetComponent<UIDataManager>()
        };

        //Corridor Length
        LengthCorridorSlider.GetComponent<UIDataManager>().minInt = (int) DataManager.instance.MinMaxLength.Value.x;
        LengthCorridorSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxLength.Value.y;
        LengthCorridorSlider.GetComponent<UIDataManager>().AtomVariableInt = DataManager.instance.CorridorLength;
        LengthCorridorSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Corridor Width
        WidthCorridorSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxSize.Value.x;
        WidthCorridorSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxSize.Value.y;
        WidthCorridorSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.CorridorWidth;
        WidthCorridorSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Image Size
        ImageSizeSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxImageSizeSlider.Value.x;
        ImageSizeSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxImageSizeSlider.Value.y;
        ImageSizeSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.ImageSize;
        ImageSizeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Wall Height Slider
        WallHeightSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxHeightSlider.Value.x;
        WallHeightSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxHeightSlider.Value.y;
        WallHeightSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.WallHeight;
        WallHeightSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Turn number
        TurnNumberSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxTurnSlider.Value.x;
        TurnNumberSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxTurnSlider.Value.y;
        TurnNumberSlider.GetComponent<UIDataManager>().AtomVariableInt = DataManager.instance.TurnNumber;
        TurnNumberSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Toggle random image
        RandomImageToggle.GetComponent<UIDataManager>().minInt = 0;
        RandomImageToggle.GetComponent<UIDataManager>().maxInt = 0;
        RandomImageToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.RandomizeImage;
        RandomImageToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle Fire
        PlayerCanFireToggle.GetComponent<UIDataManager>().minInt = 0;
        PlayerCanFireToggle.GetComponent<UIDataManager>().maxInt = 0;
        PlayerCanFireToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsShootActivated;
        PlayerCanFireToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Show end time
        ShowEndTimeToggle.GetComponent<UIDataManager>().minInt = 0;
        ShowEndTimeToggle.GetComponent<UIDataManager>().maxInt = 0;
        ShowEndTimeToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.ShowEndTime;
        ShowEndTimeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Speed
        SpeedField.GetComponent<UIDataManager>().minInt = 0;
        SpeedField.GetComponent<UIDataManager>().maxInt = 0;
        SpeedField.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.Speed;
        SpeedField.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Break force
        BreakForceField.GetComponent<UIDataManager>().minInt = 0;
        BreakForceField.GetComponent<UIDataManager>().maxInt = 0;
        BreakForceField.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.BreakForce;
        BreakForceField.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Display Score
        DisplayScoreToggle.GetComponent<UIDataManager>().minInt = 0;
        DisplayScoreToggle.GetComponent<UIDataManager>().maxInt = 0;
        DisplayScoreToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.DisplayScore;
        DisplayScoreToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Image time
        ImageTimeSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxImagesTime.Value.x;
        ImageTimeSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxImagesTime.Value.y;
        ImageTimeSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.ImageTime;
        ImageTimeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;
        
        //Toggle Camera
        FpsCameraToggle.GetComponent<UIDataManager>().minInt = 0;
        FpsCameraToggle.GetComponent<UIDataManager>().maxInt = 0;
        FpsCameraToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.FpsCamera;
        FpsCameraToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; //false
        
        //Timer 
        TimerField.GetComponent<UIDataManager>().minInt = 0;
        TimerField.GetComponent<UIDataManager>().maxInt = 0;
        TimerField.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.Timer;
        TimerField.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        //Seed
        SeedField.GetComponent<UIDataManager>().minInt = 0;
        SeedField.GetComponent<UIDataManager>().maxInt = 0;
        SeedField.GetComponent<UIDataManager>().AtomVariableInt = DataManager.instance.Seed;
        SeedField.GetComponent<UIDataManager>().RegisterThisEvent = true; 

        //Toggle remy selected
        IsRemySelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsRemySelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsRemySelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsRemySelected;
        IsRemySelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle megan selected
        IsMeganSelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsMeganSelected;
        IsMeganSelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle Dog selected
        IsDogSelectedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsDogSelectedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsDogSelectedToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsDogSelected;
        IsDogSelectedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle auto mode
        IsAutoModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsAutoModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsAutoModeToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsAutoMode;
        IsAutoModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle semi auto mode
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsSemiAutoMode;
        IsSemiAutoModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Toggle manual mode
        IsManualModeToggle.GetComponent<UIDataManager>().minInt = 0;
        IsManualModeToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsManualModeToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsManualMode;
        IsManualModeToggle.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Music volume
        MusicVolumeSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxMusicVolume.Value.x;
        MusicVolumeSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxMusicVolume.Value.y;
        MusicVolumeSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.MusicVolume;
        MusicVolumeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Music volume
        SfxVolumeSlider.GetComponent<UIDataManager>().minInt = (int)DataManager.instance.MinMaxMusicVolume.Value.x;
        SfxVolumeSlider.GetComponent<UIDataManager>().maxInt = (int)DataManager.instance.MinMaxMusicVolume.Value.y;
        SfxVolumeSlider.GetComponent<UIDataManager>().AtomVariableFloat = DataManager.instance.SfxVolume;
        SfxVolumeSlider.GetComponent<UIDataManager>().RegisterThisEvent = true;

        //Is crosshair colorized
        IsCrosshairColorizedToggle.GetComponent<UIDataManager>().minInt = 0;
        IsCrosshairColorizedToggle.GetComponent<UIDataManager>().maxInt = 0;
        IsCrosshairColorizedToggle.GetComponent<UIDataManager>().AtomVariableBool = DataManager.instance.IsCrosshairColorized;
        IsCrosshairColorizedToggle.GetComponent<UIDataManager>().RegisterThisEvent = true; // false

        IdPlayerField.GetComponent<InputUIManager>().atomVariableString = DataManager.instance.IdPlayer;       
        
        ResetVariable();
        
        InitUI();

        
    }
    private void OnDestroy()
    {
        _dataManagers.Clear();
    }
    private void Update()
    {
        //Allow us to fill preset after every start methods of the scene (because update is called after start)
        if (!_isDone)
        {
            _isDone = true;
            _fillPreset.ImportPreset(true);
        }
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
        DataManager.instance.ResetAllVariable();
    }
    #endregion
}
