using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditMenu;

    // Start is called before the first frame update
    void Start()
    {
        CreditMenu.SetActive(false);
    }

    public void StartGame()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
