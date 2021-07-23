using Cinemachine;
using Data;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Attributs
    public enum GameStatement { Ready, Countdown, Running, Pause, End, None};

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

    private float _countdownDuration = 3.4f;
    private float _countdownTimer = 3.4f;
    private bool _isGameLost = false;
    private bool _isMazeGenerated = false;

    private GameStatement _gameStatement;
    private GameStatement _previousGameStatement;

    #endregion

    #region Events
    public void Start()
    {
        //Reset UI
        UIManager.instance.ShowInGameUI(false);
        UIManager.instance.ShowReadyUI(false);
        UIManager.instance.ShowEndGameUI(false);
        UIManager.instance.ShowPauseUI(false);
        UIManager.instance.ShowLoadingUI(true);

        //Initialize the first state
        _gameStatement = GameStatement.Ready;
        _previousGameStatement = GameStatement.None;
        EnterReadyStatement();       

    }
    private void Update()
    {
        switch (_gameStatement)
        {
            case GameStatement.Ready:
                ReadyStatement();
                break;
            case GameStatement.Countdown:
                CountdownStatement();
                break;
            case GameStatement.Running:
                RunningStatement();
                break;
            case GameStatement.Pause:
                PauseStatement();
                break;
            case GameStatement.End:
                EndStatement();
                break;
        }

        

        ////Check if player press pause button
        //if (InputManager.instance.IsCancelAction() && _isGameRunning)
        //{
        //    Pause();
        //}

        

    }
    private void OnDestroy()
    {
        Cursor.visible = true;
    }
    #endregion

    #region Methods
    /*Statements methods */
    public GameStatement GetGameStatement()
    {
        return _gameStatement;
    }
    private void CallExitPreviousState(GameStatement previousGameState)
    {
        switch (previousGameState)
        {
            case GameStatement.Ready:
                ExitReadyStatement();
                break;

            case GameStatement.Countdown:
                ExitCountdownStatement();
                break;

            case GameStatement.Running:
                ExitRunningStatement();
                break;

            case GameStatement.Pause:
                ExitPauseStatement();
                break;

            case GameStatement.End:
                ExitEndStatement();
                break;
        }
    }
    public void SetReadyStatement()
    {     
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);
        _gameStatement = GameStatement.Ready;
        EnterReadyStatement();
    }
    public void SetCountdownStatement()
    {        
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);
        _gameStatement = GameStatement.Countdown;
        EnterCountdownStatement();
    }
    public void SetRunningStatement()
    {       
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);
        _gameStatement = GameStatement.Running;
        EnterRunningStatement();
    }
    public void SetPauseStatement()
    {     
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);
        _gameStatement = GameStatement.Pause;
        EnterPauseStatement();
    }
    public void SetEndStatement()
    {     
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);
        _gameStatement = GameStatement.End;
        EnterEndStatement();
    }
    public void NextState()
    {
        switch (_gameStatement)
        {
            case GameStatement.Ready:
                SetCountdownStatement();
                break;

            case GameStatement.Countdown:
                SetRunningStatement();
                break;

            case GameStatement.Running:
                SetPauseStatement();
                break;

            case GameStatement.Pause:
                SetEndStatement();
                break;

            case GameStatement.End:
                SetReadyStatement();
                break;
        }
    }
    
    public void EnterReadyStatement()
    {
        //Reset time scale
        Time.timeScale = 1f;

        //Display ready UI
        UIManager.instance.ShowReadyUI(true);

        //Lock the player because he don't need to move when he is in menu
        InputManager.instance.DisableMovementInputs();

        //Reset values of scriptable objects
        Score.Reset(true);
        TargetCount.SetValue(0);
        TargetHit.SetValue(0);
        TargetList.Clear();       

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

        //Reset some values
        _isMazeGenerated = false;
        _isGameLost = false;
    }
    public void EnterCountdownStatement()
    {
        //Reset timer
        _countdownTimer = _countdownDuration;

        //Show the good UI
        UIManager.instance.ShowReadyUI(true);
        UIManager.instance.StartCountDown();
    }
    public void EnterRunningStatement()
    {
        //Display ready UI
        UIManager.instance.ShowInGameUI(true);

        //Alow the player to move
        InputManager.instance.EnableInputs();
        InputManager.instance.EnableMovementInputs();
    }
    public void EnterPauseStatement()
    {
        //Change timescale
        Time.timeScale = 0f;

        //Display ready UI
        UIManager.instance.ShowOptionsPauseUI(true);
        UIManager.instance.ShowPauseUI(true);

        //Alow the player to move
        InputManager.instance.DisableInputs();
    }
    public void EnterEndStatement()
    {
        //Lock the player
        InputManager.instance.DisableInputs();

        //Display end game screen
        UIManager.instance.ShowEndGameScreen(_isGameLost);

        //Save player's data
        PlayerSaveData.EndGame();
    }

    public void ReadyStatement()
    {
        //Lock the cursor to the game window
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //While the maze isn't generated (due to loading image before generate the maze) just try to generate again
        if (!_isMazeGenerated)
        {
            _isMazeGenerated = MazeGenerator.GenerateMaze();
            UIManager.instance.ShowLoadingUI(!_isMazeGenerated);
        }

        //Check if the player is ready
        if (_isMazeGenerated && InputManager.instance.IsFireAction())
        {
            SetCountdownStatement();
        }

        //Check if the player want to pause the game
        if (InputManager.instance.IsCancelAction())
        {
            SetPauseStatement();
        }
    }
    public void CountdownStatement()
    {
        //Lock the cursor to the game window
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //Update Timer countdown
        _countdownTimer -= Time.deltaTime;

        //Update UI
        UIManager.instance.UpdateCountDown(_countdownTimer.ToString("0"));

        if (_countdownTimer < 0)
            SetRunningStatement();

    }
    public void RunningStatement()
    {
        //Lock the cursor to the game window
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //Check if the player want to pause the game
        if (InputManager.instance.IsCancelAction())
        {
            SetPauseStatement();
        }
    }
    public void PauseStatement()
    {     

        //Lock the cursor to the game window
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void EndStatement()
    {
        //Lock the cursor to the game window
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitReadyStatement()
    {
        //Hide the UI
        UIManager.instance.ShowReadyUI(false);
    }
    public void ExitCountdownStatement()
    {
        //Hide the UI
        UIManager.instance.ShowReadyUI(false);
    }
    public void ExitRunningStatement()
    {
        //Hide the UI
        UIManager.instance.ShowInGameUI(false);
    }
    public void ExitPauseStatement()
    {
        //Change timescale
        Time.timeScale = 1f;

        //Hide UI
        UIManager.instance.ShowPauseUI(false);

        //Enbale inputs
        InputManager.instance.EnableInputs();
    }
    public void ExitEndStatement()
    {
        //Hide the UI
        UIManager.instance.ShowEndGameUI(false);
    }


    /* Other methods */
    public void RestartGame()
    {
        //Exit the state machine 
        _previousGameStatement = _gameStatement;
        CallExitPreviousState(_previousGameStatement);

        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);  
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        //Reset TimeScale
        Time.timeScale = 1f;

        //Change Game Statement (we got a switch here because we might want to handle more than 2 cases)
        switch (_previousGameStatement)
        {
            case GameStatement.Ready:
                SetReadyStatement();
                break;
            case GameStatement.Running:
                SetRunningStatement();
                break;
        }
    }
    public void EndGame(bool isLost)
    {
        if(GetGameStatement() != GameStatement.End)
        {
            //Update game status
            _isGameLost = isLost;

            //Change state
            SetEndStatement();
        }
    }
    #endregion
}