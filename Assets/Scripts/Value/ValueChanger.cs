using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : Useless class
 * @date : 07/2020
 */
public class ValueChanger : MonoBehaviour, ISubmitHandler, IUpdateSelectedHandler
{
    public IntVariable intVariable;
    public FloatVariable floatVariable;

    public IntVariable longueurDuCouloir;
    
    public MenuType menuType;

    public enum MenuType
    {
        Normal,
        Turn
    }
    
    
    private TMP_InputField inputField;
    private Slider slider;

    private void Start()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        slider = GetComponentInChildren<Slider>();

        if (intVariable)
        {
            if(slider)
                slider.value = intVariable.Value;
            if(inputField)
                inputField.text = ""+intVariable.Value;
        }

        if (floatVariable)
        {
            if(slider)
                slider.value = floatVariable.Value;
            if(inputField)
                inputField.text = ""+floatVariable.Value;
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SetValue();
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        SetValue();
    }

    private void SetValue()
    {
        float valFloat = 0.0f;
        int valInt = 0;
        
        if (inputField)
        {
            float.TryParse(inputField.text, out valFloat);
            int.TryParse(inputField.text, out valInt);
        }

        if (slider)
        {
            valFloat = slider.value;
            valInt = (int) + slider.value;
        }
        
        switch (menuType)
        {
            case MenuType.Turn:
                if (valInt > longueurDuCouloir.Value / 2)
                {
                    valInt = longueurDuCouloir.Value / 2;
                }
                break;
        }

        if (intVariable)
            intVariable.SetValue(valInt);
        
        if (floatVariable)
            floatVariable.SetValue(valFloat);
    }
}
