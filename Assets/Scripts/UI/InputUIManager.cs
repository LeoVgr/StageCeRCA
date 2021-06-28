using System.Globalization;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : Manage input field in first UI
 * @date : 07/2020
 */
public class InputUIManager : UIDataManager
{
    public StringVariable atomVariableString;
    
    private InputField _tmpInputField;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmpInputField = GetComponentInChildren<InputField>();
        _tmpInputField.onValueChanged.AddListener (delegate {ValueChangeCheck ();});

        ResetValue();
    }

    private void OnDestroy()
    {
        _tmpInputField.onValueChanged.RemoveListener(delegate {ValueChangeCheck ();});
    }

    public void ResetValue()
    {
        atomVariableInt?.Reset();
        atomVariableFloat?.Reset();
    }
    
    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        if (atomVariableInt && int.TryParse(_tmpInputField.text, out int i))
            atomVariableInt.SetValue(i);
        

        if (atomVariableFloat && float.TryParse(_tmpInputField.text, out float f))
            atomVariableFloat.SetValue(f);

        if (atomVariableString)
            atomVariableString.SetValue(_tmpInputField.text);
    }

    public override void SetMinMaxInit()
    {
        if(!_tmpInputField)
            _tmpInputField = GetComponentInChildren<InputField>();

        _tmpInputField.text = atomVariableInt != null ? atomVariableInt.Value.ToString() :
            atomVariableFloat != null ? atomVariableFloat.Value.ToString(CultureInfo.InvariantCulture) : "";
    }

    public override void UpdateValue(float f)
    {
        SetTextValue(f);
        base.UpdateValue(f);
    }

    public override void UpdateValue(int i)
    {
        SetTextValue(i);
        base.UpdateValue(i);
    }

    public void SetTextValue(float f)
    {
        _tmpInputField.text = f.ToString(CultureInfo.InvariantCulture);
    }
}
