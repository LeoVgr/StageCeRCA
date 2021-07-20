using Cinemachine;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Attributs

    public VictoryScreen VictoryScreen;
    public IntVariable Score;
    public BoolVariable IsGameStart;
    public BoolVariable FpsCamera;
    public BoolVariable IsPlayerLock;
    public BoolVariable IsGameStarted;
    public GameObjectVariable PlayerVariable;
    public BoolVariable IsRemySelected;
    public BoolVariable IsMeganSelected;
    public BoolVariable IsMouseySelected;

    public PlayerMovement Player;

    public GameObject Remy;
    public GameObject Megan;
    public GameObject Mousey;

    public CinemachineVirtualCamera FPSCamera;
    public CinemachineVirtualCamera TPSCamera;

    public MazeGenerator MazeGenerator;
    private bool _isMazeGenerated = false;

    #endregion

    #region Events
    void Start()
    {
        //Lock the cursor to the game window
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;


        PlayerVariable.SetValue(Player.gameObject);

        //Activate the right camera
        if (FpsCamera.Value)
        {
            Remy.SetActive(false);
            Megan.SetActive(false);
            Mousey.SetActive(false);
            
            FPSCamera.Priority = 1;
            TPSCamera.Priority = 0;
        }
        else
        {
            //Enable the right character
            Remy.SetActive(IsRemySelected.Value);
            Megan.SetActive(IsMeganSelected.Value);
            Mousey.SetActive(IsMouseySelected.Value);
            
            FPSCamera.Priority = 0;
            TPSCamera.Priority = 1;
        }
    }
    private void Update()
    {
        //While the maze isn't generated (due to loading image before generate the maze) just try to generate again
        if (!_isMazeGenerated)
        {
            _isMazeGenerated = MazeGenerator.GenerateMaze();

            IsGameStarted.SetValue(true);
            IsPlayerLock.SetValue(false);
        }
    }
    #endregion

    #region Methods
    public void RestartGame()
    {
        if (VictoryScreen)
        {
            VictoryScreen.HideScreen();
        }
        else
        {
            Player.ShowMenu();
        }

        //Lock the player because he don't need to move when he is in menu
        IsPlayerLock.Value = true;

        //Reset values of scriptable objects
        Score.Reset(true);
        IsGameStart.Reset(true);
        //TODO Player.Restart();

        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    #endregion
}