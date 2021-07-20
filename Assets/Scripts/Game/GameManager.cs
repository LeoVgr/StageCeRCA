using Cinemachine;
using Data;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Attributs
    public IntVariable Score;
    public BoolVariable FpsCamera;
    public IntVariable TargetCount;
    public IntVariable TargetHit;
    public GameObjectValueList TargetList;
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
    public PlayerSaveData PlayerSaveData;
    private bool _isMazeGenerated = false;
    private bool _isGameRunning = false;

    #endregion

    #region Events
    void Start()
    {
        //Reset some values
        _isMazeGenerated = false;
        _isGameRunning = false;

        StartGame();
    }
    private void Update()
    {
        //While the maze isn't generated (due to loading image before generate the maze) just try to generate again
        if (!_isMazeGenerated)
        {
            _isMazeGenerated = MazeGenerator.GenerateMaze();

            _isGameRunning = true;
            InputManager.instance.EnableInputs();
        }

        //Check if player press pause button
        if (InputManager.instance.IsCancelAction() && _isGameRunning)
        {
            Pause();
        }

    }
    private void OnDestroy()
    {
        Cursor.visible = true;
    }
    #endregion

    #region Methods
    public void StartGame()
    {
        if (!_isGameRunning)
        {
            //Lock the cursor to the game window
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            //Get the references of the player
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

            //Reset values
            TargetCount.SetValue(0);
            TargetHit.SetValue(0);
            TargetList.Clear();
        }
    }
    public void RestartGame()
    {
        //Hide UI
        UIManager.instance.HideEndGameScreen();
        UIManager.instance.HidePauseScreen();

        //Lock the player because he don't need to move when he is in menu
        InputManager.instance.DisableInputs();

        //Reset values of scriptable objects
        Score.Reset(true);

        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        //Reset values
        TargetCount.SetValue(0);
        TargetHit.SetValue(0);
        TargetList.Clear();
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Pause()
    {       
        //Stop game
        _isGameRunning = false;
        Time.timeScale = 0;
        InputManager.instance.DisableInputs();

        //Show Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //Show UI
        UIManager.instance.ShowPauseUI();
    }
    public void Resume()
    {
        //Play game
        _isGameRunning = true;
        Time.timeScale = 1;
        InputManager.instance.EnableInputs();

        //Show Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //Show UI
        UIManager.instance.HidePauseUI();
    }
    public void EndGame(bool isLost)
    {
        if (_isGameRunning)
        {
            _isGameRunning = false;

            //Lock the player
            InputManager.instance.DisableInputs();

            //Display end game screen
            UIManager.instance.DisplayEndGameScreen(isLost);

            //Save player's data
            PlayerSaveData.EndGame();
        }
        
    }
    public bool IsGameRunning()
    {
        return _isGameRunning;
    }
    #endregion
}