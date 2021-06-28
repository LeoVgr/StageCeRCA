#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUIAtom : BaseUIAtom
{
    public Text minText;
    public Text maxText;

    public Slider slider;
    
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}

#endif