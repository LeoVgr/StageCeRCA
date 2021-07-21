using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @author : Samuel BUSSON
 * @brief : GameFunction is used to set buttons action
 * @date : 07/2020
 */
public class GameFunction : MonoBehaviour
{

    public GameObjectVariable a_mazeGeneratorObject;
    public IntVariable score;
    public BoolVariable isGameStart;

    private VictoryScreen _victoryScreenScript;
    public GameObjectVariable _player;
    
    private void Start()
    {
        _victoryScreenScript = GetComponentInParent<VictoryScreen>();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void RestartGame()
    {
        if (_victoryScreenScript)
        {
            _victoryScreenScript.HideScreen();
        }
        else
        {
            //_player.Value.GetComponent<PlayerMovement>().ShowMenu();
        }
        
        //TODO _player.Value.GetComponent<PlayerMovement>().Restart();
        a_mazeGeneratorObject.Value.SetActive(true);
        InputManager.instance.EnableInputs();
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        
        score.Reset(true);
        isGameStart.Reset(true);
    }

    public void StartGame(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
    
    public void ShowGameObject(GameObject go)
    {
      go.SetActive(!go.activeSelf);
    }
}
