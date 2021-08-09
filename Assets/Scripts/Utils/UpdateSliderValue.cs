using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSliderValue : MonoBehaviour
{
    public float ValueDisplayFactor;
    public string Precision;

    private TextMeshProUGUI _valueText;

    private void Start()
    {
        _valueText = GetComponent<TextMeshProUGUI>();
    }

    public void OnSliderValueChanged(float value)
    {
        if(_valueText)
            _valueText.text = (ValueDisplayFactor * value).ToString(Precision);
    }
}
