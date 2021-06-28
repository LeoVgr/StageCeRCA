
#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxUIAtom : BaseUIAtom
{
    public Toggle checkbox;

    public override void SetOnValueChanged()
    {
        SetValue();
        base.SetOnValueChanged();
        switch (atomVariableEditor.atomType)
        {
            case AtomType.Int:
                checkbox.onValueChanged.AddListener(delegate(bool isOn) { SetValueInt(isOn ? 1 : 0); });
                break;
            case AtomType.Float:
                checkbox.onValueChanged.AddListener(delegate(bool isOn) { SetValueFloat(isOn ? 1 : 0); });
                break;
            case AtomType.String:
                checkbox.onValueChanged.AddListener(delegate(bool isOn) { SetValueString(isOn ? "1" : "0"); });
                break;
            case AtomType.Bool:
                checkbox.onValueChanged.AddListener(SetValueBool);
                break;
        }
    }

    public void SetValue()
    {
        switch (atomVariableEditor.atomType)
        {
            case AtomType.Int:
                IntVariable var = atomVariableEditor.atomVariable as IntVariable;
                if (var != null)
                    checkbox.isOn = var.Value == 1;
                break;
            case AtomType.Float:
                FloatVariable varFloat = atomVariableEditor.atomVariable as FloatVariable;
                if (varFloat != null)
                    checkbox.isOn = varFloat.Value >= 0.9f;
                break;
            case AtomType.String:
                StringVariable varString = atomVariableEditor.atomVariable as StringVariable;
                if (varString != null)
                    checkbox.isOn = varString.Value == "1";
                break;
            case AtomType.Bool:
                BoolVariable varBool = atomVariableEditor.atomVariable as BoolVariable;
                if (varBool != null)
                    checkbox.isOn = varBool.Value;
                break;
        }
    }


    private void OnDestroy()
    {
        checkbox.onValueChanged.RemoveAllListeners();
    }
}
#endif