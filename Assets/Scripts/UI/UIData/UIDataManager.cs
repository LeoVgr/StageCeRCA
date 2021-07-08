using UnityAtoms.BaseAtoms;
using UnityEngine;


/**
 * @author : Samuel BUSSON
 * @brief : Base class for all UI type such as slider, checkbox or input field
 * @date : 07/2020
 */
public class UIDataManager : MonoBehaviour
{
    [HideInInspector]
    public int minInt;
    [HideInInspector]
    public int maxInt;

    public bool RegisterThisEvent = true;
    
    public IntVariable AtomVariableInt;
    public FloatVariable AtomVariableFloat;
    public BoolVariable AtomVariableBool;

    private IntEvent _atomVariableIntEvent;
    private FloatEvent _atomVariableFloatEvent;
    private BoolEvent _atomVariableBoolEvent;
    
    public StringVariable presetName;

    private void OnDestroy()
    {
        if (_atomVariableIntEvent)
            _atomVariableIntEvent.Unregister(UpdateValue);

        if (_atomVariableFloatEvent)
            _atomVariableFloatEvent.Unregister(UpdateValue);

        if (_atomVariableBoolEvent)
            _atomVariableBoolEvent.Unregister(UpdateValue);
    }

    public virtual void SetMinMaxInit()
    {
       
    }
    public virtual void Init()
    {
        if (AtomVariableInt)
            _atomVariableIntEvent = AtomVariableInt.Changed;
        if (AtomVariableFloat)
            _atomVariableFloatEvent = AtomVariableFloat.Changed;
        if (AtomVariableBool)
            _atomVariableBoolEvent = AtomVariableBool.Changed;

        if (RegisterThisEvent)
        {
            if (_atomVariableIntEvent)
            {
                _atomVariableIntEvent.UnregisterAll();
                _atomVariableIntEvent.Register(UpdateValue);
            }

            if (_atomVariableFloatEvent)
            {
                _atomVariableFloatEvent.UnregisterAll();
                _atomVariableFloatEvent.Register(UpdateValue);
            }

            if (_atomVariableBoolEvent)
            {
                _atomVariableBoolEvent.UnregisterAll();
                _atomVariableBoolEvent.Register(UpdateValue);
            }
        }

        AtomVariableInt?.Reset();
        AtomVariableFloat?.Reset();
        AtomVariableBool?.Reset();
    }
    public virtual void UpdateValue(int i)
    {
        //presetName?.SetValue("");
    }   
    public virtual  void UpdateValue(float f)
    {
        //presetName?.SetValue("");
    }  
    public virtual  void UpdateValue(bool b)
    {
        //presetName?.SetValue("");
    }
    
}
