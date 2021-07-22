using System;
using DG.Tweening;
using Player;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimer : MonoBehaviour
{
    #region "Attributs"
    public FloatVariable TimerMax;

    /* Timer representation (text or image) */
    public TextMeshProUGUI TimerText;
    public GameObject TimerTextBackground;
    public Image TimerSprite;

    private float _lastCountDown;
    private float _initialValue;
    private bool _once;
    #endregion

    #region "Events"
    private void Start()
    {
        _initialValue = TimerMax.Value;
        _lastCountDown = 10f;
        gameObject.SetActive(TimerMax.Value > 1);
    }
    void Update()
    {
        if (GameManager.instance.GetGameStatement() == GameManager.GameStatement.Running)
        {
            if (TimerMax.Value > 0)
            {
                TimerMax.Value -= Time.deltaTime;

                //Animate timer text under 10 seconds
                if (TimerMax.Value < _lastCountDown && TimerMax.Value != 0)
                {
                    _lastCountDown -= 1;
                   
                    TimerTextBackground.transform.DOPunchScale(TimerText.transform.localScale * 0.2f, 0.3f);
                    TimerText.DOColor(Color.red, 0.3f).OnComplete(()=> TimerText.DOColor(Color.white, 0.3f));
                }
            }
            else
            {
                TimerMax.Value = 0.0f;
                if (!_once)
                {
                    _once = true;
                    GameManager.instance.EndGame(true);
                }   
            }

            //Update timer image
            TimerSprite.fillAmount = TimerMax.Value / _initialValue;

            //Update timer text
            TimerText.SetText(TimerParser(TimerMax.Value));
        }
    }
    #endregion

    #region "Methods"
    private string TimerParser(float seconds)
    {
        string s = "";


        int minutesInt = (int) (seconds / 60);
        int secondsInt = (int) (seconds - minutesInt * 60);
        int miliSeconds =  (int)((seconds - secondsInt - (minutesInt * 60)) * 100);
        

        s = CheckUnderTen(minutesInt) + ":" + CheckUnderTen(secondsInt) /*+ "." + CheckUnderTen(miliSeconds)*/;

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
