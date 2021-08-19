using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [Header("Atom's variables")]
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
    public FloatVariable BreakForce;
    public BoolVariable IsRemySelected;
    public BoolVariable IsMeganSelected;
    public BoolVariable IsDogSelected;
    public BoolVariable IsAutoMode;
    public BoolVariable IsSemiAutoMode;
    public BoolVariable IsManualMode;
    public StringVariable IdPlayer;
    public FloatVariable MusicVolume;
    public FloatVariable SfxVolume;
    public BoolVariable IsCrosshairColorized;
    public IntVariable TargetCount;
    public IntVariable TargetHit;
    public StringVariable PresetName;
    public FloatValueList TimeToShootList;
    public GameObjectValueList TargetList;
    public IntVariable Score;
    public GameObjectVariable Player;
    public FloatVariable DistanceToShow;
    public Vector2Variable ScreenShakesValues;
    public FloatVariable LifeAmount;
    public IntVariable WaypointIndex;
    public BoolVariable IsTutorial;

    [Header("Atoms Events")]
    public IntEvent PlayerWaypointChange;
    public IntEvent UpdateEvent;

    [Header("Min max")]
    public Vector2Constant WidthHeight;
    public Vector2Constant MinMaxLength;
    public Vector2Constant MinMaxSize;
    public Vector2Constant MinMaxImageSizeSlider;
    public Vector2Constant MinMaxHeightSlider;
    public Vector2Variable MinMaxTurnSlider;
    public Vector2Constant MinMaxImagesTime;
    public Vector2Constant MinMaxMusicVolume;

    

    public void ResetAllVariable()
    {     
      Seed.Reset();
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
      ShowEndTime.Reset();
      Speed.Reset();
      BreakForce.Reset();
      IsRemySelected.Reset();
      IsMeganSelected.Reset();
      IsDogSelected.Reset();
      IsAutoMode.Reset();
      IsSemiAutoMode.Reset();
      IsManualMode.Reset();
      IdPlayer.Reset();
      MusicVolume.Reset();
      SfxVolume.Reset();
      IsCrosshairColorized.Reset();
      TargetCount.Reset();
      TargetHit.Reset();
      PresetName.Reset();
      TimeToShootList.Clear();
      TargetList.Clear();
      Score.Reset();
      Player.Reset();
      DistanceToShow.Reset();
      ScreenShakesValues.Reset();
      LifeAmount.Reset();
      WaypointIndex.Reset();
      MinMaxTurnSlider.Reset();
      IsTutorial.Reset();
    }
}
