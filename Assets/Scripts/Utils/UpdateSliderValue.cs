using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSliderValue : MonoBehaviour
{
    private TextMeshProUGUI ValueText;

    private void Start()
    {
        ValueText = GetComponent<TextMeshProUGUI>();
    }

    public void OnSliderValueChanged(float value)
    {
        if(ValueText)
            ValueText.text = (value * 100).ToString("0");
    }
}
