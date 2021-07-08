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

    [Header("GameObject")]
    public GameObject lengthSlider;
    public GameObject sizeSlider;
    public GameObject imageSizeSlider;
    public GameObject heightSlider;
    public GameObject nbrTurnSlider;
    public GameObject toggleRandomImage;
    public GameObject toggleDisplayScore;
    public GameObject togglePlayerCanFire;
    public GameObject toggleCameraFPS;
    public GameObject imageTime;
    public GameObject seed;
    public GameObject timer;
    public GameObject id_player;

    [Header("Scriptable object references")]
    public FloatVariable corridorSize;
    public FloatVariable imageSize;
    public FloatVariable wallHeight;
    public IntVariable corridorLength;
    public IntVariable seedNumber;
    public IntVariable turnNumber;
    public FloatVariable imageTimeVariable;
    public BoolVariable randomizeImage;
    public BoolVariable displayScore;
    public BoolVariable playerCanFire;
    public BoolVariable cameraFPS;
    public FloatVariable timerVariable;
    public StringVariable id_playerVariable;

    [Header("Min max")]
    public Vector2Constant minMaxLength;
    public Vector2Constant minMaxSize;
    public Vector2Constant minMaxImageSizeSlider;
    public Vector2Constant minMaxHeightSlider;
    public Vector2Variable minMaxTurnSlider;
    public Vector2Constant minMaxImagesTime;


    private List<UIDataManager> _dataManagers;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _dataManagers = new List<UIDataManager>
        {
            lengthSlider.GetComponent<UIDataManager>(),
            sizeSlider.GetComponent<UIDataManager>(),
            imageSizeSlider.GetComponent<UIDataManager>(),
            heightSlider.GetComponent<UIDataManager>(),
            nbrTurnSlider.GetComponent<UIDataManager>(),
            seed.GetComponent<UIDataManager>(),
            toggleRandomImage.GetComponent<UIDataManager>(),
            toggleDisplayScore.GetComponent<UIDataManager>(),
            imageTime.GetComponent<UIDataManager>(),
            id_player.GetComponent<InputUIManager>(), 
            togglePlayerCanFire.GetComponent<UIDataManager>(),
            toggleCameraFPS.GetComponent<UIDataManager>(),
            timer.GetComponent<UIDataManager>()
        };

        //Corridor Length
        lengthSlider.GetComponent<UIDataManager>().minInt = (int) minMaxLength.Value.x;
        lengthSlider.GetComponent<UIDataManager>().maxInt = (int) minMaxLength.Value.y;
        lengthSlider.GetComponent<UIDataManager>().atomVariableInt = corridorLength;
        lengthSlider.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Corridor Size
        sizeSlider.GetComponent<UIDataManager>().minInt = (int) minMaxSize.Value.x;
        sizeSlider.GetComponent<UIDataManager>().maxInt = (int) minMaxSize.Value.y;
        sizeSlider.GetComponent<UIDataManager>().atomVariableFloat = corridorSize;
        sizeSlider.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Image Size
        imageSizeSlider.GetComponent<UIDataManager>().minInt = (int) minMaxImageSizeSlider.Value.x;
        imageSizeSlider.GetComponent<UIDataManager>().maxInt = (int) minMaxImageSizeSlider.Value.y;
        imageSizeSlider.GetComponent<UIDataManager>().atomVariableFloat = imageSize;
        imageSizeSlider.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Height Slider
        heightSlider.GetComponent<UIDataManager>().minInt = (int) minMaxHeightSlider.Value.x;
        heightSlider.GetComponent<UIDataManager>().maxInt = (int) minMaxHeightSlider.Value.y;
        heightSlider.GetComponent<UIDataManager>().atomVariableFloat = wallHeight;
        heightSlider.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Turn number
        nbrTurnSlider.GetComponent<UIDataManager>().minInt = (int) minMaxTurnSlider.Value.x;
        nbrTurnSlider.GetComponent<UIDataManager>().maxInt = (int) minMaxTurnSlider.Value.y;
        nbrTurnSlider.GetComponent<UIDataManager>().atomVariableInt = turnNumber;
        nbrTurnSlider.GetComponent<UIDataManager>().registerThisEvent = true;

        //seed
        seed.GetComponent<UIDataManager>().minInt = (int) minMaxTurnSlider.Value.x;
        seed.GetComponent<UIDataManager>().maxInt = (int) minMaxTurnSlider.Value.y;
        seed.GetComponent<UIDataManager>().atomVariableInt = seedNumber;
        seed.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Toggle image
        toggleRandomImage.GetComponent<UIDataManager>().minInt = 0;
        toggleRandomImage.GetComponent<UIDataManager>().maxInt = 0;
        toggleRandomImage.GetComponent<UIDataManager>().atomVariableBool = randomizeImage;
        toggleRandomImage.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Toggle fire
        togglePlayerCanFire.GetComponent<UIDataManager>().minInt = 0;
        togglePlayerCanFire.GetComponent<UIDataManager>().maxInt = 0;
        togglePlayerCanFire.GetComponent<UIDataManager>().atomVariableBool = playerCanFire;
        togglePlayerCanFire.GetComponent<UIDataManager>().registerThisEvent = false;

        //Display Score
        toggleDisplayScore.GetComponent<UIDataManager>().minInt = 0;
        toggleDisplayScore.GetComponent<UIDataManager>().maxInt = 0;
        toggleDisplayScore.GetComponent<UIDataManager>().atomVariableBool = displayScore;
        toggleDisplayScore.GetComponent<UIDataManager>().registerThisEvent = false;

        //Image time
        imageTime.GetComponent<UIDataManager>().minInt = (int) minMaxImagesTime.Value.x;
        imageTime.GetComponent<UIDataManager>().maxInt = (int) minMaxImagesTime.Value.y;
        imageTime.GetComponent<UIDataManager>().atomVariableFloat = imageTimeVariable;
        imageTime.GetComponent<UIDataManager>().registerThisEvent = true;
        
        //Toggle Camera
        toggleCameraFPS.GetComponent<UIDataManager>().minInt = 0;
        toggleCameraFPS.GetComponent<UIDataManager>().maxInt = 0;
        toggleCameraFPS.GetComponent<UIDataManager>().atomVariableBool = cameraFPS;
        toggleCameraFPS.GetComponent<UIDataManager>().registerThisEvent = false;
        
        //Timer 
        timer.GetComponent<UIDataManager>().minInt = 0;
        timer.GetComponent<UIDataManager>().maxInt = 0;
        timer.GetComponent<UIDataManager>().atomVariableFloat = timerVariable;
        timer.GetComponent<UIDataManager>().registerThisEvent = false;

        id_player.GetComponent<InputUIManager>().atomVariableString = id_playerVariable;

        ResetVariable();
        
        InitSlider();
    }

    void InitSlider()
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

    private void OnDestroy()
    {
        _dataManagers.Clear();
    }

    private void ResetVariable()
    {
          corridorSize.Reset();
          imageSize.Reset();
          wallHeight.Reset();
          corridorLength.Reset();
          seedNumber.Reset();
          turnNumber.Reset();
          imageTimeVariable.Reset();
          randomizeImage.Reset();
          id_playerVariable.Reset();
    }
}
