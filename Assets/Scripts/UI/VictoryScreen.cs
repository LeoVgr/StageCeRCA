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

    [Header("Atom Variables")] 
    public IntVariable scoreAtom;
    public IntVariable targetHit;
    public FloatVariable time;
    
    [Header("GameObject")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI temps;
    public TextMeshProUGUI cibleTouchees;
    public Text messageText;
    
    private List<Image> _baseImageList;
    private List<Text> _baseTextList;
    private List<TextMeshProUGUI> _baseTextMeshProList;

    
    private Color alphaNull;
    
    // Start is called before the first frame update
    void Awake()
    {
        CheckList();
        _baseImageList = GetComponentsInChildren<Image>(true).ToList();
        _baseTextList = GetComponentsInChildren<Text>(true).ToList();
        _baseTextMeshProList = GetComponentsInChildren<TextMeshProUGUI>(true).ToList();

        HideScreen();
    }


    public void ShowScreen(bool isLoose = false)
    {
        
        messageText.text = isLoose ? "Victoire !!" : "Temps écoulé";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetAll();
        FillText();
    }

    public void FillText()
    {
        score.text = "" + scoreAtom.Value * 100;
        cibleTouchees.text = "" + targetHit.Value;
        temps.text = "" + time.Value + " s";
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
}
