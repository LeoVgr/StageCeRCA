#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputFieldUIAtom : BaseUIAtom
{
    public InputField InputField;

    public override void SetOnValueChanged()
    {
        SetValue();
        base.SetOnValueChanged();
        switch (atomVariableEditor.atomType)
        {
            case AtomType.Int:
                InputField.onValueChanged.AddListener(delegate(string text) { SetValueInt(int.TryParse(text, out int val) ? val : 0); });
                break;
            case AtomType.Float:
                InputField.onValueChanged.AddListener(delegate(string text) { SetValueFloat(float.TryParse(text, out float val) ? val : 0); });
                break;
            case AtomType.String:
                InputField.onValueChanged.AddListener(SetValueString);
                break;
            case AtomType.Bool:
                InputField.onValueChanged.AddListener(delegate(string text) { SetValueBool(text == "1");});
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
                    InputField.text = var.Value.ToString();
                break;
            case AtomType.Float:
                FloatVariable varFloat = atomVariableEditor.atomVariable as FloatVariable;
                if (varFloat != null)
                    InputField.text = varFloat.Value.ToString(CultureInfo.InvariantCulture);
                break;
            case AtomType.String:
                StringVariable varString = atomVariableEditor.atomVariable as StringVariable;
                if (varString != null)
                    InputField.text = varString.Value;
                break;
            case AtomType.Bool:
                BoolVariable varBool = atomVariableEditor.atomVariable as BoolVariable;
                if (varBool != null)
                    InputField.text = varBool.Value ? "1" : "0";
                break;
        }
    }
    
    private void OnDestroy()
    {
        InputField.onValueChanged.RemoveAllListeners();
    }
}

#endif