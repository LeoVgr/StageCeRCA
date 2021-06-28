using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : FillPresets allows player to click on a preset and fill all fields (slider, checkbox, input field,...) with the preset values
 * @date : 07/2020
 */
public class FillPresets : MonoBehaviour
{
    public IntVariable seed;
    public IntVariable longueur;
    public IntVariable nbrVirage;
    public FloatVariable largeur;
    public FloatVariable hauteur;
    public FloatVariable tailleDesImages;
    public FloatVariable tempsImage;
    public FloatVariable tempsChrono;
    public BoolVariable randomizeImage;
    
    
    public StringVariable presetName;

    public InputField _Input;
    private Dropdown _TmpDropdown;
    
    public enum ColumnNames
    {
        Seed,
        Longueur,
        Largeur,
        Hauteur,
        NbrVirage,
        TempsImage,
        RandomizeImage,
        TailleDesImages,
        Timer
    }

    private Dictionary<string, Dictionary<ColumnNames, float>> _presets;

    private void Start()
    {
        _TmpDropdown = GetComponentInChildren<Dropdown>();
        Dropdown.OptionData option = new Dropdown.OptionData("");
        _TmpDropdown.options.Add(option);
        
        _presets = new Dictionary<string, Dictionary<ColumnNames, float>>();
        
        _TmpDropdown.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        
        ReadCsv();
    }

    public  Dictionary<string, Dictionary<ColumnNames, float>> GetPresets()
    {
        return _presets;
    }

    private void ValueChangeCheck()
    {
        string text = _TmpDropdown.options[_TmpDropdown.value].text;
        _Input.text = text;
        
        DOVirtual.DelayedCall(Time.deltaTime, (() => presetName?.SetValue(text)));
        
        Dictionary<ColumnNames, float> dico;
         if (_presets.TryGetValue(text, out dico))
         {
             foreach (var keyValuePair in dico)
             {
                 switch (keyValuePair.Key)
                 {
                     case ColumnNames.Seed:
                         seed.SetValue((int)keyValuePair.Value);
                         break;
                     case ColumnNames.Longueur:
                         longueur.SetValue((int)keyValuePair.Value);
                         break;
                     case ColumnNames.Largeur:
                         largeur.SetValue(keyValuePair.Value);
                         break;
                     case ColumnNames.Hauteur:
                         hauteur.SetValue(keyValuePair.Value);
                         break;
                     case ColumnNames.TailleDesImages:
                         tailleDesImages.SetValue(keyValuePair.Value);
                         break;
                     case ColumnNames.NbrVirage:
                         nbrVirage.SetValue((int)keyValuePair.Value);
                         break;
                     case ColumnNames.TempsImage:
                         tempsImage.SetValue((int)keyValuePair.Value);
                         break;
                     case ColumnNames.RandomizeImage:
                         randomizeImage.SetValue(keyValuePair.Value >= 0.5f);
                         break;
                     case ColumnNames.Timer:
                         tempsChrono.SetValue(keyValuePair.Value);
                         break;
                 }
             }
         }
    }


    private void ReadCsv()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        //print("Streaming Assets Path: " + Application.streamingAssetsPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*");
                 
        foreach (FileInfo file in allFiles)
        {
            if (file.Name.Contains("presetsSave"))
            {
                StreamReader fileText = file.OpenText();

                string[] lines = fileText.ReadToEnd().
                    Split("\n"[0]);

                int i = 0;
         
                foreach (var line in lines)
                {
                    if (i > 0)
                    {
                        string[] value = line.Split(';');
                        string selection =  value[0].Trim();

                        if (selection != "")
                        {
                            Dropdown.OptionData option = new Dropdown.OptionData(selection);
                            _TmpDropdown.options.Add(option);
                        
                            _presets[selection] = new Dictionary<ColumnNames, float>();

                            float f = CheckStringLength(value[1]);
                            _presets[selection][(ColumnNames.Seed)] =  f;
                            
                            f = CheckStringLength(value[2]);
                            _presets[selection][(ColumnNames.Longueur)] = f;
                            
                            f = CheckStringLength(value[3]);
                            _presets[selection][(ColumnNames.Largeur)] = f;
                            
                            f = CheckStringLength(value[4]);
                            _presets[selection][(ColumnNames.Hauteur)] =  f;
                            
                            f = CheckStringLength(value[5]);
                            _presets[selection][(ColumnNames.NbrVirage)] =  f;
                            
                            f = CheckStringLength(value[6]);
                            _presets[selection][(ColumnNames.TailleDesImages)] = f;
                            
                            f = CheckStringLength(value[7]);
                            _presets[selection][(ColumnNames.RandomizeImage)] = f;
                            
                            f = CheckStringLength(value[8]);
                            _presets[selection][(ColumnNames.TempsImage)] = f;

                            f = CheckStringLength(value[9]);
                            _presets[selection][(ColumnNames.Timer)] = f;
                        }
                    }
                    i++;
                }
         
                fileText.Close();
                break;
            }
        }
    }

    private float CheckStringLength(string s)
    {
        if (s.Length > 0)
            s.Replace(".", ",");

        if (float.TryParse(s, out float result))
        {
            return result;
        }
        return 0;
    }

    public void UpdateDictionnary(Dictionary<string, Dictionary<ColumnNames, float>> newDico, bool resetSelect)
    {
        _presets = newDico;
        _TmpDropdown.options.Clear();
        
        Dropdown.OptionData option = new Dropdown.OptionData("");
        _TmpDropdown.options.Add(option);

        if(resetSelect)
            _TmpDropdown.value = 0;

        foreach (var keyValuePair in _presets)
        {
             option = new Dropdown.OptionData(keyValuePair.Key);
            _TmpDropdown.options.Add(option);
        }
    }


    public string GetText()
    {
      return _TmpDropdown.options[_TmpDropdown.value].text;
    }
}
