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
        //When the toggle change, update the atom's variable value
        if(AtomVariableBool)
            AtomVariableBool.SetValue( _toggle.isOn );
    }
    public override void UpdateValue(bool b)
    {
        //When the atom's variable change, change the value of the toggle
        _toggle.isOn = b;
    }
    public override void Init()
    {
        base.Init();

        if (AtomVariableBool)
            _toggle.isOn = AtomVariableBool.Value;
        
    }
    
    
}
