#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
public class GenerateAtomUI : MonoBehaviour
{
    public List<AtomVariableEditor> _Variables;
    private List<BaseUIAtom> _baseUiAtoms;
    
    
    public GameObject prefabSlider;
    public GameObject prefabCheckBox;
    public GameObject prefabInputField;
    
    
    private void Awake()
    {
        if (Application.IsPlaying(gameObject))
        {
            GenerateAllListeners();
        }
        else
        {
            InitGameObject();
            if (!gameObject.GetComponent<VerticalLayoutGroup>())
            {
                gameObject.AddComponent<VerticalLayoutGroup>();
                gameObject.GetComponent<VerticalLayoutGroup>().padding.left = 100;
            }
        }
    }

    [ContextMenu("Generate UI")]
    public void Generate()
    {
        foreach (AtomVariableEditor atomBaseVariable in _Variables)
        {
            if (atomBaseVariable.displayObject)
            {
                switch (atomBaseVariable.displayStyle)
                {
                    case DisplayStyle.Slider:
                        GenerateSlider(atomBaseVariable);
                        break;
                    case DisplayStyle.InputField:
                        GenerateInputField(atomBaseVariable);
                        break;
                    case DisplayStyle.Checkbox:
                        GenerateCheckbox(atomBaseVariable);
                        break;
                }
            }
        }
    }

    private void GenerateSlider(AtomVariableEditor atomBaseVariable)
    {
        GameObject go = Instantiate(prefabSlider, Vector3.zero, Quaternion.identity);
        SetObjectData(atomBaseVariable, go);
    }
    
    
    private void GenerateInputField(AtomVariableEditor atomBaseVariable)
    {
        GameObject go = Instantiate(prefabInputField, Vector3.zero, Quaternion.identity);
        SetObjectData(atomBaseVariable, go);
    }
    
    
    private void GenerateCheckbox(AtomVariableEditor atomBaseVariable)
    {
        GameObject go = Instantiate(prefabCheckBox, Vector3.zero, Quaternion.identity);
        SetObjectData(atomBaseVariable, go);

    }

    private void SetObjectData(AtomVariableEditor atomBaseVariable, GameObject go)
    {
        go.transform.SetParent(transform);
        go.GetComponent<BaseUIAtom>().atomVariableEditor = atomBaseVariable;
        go.name = atomBaseVariable.atomVariable.name;
        go.GetComponent<BaseUIAtom>().SetOnValueChanged();
    }

    private void GenerateAllListeners()
    {
        _baseUiAtoms = GetComponentsInChildren<BaseUIAtom>().ToList();

        foreach (BaseUIAtom baseUiAtom in _baseUiAtoms)
        {
            baseUiAtom.SetOnValueChanged();
        }
    }
    

    [ContextMenu("Fill List")]
    public void FillList()
    {
        _Variables = new List<AtomVariableEditor>();
        foreach (AtomBaseVariable atomBaseVariable in GetAllInstances<AtomBaseVariable>(true))
        {
            _Variables.Add(new AtomVariableEditor(atomBaseVariable, GetStyle(atomBaseVariable)));
        }
    }

    private DisplayStyle GetStyle(AtomBaseVariable atomBaseVariable)
    {
        DisplayStyle style = DisplayStyle.InputField;

        if (atomBaseVariable as BoolVariable)
            style = DisplayStyle.Checkbox;

        if (atomBaseVariable as StringVariable)
            style = DisplayStyle.InputField;

        return style;
    }

    private void InitGameObject()
    {
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GetComponent<Canvas>().sortingOrder = 100;
        GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(1440, 810);
    }
    
    
    public List<T> GetAllInstances<T>(bool withoutConstant) where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
        
        List<T> a = new List<T>();
        
        for(int i =0; i <guids.Length;i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            //If without constant, then don't add constant to list
            if (!(withoutConstant && AssetDatabase.LoadAssetAtPath<T>(path).GetType().Name.Contains("Const")))
            {
                if(ContainsType(AssetDatabase.LoadAssetAtPath<T>(path).GetType().Name))
                   a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }
        }
        return a;
    }

    private bool ContainsType(string s)
    {
        if (s.Contains("Int"))
            return true;
        if (s.Contains("String"))
            return true;
        if (s.Contains("Float"))
            return true;
        if (s.Contains("Bool"))
            return true;

        return false;
    }
}

public enum DisplayStyle
{
    Slider,
    InputField,
    Checkbox
}

public enum AtomType
{
    Int,
    Float,
    String,
    Bool
}



[Serializable]
public class AtomVariableEditor
{

    public AtomBaseVariable atomVariable;
    public DisplayStyle displayStyle;
    public AtomType atomType;
    public bool displayObject;

    public AtomVariableEditor(AtomBaseVariable atomVar, DisplayStyle display)
    {
        atomVariable = atomVar;
        displayStyle = display;
        this.displayObject = true;
        SetAtomType();
    }

    private void SetAtomType()
    {
        if (atomVariable.GetType().Name.Contains("Int"))
            atomType = AtomType.Int;
        
        if (atomVariable.GetType().Name.Contains("String"))
            atomType = AtomType.String;
        
        if (atomVariable.GetType().Name.Contains("Float"))
            atomType = AtomType.Float;
        
        if (atomVariable.GetType().Name.Contains("Bool"))
            atomType = AtomType.Bool;
    }
}

#endif