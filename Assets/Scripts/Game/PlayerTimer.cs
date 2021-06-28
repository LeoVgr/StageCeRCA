using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimer : MonoBehaviour
{

    public FloatVariable timerMax;
    public Image timerSprite;
    public BoolVariable a_IsGameStart;
    public PlayerMovement playerMovement;
    
    private float _initialValue;
    private bool _once;

    private void Start()
    {
        a_IsGameStart.Changed.Register(IsGameStart);
        _initialValue = float.MaxValue;
    }


    // Update is called once per frame
    void Update()
    {
        if (a_IsGameStart.Value)
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
                    playerMovement.DisplayScreen(true);
                }   
            }
            timerSprite.fillAmount = timerMax.Value / _initialValue;
        }
    }

    private void IsGameStart(bool b)
    {
        if (b)
        {
            _initialValue = timerMax.Value;
            gameObject.SetActive(timerMax.Value > 1);
        }
    }


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
}
