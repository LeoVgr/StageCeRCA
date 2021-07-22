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
    void Awake()
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
        AtomVariableInt?.Reset();
        AtomVariableFloat?.Reset();
    }
    
    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        if (AtomVariableInt && int.TryParse(_tmpInputField.text, out int i))
            AtomVariableInt.SetValue(i);
        

        if (AtomVariableFloat && float.TryParse(_tmpInputField.text, out float f))
            AtomVariableFloat.SetValue(f);

        if (atomVariableString)
            atomVariableString.SetValue(_tmpInputField.text);
    }

    public override void SetMinMaxInit()
    {
        if(!_tmpInputField)
            _tmpInputField = GetComponentInChildren<InputField>();

        _tmpInputField.text = AtomVariableInt != null ? AtomVariableInt.Value.ToString() :
            AtomVariableFloat != null ? AtomVariableFloat.Value.ToString(CultureInfo.InvariantCulture) : "";
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
