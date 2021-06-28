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

    public bool registerThisEvent = true;
    
    public IntVariable atomVariableInt;
    public FloatVariable atomVariableFloat;
    public BoolVariable atomVariableBool;
    
    private IntEvent _atomVariableIntEvent;
    private FloatEvent _atomVariableFloatEvent;
    private BoolEvent _atomVariableBoolEvent;
    
    public StringVariable presetName;
    
    public virtual void SetMinMaxInit()
    {
       
    }

    public virtual void Init()
    {
        if (atomVariableInt)
            _atomVariableIntEvent = atomVariableInt.Changed;
        if (atomVariableFloat)
            _atomVariableFloatEvent = atomVariableFloat.Changed;
        if (atomVariableBool)
            _atomVariableBoolEvent = atomVariableBool.Changed;

        if (registerThisEvent)
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

        atomVariableInt?.Reset();
        atomVariableFloat?.Reset();
        atomVariableBool?.Reset();
    }


    public virtual void UpdateValue(int i)
    {
        presetName?.SetValue("");
        //Debug.Log("Updae int");
    }
    
    public virtual  void UpdateValue(float f)
    {
        presetName?.SetValue("");
        //Debug.Log("Updae float");
    }
    
    public virtual  void UpdateValue(bool b)
    {
        presetName?.SetValue("");
        //Debug.Log("Updae bool " + b);
    }

    private void OnDestroy()
    {
        if(_atomVariableIntEvent)
            _atomVariableIntEvent.Unregister(UpdateValue);
        
        if(_atomVariableFloatEvent)
            _atomVariableFloatEvent.Unregister(UpdateValue);
        
        if(_atomVariableBoolEvent)
            _atomVariableBoolEvent.Unregister(UpdateValue);
    }
}
