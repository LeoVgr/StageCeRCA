using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject CreditMenu;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(true);
        CreditMenu.SetActive(false);
    }

    public void StartGame()
    {

    }
    public void ShowCredit()
    {      
        CreditMenu.SetActive(true);
        Menu.GetComponent<Animator>().SetTrigger("Out");
        CreditMenu.GetComponent<Animator>().SetTrigger("In");
    }
    public void HideCredit()
    {
        Menu.GetComponent<Animator>().SetTrigger("In");
        CreditMenu.GetComponent<Animator>().SetTrigger("Out");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
