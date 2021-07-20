using System;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimer : MonoBehaviour
{
    #region "Attributs"
    public FloatVariable timerMax;
    public Image timerSprite;
    public PlayerMovement playerMovement;
    
    private float _initialValue;
    private bool _once;
    #endregion

    #region "Events"
    private void Start()
    {
        _initialValue = timerMax.Value;
        gameObject.SetActive(timerMax.Value > 1);
    }
    void Update()
    {
        if (GameManager.instance.IsGameRunning())
        {
            if (timerMax.Value > 0)
            {
                timerMax.Value -= Time.deltaTime;
            }
            else
            {
                timerMax.Value = 0.0f;
                if (!_once)
                {
                    _once = true;
                    GameManager.instance.EndGame(true);
                }   
            }

            timerSprite.fillAmount = timerMax.Value / _initialValue;
        }
    }
    #endregion

    #region "Methods"
    private string timeParser(float seconds)
    {
        string s = "";


        int minutesInt = (int) (seconds / 60);
        int secondsInt = (int) (seconds - minutesInt * 60);
        int miliSeconds =  (int)((seconds - secondsInt - (minutesInt * 60)) * 100);
        

        s = CheckUnderTen(minutesInt) + " : " + CheckUnderTen(secondsInt) + "." + CheckUnderTen(miliSeconds);

        return s;
    }
    private string CheckUnderTen(int number)
    {
        string stringValue = number + "";

        if (number < 10)
            stringValue = "0" + stringValue;

        return stringValue;
    }
    #endregion
}
