using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region "Attributs"
    public VictoryScreen VictoryScreen;
    public IntVariable Score;
    public BoolVariable IsGameStart;
    public BoolVariable FpsCamera;
    public BoolVariable IsPlayerLock;
    public BoolVariable IsGameStarted;
    public GameObjectVariable Player;
    public BoolVariable IsRemySelected;
    public BoolVariable IsMeganSelected;
    public BoolVariable IsMouseySelected;

    public GameObject Remy;
    public CinemachineVirtualCamera RemyFPSCamera;
    public CinemachineFreeLook RemyTPSCamera;
    public GameObject Megan;
    public CinemachineVirtualCamera MeganFPSCamera;
    public CinemachineFreeLook MeganTPSCamera;
    public GameObject Mousey;
    public CinemachineVirtualCamera MouseyFPSCamera;
    public CinemachineFreeLook MouseyTPSCamera;

    private MazeGenerator _mazeGenerator;
    private bool _isMazeGenerated = false;
    #endregion

    #region "Events"
    void Start()
    {
        //Get references to other components
        _mazeGenerator = this.GetComponent<MazeGenerator>();

        //Lock the cursor to the game window
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //Enable the right character
        Remy.SetActive(IsRemySelected.Value);
        Megan.SetActive(IsMeganSelected.Value);
        Mousey.SetActive(IsMouseySelected.Value);

        if (IsRemySelected.Value)
        {
            Player.SetValue(Remy.GetComponentInChildren<PlayerMovement>().gameObject);

            //Activate the right camera
            if (FpsCamera.Value)
            {
                RemyFPSCamera.Priority = 1;
                RemyTPSCamera.Priority = 0;
            }
            else
            {
                RemyFPSCamera.Priority = 0;
                RemyTPSCamera.Priority = 1;
            }
            
        }

        if (IsMeganSelected.Value)
        {
            Player.SetValue(Megan.GetComponentInChildren<PlayerMovement>().gameObject);

            //Activate the right camera
            if (FpsCamera.Value)
            {
                MeganFPSCamera.Priority = 1;
                MeganTPSCamera.Priority = 0;
            }
            else
            {
                MeganFPSCamera.Priority = 0;
                MeganTPSCamera.Priority = 1;
            }
        }

        if (IsMouseySelected.Value)
        {
            Player.SetValue(Mousey.GetComponentInChildren<PlayerMovement>().gameObject);

            //Activate the right camera
            if (FpsCamera.Value)
            {
                MeganFPSCamera.Priority = 1;
                MeganTPSCamera.Priority = 0;
            }
            else
            {
                MeganFPSCamera.Priority = 0;
                MeganTPSCamera.Priority = 1;
            }
        }

    }
    private void Update()
    {
        //While the maze isn't generated (due to loading image before generate the maze) just try to generate again
        if (!_isMazeGenerated)
        {
            _isMazeGenerated = _mazeGenerator.GenerateMaze();

            IsGameStarted.SetValue(true);
            IsPlayerLock.SetValue(false);

        }
    }
    #endregion

    #region "Methods"
    public void RestartGame()
    {
        
        if (VictoryScreen)
        {
            VictoryScreen.HideScreen();
        }
        else
        {
            Player.Value.GetComponent<PlayerMovement>().ShowMenu();
        }
        
        Player.Value.GetComponent<PlayerMovement>().Restart();
        IsPlayerLock.Value = true;

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        Score.Reset(true);
        IsGameStart.Reset(true);

        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Exit()
    {
        Application.Quit();
    }
    #endregion
}
