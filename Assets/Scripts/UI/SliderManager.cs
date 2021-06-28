using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : Manage Slider in first UI
 * @date : 07/2020
 */
public class SliderManager : UIDataManager
{
    public enum SliderType
    {
        Normal,
        Turn,
        Length,
        Option
    }

    public int roundNumber;

    public TextMeshProUGUI min;
    public TextMeshProUGUI max;
    public TextMeshProUGUI value;

    public Vector2Event turnEvent;
    public Vector2Variable turnMinMax;
    
    public SliderType sliderType;

    private Slider _slider;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponentInChildren<Slider>();
        _slider.onValueChanged.AddListener (delegate {ValueChangeCheck ();});

        if (turnEvent)
            turnEvent.Register(EventResponse);
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(delegate {ValueChangeCheck ();});
        
        if (turnEvent)
            turnEvent.Unregister(EventResponse);
    }


    private void EventResponse(Vector2 value)
    {
        minInt = (int) value.x;
        maxInt = (int) value.y;
        SetMinMax(minInt, maxInt);
        _slider.value = maxInt / 2.0f;
        ValueChangeCheck();
    }

    private void SetMinMax(int x, int y)
    {
        _slider.minValue = x;
        _slider.maxValue = y;
        min.text = _slider.minValue.ToString(CultureInfo.InvariantCulture);
        max.text = _slider.maxValue.ToString(CultureInfo.InvariantCulture);
    }

    public override void SetMinMaxInit()
    {
        if(!_slider)
            _slider = GetComponentInChildren<Slider>();

        SetMinMax(minInt, maxInt);

        _slider.value = atomVariableInt != null ? atomVariableInt.Value :
            atomVariableFloat != null ? atomVariableFloat.Value : 0;

        value.text = _slider.value.ToString("F2");
    }
    
    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        if(roundNumber > 0)
            _slider.value = (float) Math.Round(_slider.value , roundNumber);
        
        
        if (atomVariableInt)
            atomVariableInt.SetValue((int)_slider.value);
        

        if (atomVariableFloat)
            atomVariableFloat.SetValue(_slider.value);


        if (sliderType == SliderType.Length && _slider.value > 0.0f)
        {
            turnMinMax.SetValue(new Vector2(0, _slider.value * 0.25f));
        }
        
        value.text = _slider.value.ToString("F2");
    }

    public override void UpdateValue(float f)
    {
        SetSliderValue(f);
        base.UpdateValue(f);
    }

    public override void UpdateValue(int i)
    {
        SetSliderValue(i);
        base.UpdateValue(i);
    }

    public void SetSliderValue(float f)
    {
        if (_slider)
        {
            if(Math.Abs(f - _slider.value) > 0.1f)
                _slider.value = f;
        }
    }
}
