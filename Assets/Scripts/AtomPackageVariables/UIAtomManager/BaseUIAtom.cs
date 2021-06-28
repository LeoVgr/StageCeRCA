#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class BaseUIAtom : MonoBehaviour
{
    public Text label;

    public AtomVariableEditor atomVariableEditor;


    public virtual void SetOnValueChanged()
    {
        label.text = atomVariableEditor.atomVariable.name;
    }
    

    public virtual void SetValueBool(bool b)
    {
        BoolVariable bv = atomVariableEditor.atomVariable as BoolVariable;
        bv?.SetValue(b);
    }
    
    public virtual void SetValueFloat(float f)
    {
        FloatVariable fv = atomVariableEditor.atomVariable as FloatVariable;
        fv?.SetValue(f);
    }
    
    public virtual  void SetValueInt(int i)
    {
        IntVariable iv = atomVariableEditor.atomVariable as IntVariable;
        iv?.SetValue(i);
    }

    public virtual void SetValueString(string s)
    {
        StringVariable sv = atomVariableEditor.atomVariable as StringVariable;
        sv?.SetValue(s);
    }
}

#endif