using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

/**
 * @author : Samuel BUSSON
 * @brief : Manage CheckBox in first UI
 * @date : 07/2020
 */
public class ToggleUIManager : UIDataManager
{
    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponentInChildren<Toggle>();
        
        _toggle.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
    }
    
    private void OnDestroy()
    {
        _toggle.onValueChanged.RemoveListener(delegate {ValueChangeCheck ();});
    }

    private void ValueChangeCheck()
    {
        if(atomVariableBool)
            atomVariableBool.SetValue( _toggle.isOn );
    }

    public override void UpdateValue(bool b)
    {
        _toggle.isOn = b;
    }

    public override void Init()
    {
        base.Init();

        if (atomVariableBool)
            _toggle.isOn = atomVariableBool.Value;
        
    }
    
    
}
