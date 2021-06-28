using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/**
 * @author : Samuel BUSSON
 * @brief : UpdateTextMeshPro is used to set the score through the IntEvent
 * @date : 07/2020
 */
public class UpdateTextScore : MonoBehaviour
{

    public IntEvent updateEvent;
    public BoolVariable isCameraFps;
    public BoolVariable isScoreShow;
    
    [Header("Sound Effect")]
    [FMODUnity.EventRef][SerializeField]
    private string increaseScore;
    [FMODUnity.EventRef][SerializeField]
    private string decreaseScore;

    private int previousNumber;
    
    //FMODUnity.RuntimeManager.PlayOneShot(fireEvent, transform.position);
    
    private TextMeshProUGUI _textMeshProUi;
    private Text _text;


    // Start is called before the first frame update
    void Awake()
    {
        _textMeshProUi = GetComponent<TextMeshProUGUI>();
        _text = GetComponent<Text>();
        updateEvent.Register(UpdateText);
        if (isCameraFps)
        {
            isCameraFps.Changed.Register(ShowText);
            ShowText(isCameraFps.Value);
        }
    }

    private void ShowText(bool val)
    {
        if(isScoreShow.Value)
            gameObject.SetActive(val);
    }

    private void OnDestroy()
    {
        updateEvent.Unregister(UpdateText);
    }


    void UpdateText(int i)
    {
        ScaleEvent();
        ColorEvent();
        
        if(previousNumber < i)
            FMODUnity.RuntimeManager.PlayOneShot(increaseScore, transform.position);
        else
            FMODUnity.RuntimeManager.PlayOneShot(decreaseScore, transform.position);
        

        previousNumber = i;
        DOVirtual.Float((i - 1) * 100, i * 100, 0.5f, UpdateFloatText);
    }

    private void ScaleEvent()
    {
        var localScale = transform.localScale;
        
        float x = Random.Range(0.1f * localScale.x, 0.4f * localScale.x);
        float y = Random.Range(0.1f * localScale.y, 0.4f * localScale.y);

        transform.DOScale(
                new Vector3(localScale.x + x, localScale.y + y, localScale.z), 0.35f)
            .SetEase(Ease.OutBounce).SetLoops(2, LoopType.Yoyo);
    }

    private void ColorEvent()
    {
        if (_text)
            _text.material.DOColor(Color.green * 4.0f, "_Color", 0.35f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>  _text.material.DOColor(Color.yellow * 2.0f, "_Color", 0.35f));
    }

    private void UpdateFloatText(float f)
    {
        if(_textMeshProUi)
            _textMeshProUi.text = "Score : " + (int)f;
        if(_text)
            _text.text = "Score : " + (int)f;
    }
    
}
