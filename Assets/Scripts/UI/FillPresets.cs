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
    public InputField Input;

    private Dropdown _tmpDropdown;
    private Dictionary<string, Dictionary<ColumnNames, float>> _presets;

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
        Timer,
        Score,
        FpsCamera,
        Shoot,
        ShowEndTime,
        Speed,
        BreakForce,
        Remy,
        Megan,
        Mousey,
        AutoMode,
        SemiAutoMode,
        ManualMode,
        MusicVolume,
        SfxVolume,
        CrosshairColorized
    }    

    private void Start()
    {
        _tmpDropdown = GetComponentInChildren<Dropdown>();
        Dropdown.OptionData option = new Dropdown.OptionData("");
        _tmpDropdown.options.Add(option);
        
        _presets = new Dictionary<string, Dictionary<ColumnNames, float>>();
        
        _tmpDropdown.onValueChanged.AddListener (delegate {ValueChangeCheck ();});
        
        ReadCsv();
    }

    public  Dictionary<string, Dictionary<ColumnNames, float>> GetPresets()
    {
        return _presets;
    }
    private void ValueChangeCheck()
    {
        ImportPreset(false);
    }
    public void ImportPreset(bool IsTempSave)
    {
        string text;

        if (IsTempSave)
        {
            text = "[TempSave]";
        }
        else
        {
            //Set the preset name in the input field and dropdown UI
            text = _tmpDropdown.options[_tmpDropdown.value].text;
            Input.text = text;

            DataManager.instance.PresetName?.SetValue(text);
        }
        

        //Set all the csv value to atom's variable
        Dictionary<ColumnNames, float> dico;

        if(_presets == null)
            _presets = new Dictionary<string, Dictionary<ColumnNames, float>>();

        if (_presets.TryGetValue(text, out dico))
        {
            foreach (var keyValuePair in dico)
            {

                switch (keyValuePair.Key)
                {
                    case ColumnNames.Seed:
                        DataManager.instance.Seed.SetValue((int)keyValuePair.Value);
                        break;
                    case ColumnNames.Longueur:
                        DataManager.instance.CorridorLength.SetValue((int)keyValuePair.Value);
                        break;
                    case ColumnNames.Largeur:
                        DataManager.instance.CorridorWidth.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.Hauteur:
                        DataManager.instance.WallHeight.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.TailleDesImages:
                        DataManager.instance.ImageSize.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.NbrVirage:
                        DataManager.instance.TurnNumber.SetValue((int)keyValuePair.Value);
                        break;
                    case ColumnNames.TempsImage:
                        DataManager.instance.ImageTime.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.RandomizeImage:
                        DataManager.instance.RandomizeImage.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.Timer:
                        DataManager.instance.Timer.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.Score:
                        DataManager.instance.DisplayScore.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.FpsCamera:
                        DataManager.instance.FpsCamera.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.Shoot:
                        DataManager.instance.IsShootActivated.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.ShowEndTime:
                        DataManager.instance.ShowEndTime.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.Speed:
                        DataManager.instance.Speed.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.BreakForce:
                        DataManager.instance.BreakForce.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.Remy:
                        DataManager.instance.IsRemySelected.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.Megan:
                        DataManager.instance.IsMeganSelected.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.Mousey:
                        DataManager.instance.IsMouseySelected.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.AutoMode:
                        DataManager.instance.IsAutoMode.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.SemiAutoMode:
                        DataManager.instance.IsSemiAutoMode.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.ManualMode:
                        DataManager.instance.IsManualMode.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                    case ColumnNames.MusicVolume:
                        DataManager.instance.MusicVolume.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.SfxVolume:
                        DataManager.instance.SfxVolume.SetValue(keyValuePair.Value);
                        break;
                    case ColumnNames.CrosshairColorized:
                        DataManager.instance.IsCrosshairColorized.SetValue(keyValuePair.Value >= 0.5f);
                        break;
                }
            }
        }
    }
    private void ReadCsv()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        
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
                            //Add this preset into options of the dropdown box
                            Dropdown.OptionData option = new Dropdown.OptionData(selection);
                            _tmpDropdown.options.Add(option);
                        
                            //Fill the preset with all the varaibles
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
                            _presets[selection][(ColumnNames.TempsImage)] = f;
                            
                            f = CheckStringLength(value[7]);
                            _presets[selection][(ColumnNames.RandomizeImage)] = f;
                            
                            f = CheckStringLength(value[8]);
                            _presets[selection][(ColumnNames.TailleDesImages)] = f;

                            f = CheckStringLength(value[9]);
                            _presets[selection][(ColumnNames.Timer)] = f;

                            f = CheckStringLength(value[10]);
                            _presets[selection][(ColumnNames.Score)] = f;

                            f = CheckStringLength(value[11]);
                            _presets[selection][(ColumnNames.FpsCamera)] = f;

                            f = CheckStringLength(value[12]);
                            _presets[selection][(ColumnNames.Shoot)] = f;

                            f = CheckStringLength(value[13]);
                            _presets[selection][(ColumnNames.ShowEndTime)] = f;

                            f = CheckStringLength(value[14]);
                            _presets[selection][(ColumnNames.Speed)] = f;

                            f = CheckStringLength(value[15]);
                            _presets[selection][(ColumnNames.BreakForce)] = f;

                            f = CheckStringLength(value[16]);
                            _presets[selection][(ColumnNames.Remy)] = f;

                            f = CheckStringLength(value[17]);
                            _presets[selection][(ColumnNames.Megan)] = f;

                            f = CheckStringLength(value[18]);
                            _presets[selection][(ColumnNames.Mousey)] = f;

                            f = CheckStringLength(value[19]);
                            _presets[selection][(ColumnNames.AutoMode)] = f;

                            f = CheckStringLength(value[20]);
                            _presets[selection][(ColumnNames.SemiAutoMode)] = f;

                            f = CheckStringLength(value[21]);
                            _presets[selection][(ColumnNames.ManualMode)] = f;

                            f = CheckStringLength(value[22]);
                            _presets[selection][(ColumnNames.MusicVolume)] = f;

                            f = CheckStringLength(value[23]);
                            _presets[selection][(ColumnNames.SfxVolume)] = f;

                            f = CheckStringLength(value[24]);
                            _presets[selection][(ColumnNames.CrosshairColorized)] = f;
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
            s = s.Replace(".", ",");

        if (float.TryParse(s, out float result))
        {
            return result;
        }
        return 0;
    }
    public void UpdateDictionnary(Dictionary<string, Dictionary<ColumnNames, float>> newDico, bool resetSelect)
    {
        _presets = newDico;
        _tmpDropdown.options.Clear();
        
        Dropdown.OptionData option = new Dropdown.OptionData("");
        _tmpDropdown.options.Add(option);

        if(resetSelect)
            _tmpDropdown.value = 0;

        foreach (var keyValuePair in _presets)
        {
             option = new Dropdown.OptionData(keyValuePair.Key);
            _tmpDropdown.options.Add(option);
        }
    }
    public string GetText()
    {
      return _tmpDropdown.options[_tmpDropdown.value].text;
    }
}
