using Cinemachine;
using Data;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Attributs
    public enum GameStatement
    {
        Ready,
        Countdown,
        Running,
        Pause,
        End,
        None
    };

    public PlayerMovement Player;
    public GameObject Remy;
    public GameObject Megan;
    public GameObject Dog;

    public CinemachineVirtualCamera FPSCamera;
    public CinemachineVirtualCamera TPSCamera;

    public MazeGenerator MazeGenerator;
    public AudioSource MusicManager;
    public PlayerSaveData PlayerSaveData;
    

    private float _countdownDuration = 3.4f;
    private float _countdownTimer = 3.4f;
    private bool _isGameLost = false;
    private bool _isMazeGenerated = false;

    private int _tutorialStep = 0;
    private float _tutorialInstructionMinimumTime = 0.5f;
    private float _tutorialTimer = 0;
    private Vector2 _previousMousePos = Vector2.one;
    private string[] _tutorialInstructions; 

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

        _tutorialInstructions = new string[]
        {
            "Bienvenue dans ce tutoriel, nous allons ensemble voir les bases",
            "Le but du jeu est d'arriver au bout de ce couloir dans le temps imparti tout en marquant des points",
            "Pour cela, tirez sur les images " + (DataManager.instance.IsCrosshairColorized.Value? "lorsque votre viseur devient vert en les survolants" : "que le chercheur vous a décrites"),
            (DataManager.instance.IsCrosshairColorized.Value? "Si votre viseur est rouge, il ne faut pas tirer dessus" : "Sinon il ne faut pas tirer dessus"),
            "Parfois, une tourelle fera son apparaition, empressez vous de la neutraliser",
            (InputManager.instance.IsUsingGamepad()? "Utilisez le joystick droit pour bouger la vue" : "Utilisez la souris pour bouger la vue"),
            (InputManager.instance.IsUsingGamepad()? "Appuyez sur [RT] pour tirer" : "Appuyer sur le clic gauche pour tirer"),
            "Pour déplacer le chariot, "+(InputManager.instance.IsUsingGamepad()? "utilisez le joystick gauche" : "utilisez les touches [Z] et [S]"),
            "Pour faire freiner le chariot, "+(InputManager.instance.IsUsingGamepad()? "appuyez sur [LT]" : "appuyez sur [Espace]"),
            "Est-ce que tu es prêt à commencer ?"
        };
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
        UIManager.instance.ShowTipUI(true);      

        //Lock the player because he don't need to move when he is in menu
        InputManager.instance.DisableMovementInputs();

        //Reset values of scriptable objects
        DataManager.instance.WaypointIndex.SetValue(0,true);
        DataManager.instance.Score.SetValue(0,true);
        DataManager.instance.TargetCount.SetValue(0);
        DataManager.instance.TargetHit.SetValue(0);
        DataManager.instance.TargetList.Clear();
        _tutorialStep = 0;

        //Get the references of the player
        DataManager.instance.Player.SetValue(Player.gameObject);

        //Activate the right camera
        if (DataManager.instance.FpsCamera.Value)
        {
            FPSCamera.Priority = 1;
            TPSCamera.Priority = 0;
        }
        else
        {
            FPSCamera.Priority = 0;
            TPSCamera.Priority = 1;
        }

        //Enable the right character
        Remy.SetActive(DataManager.instance.IsRemySelected.Value);
        Megan.SetActive(DataManager.instance.IsMeganSelected.Value);
        Dog.SetActive(DataManager.instance.IsDogSelected.Value);
        
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

        //Play Music
        MusicManager.Play();
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

        //Tutorial
        if (DataManager.instance.IsTutorial.Value)
        {
            UIManager.instance.TipText.text = _tutorialInstructions[_tutorialStep];

            switch (_tutorialStep)
            {
                case 1:
                    if(InputManager.instance.GetInputMovementVector() != Vector2.one && InputManager.instance.GetInputMovementVector() != _previousMousePos)
                    {
                        TutorialNextStep();
                    }
                    _previousMousePos = InputManager.instance.GetInputMovementVector();
                    break;
            }

            _tutorialTimer += Time.deltaTime;
        }
        else
        {
            UIManager.instance.TipText.text = "Est-ce que tu es prêt à commencer ?";           
        }

        //Check if the player is ready
        if ((_isMazeGenerated && InputManager.instance.IsInteractAction() && !DataManager.instance.IsTutorial.Value))
        {
            SetCountdownStatement();
        }

        //Go to next tutorial step
        if (InputManager.instance.IsInteractAction())
        {
            TutorialNextStep();
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
        UIManager.instance.ShowTipUI(false);
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
    public void TutorialNextStep()
    {
        if(_tutorialTimer > _tutorialInstructionMinimumTime && _tutorialStep <= _tutorialInstructions.Length - 1)
        {      
            //If the player is ready at the last instructions, start the countdown
            if(_tutorialStep == _tutorialInstructions.Length - 1)
            {
                SetCountdownStatement();
                return;
            }


            _tutorialStep++;

            if(_tutorialStep == 7)
            {
                _tutorialStep = (DataManager.instance.IsManualMode.Value ? 7 : (DataManager.instance.IsSemiAutoMode.Value ? 8 : 9));
            }
            else
            {
                if (_tutorialStep > 7)
                {
                    _tutorialStep = _tutorialInstructions.Length - 1;
                }
            }
          
            _tutorialTimer = 0;
        }
    }
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
        if (GetGameStatement() != GameStatement.End)
        {
            //Update game status
            _isGameLost = isLost;

            //Change state
            SetEndStatement();
        }
    }

    #endregion
}