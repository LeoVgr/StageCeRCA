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
    [Header("Atom Variables")] 
    public IntVariable ScoreAtom;
    public IntVariable TargetHit;
    public BoolVariable ShowEndTime;
    
    [Header("GameObject")]
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI TargetHitText;
    public Text MessageText;
    
    private List<Image> _baseImageList;
    private List<Text> _baseTextList;
    private List<TextMeshProUGUI> _baseTextMeshProList;

    private Color alphaNull;
    #endregion

    #region "Events"
    void Awake()
    {
        CheckList();
        _baseImageList = GetComponentsInChildren<Image>(true).ToList();
        _baseTextList = GetComponentsInChildren<Text>(true).ToList();
        _baseTextMeshProList = GetComponentsInChildren<TextMeshProUGUI>(true).ToList();

        HideScreen();
    }
    #endregion

    #region "Methods"
    public void ShowScreen(bool isLoose = false)
    {
 
        MessageText.text = isLoose ? "Temps écoulé" : "Victoire !";     
        SetAll();
        FillText();
    }
    public void FillText()
    {
        ScoreText.text = "" + ScoreAtom.Value * 100;
        TargetHitText.text = "" + TargetHit.Value;
        TimeText.text = "" + UIManager.instance.TimerUI.GetComponent<PlayerTimer>().GetPlayerTimer() + " s";

        //Show end time only if it is allowed
        if (!ShowEndTime.Value)
        {
            TimeText.text = "--:--";
        }
      
    }
    public void HideScreen()
    {
        CheckList();
        foreach (Image image in _baseImageList)
        {
            var color = image.color;
            color =  new Color(color .r,color .g,color.b,0);
            image.color = color;
        }
        
        foreach (Text text in _baseTextList)
        {
            var color = text.color;
            color =  new Color(color .r,color .g,color.b,0);
            text.color = color;
        }
        
        
        foreach (TextMeshProUGUI text in _baseTextMeshProList)
        {
            var color = text.color;
            color =  new Color(color .r,color .g,color.b,0);
            text.color = color;        
        }
    }
    public void SetAll()
    {
        CheckList();

        foreach (Image image in _baseImageList)
        {
            var color = image.color;
            image.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f);
        }
        
        foreach (Text text in _baseTextList)
        {
            var color = text.color;
            text.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f);
        }
        
        
        foreach (TextMeshProUGUI text in _baseTextMeshProList)
        {
            var color = text.color;
            text.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f);
        }
    }
    private void CheckList()
    {
        if (_baseImageList == null)
            _baseImageList = GetComponentsInChildren<Image>(true).ToList();
        
        if (_baseTextList == null)
            _baseTextList = GetComponentsInChildren<Text>(true).ToList();
        
        if (_baseTextMeshProList == null)
            _baseTextMeshProList = GetComponentsInChildren<TextMeshProUGUI>(true).ToList();
    }
    #endregion
}
