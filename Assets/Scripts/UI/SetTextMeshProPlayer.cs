using System;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

/**
 * @author : Samuel BUSSON
 * @brief : SetTextMeshProPlayer set values when the player pauses the game
 * @date : 07/2020
 */
public class SetTextMeshProPlayer : MonoBehaviour
{
    public List<TextToEvent> _Dictionary;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (_Dictionary != null)
        {
            foreach (TextToEvent textToEvent in _Dictionary)
            {
                textToEvent.InitEvent();
            }
        }
    }


    private void OnEnable()
    {
        SetAllText();
    }

    public void SetAllText()
    {
        if (_Dictionary != null)
        {
            foreach (TextToEvent textToEvent in _Dictionary)
            {
                textToEvent.SetTextMeshProValue();
            }
        }
    }

    [Serializable]
    public class TextToEvent
    {
        public TMP_Text tmpText;
        public IntVariable atomInt;
        public FloatVariable atomFloat;
        public BoolVariable atomBool;

        private string _baseText = "";

        public void InitEvent()
        {
            _baseText = tmpText.text;
        }

        public void RegisterAction(int i)
        {
            tmpText.text = _baseText + " " + i;
        }
        
        public void RegisterAction(float f)
        {
            tmpText.text = _baseText + " " +  f;
        }
        
        public void RegisterAction(bool b)
        {
            tmpText.text = _baseText + (b ? " oui" : " non");
        }

        public void SetTextMeshProValue()
        {
            if (atomInt)
                RegisterAction(atomInt.Value);
            if (atomFloat)
                RegisterAction(atomFloat.Value);
            if (atomBool)
                RegisterAction(atomBool.Value);

        }
    }

   
}
