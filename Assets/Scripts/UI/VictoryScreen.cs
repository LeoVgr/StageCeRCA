using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    #region "Attributs"
    
    [Header("GameObject")]
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI TargetHitText;
    public Text MessageText;
    #endregion

    #region "Events"
    #endregion

    #region "Methods"
    public void ShowScreen(bool isLoose = false)
    {
 
        MessageText.text = isLoose ? "Temps écoulé" : "Victoire !";     
       
        FillText();
    }
    public void FillText()
    {
        ScoreText.text = "" + DataManager.instance.Score.Value * 100;
        TargetHitText.text = "" + DataManager.instance.TargetHit.Value;
        TimeText.text = "" + UIManager.instance.TimerUI.GetComponent<PlayerTimer>().GetPlayerTimer() + " s";

        //Show end time only if it is allowed
        if (!DataManager.instance.ShowEndTime.Value)
        {
            TimeText.text = "--:--";
        }
      
    }
    #endregion
}
