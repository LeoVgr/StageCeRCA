using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author : Samuel BUSSON
 * @brief : SavePreset is used to manage the CSV "corridor_csv.csv" located in the StreamingAssets folder, it contains the saved presets
 * @date : 07/2020
 */
public class SavePreset : MonoBehaviour
{
    #region "Attributs"

    public Image saveImage;
    public Image deleteImage;

    public GameObjectVariable fillPresetObject;
    
    private FillPresets fillPresets;
    private InputField _Input;

    private bool delete;

    private Dictionary<string, Dictionary<FillPresets.ColumnNames, float>> dico;
    #endregion

    #region "Events"
    private void Start()
    {
        _Input = GetComponentInChildren<InputField>();
        if(fillPresetObject.Value)
            fillPresets = fillPresetObject.Value.GetComponent<FillPresets>();
    }
    #endregion

    #region "Methods"
    public void AddItemThenSave(bool IsTempSave)
    {
        //Name of the preset
        string inputTextName;

        if (IsTempSave)
        {
            inputTextName = "[TempSave]";
        }
        else
        {
            if (_Input.text == "")
                return;

            _Input.text = _Input.text.Replace(";", ".");

            inputTextName = _Input.text;
        }

        if (fillPresetObject.Value && !fillPresets)
            fillPresets = fillPresetObject.Value.GetComponent<FillPresets>();

        dico = fillPresets.GetPresets();
        
        dico[inputTextName] = new Dictionary<FillPresets.ColumnNames, float>();
        dico[inputTextName][(FillPresets.ColumnNames.Seed)] = DataManager.instance.Seed.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Longueur)] = DataManager.instance.CorridorLength.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Largeur)] = DataManager.instance.CorridorWidth.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Hauteur)] = DataManager.instance.WallHeight.Value;
        dico[inputTextName][(FillPresets.ColumnNames.NbrVirage)] = DataManager.instance.TurnNumber.Value;
        dico[inputTextName][(FillPresets.ColumnNames.TempsImage)] = DataManager.instance.ImageTime.Value;
        dico[inputTextName][(FillPresets.ColumnNames.RandomizeImage)] = DataManager.instance.RandomizeImage.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.TailleDesImages)] = DataManager.instance.ImageSize.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Timer)] = DataManager.instance.Timer.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Score)] = DataManager.instance.DisplayScore.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.FpsCamera)] = DataManager.instance.FpsCamera.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.Shoot)] = DataManager.instance.IsShootActivated.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.ShowEndTime)] = DataManager.instance.ShowEndTime.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.Speed)] = DataManager.instance.Speed.Value;
        dico[inputTextName][(FillPresets.ColumnNames.BreakForce)] = DataManager.instance.BreakForce.Value;
        dico[inputTextName][(FillPresets.ColumnNames.Remy)] = DataManager.instance.IsRemySelected.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.Megan)] = DataManager.instance.IsMeganSelected.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.Mousey)] = DataManager.instance.IsMouseySelected.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.AutoMode)] = DataManager.instance.IsAutoMode.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.SemiAutoMode)] = DataManager.instance.IsSemiAutoMode.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.ManualMode)] = DataManager.instance.IsManualMode.Value ? 1 : 0;
        dico[inputTextName][(FillPresets.ColumnNames.MusicVolume)] = DataManager.instance.MusicVolume.Value;
        dico[inputTextName][(FillPresets.ColumnNames.SfxVolume)] = DataManager.instance.SfxVolume.Value;
        dico[inputTextName][(FillPresets.ColumnNames.CrosshairColorized)] = DataManager.instance.IsCrosshairColorized.Value ? 1 : 0;
        delete = false;

        DOVirtual.DelayedCall(Time.deltaTime, (() => DataManager.instance.PresetName?.SetValue(inputTextName)));

        SaveCSV();
    }
    public void DeleteItemThenSave()
    {
        delete = true;
        
        if(fillPresetObject.Value && !fillPresets)
            fillPresets = fillPresetObject.Value.GetComponent<FillPresets>();
        dico = fillPresets.GetPresets();
        if (dico.Remove(fillPresets.GetText()))
            SaveCSV();
    }
    private void SaveCSV()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        
        IEnumerable<FillPresets.ColumnNames> allEnumNames = Enum.GetValues(typeof(FillPresets.ColumnNames)).Cast<FillPresets.ColumnNames>();;

        string text = "Preset;";
        
        foreach (var enumName in allEnumNames)
        {
            text += GetColumnName(enumName) + ";";
        }

        text += "\n";

        foreach (var keyValuePair in dico)
        {
            text += keyValuePair.Key + ";";

            foreach (var valuePair in keyValuePair.Value)
            {
                text += valuePair.Value.ToString(CultureInfo.InvariantCulture) + ";";
            }

            text += "\n";
        }

        fillPresets.UpdateDictionnary(dico, delete);

        if (delete)
            deleteImage.DOColor(Color.white, 0.3f).SetLoops(2, LoopType.Yoyo);
        else
            saveImage.DOColor(Color.white, 0.3f).SetLoops(2, LoopType.Yoyo);
        

        File.WriteAllText(directoryInfo + "/presetsSave.csv", text);
    }
    private string GetColumnName(FillPresets.ColumnNames name)
    {
        switch (name)
        {
            case FillPresets.ColumnNames.Seed:
                return "Seed";
            case FillPresets.ColumnNames.Longueur:
                return "Longueur";
            case FillPresets.ColumnNames.Largeur:
                return "Largeur";
            case FillPresets.ColumnNames.Hauteur:
                return "Hauteur";
            case FillPresets.ColumnNames.NbrVirage:
                return "Nbr Virages";
            case FillPresets.ColumnNames.TempsImage:
                return "Temps Image";
            case FillPresets.ColumnNames.RandomizeImage:
                return "Randomize Image";
            case FillPresets.ColumnNames.TailleDesImages:
                return "Taille des images";
            case FillPresets.ColumnNames.Timer:
                return "Timer";
            case FillPresets.ColumnNames.Score:
                return "Score";
            case FillPresets.ColumnNames.FpsCamera:
                return "FpsCamera";
            case FillPresets.ColumnNames.Shoot:
                return "Shoot";
            case FillPresets.ColumnNames.ShowEndTime:
                return "ShowEndTime";
            case FillPresets.ColumnNames.Speed:
                return "Speed";
            case FillPresets.ColumnNames.BreakForce:
                return "BreakForce";
            case FillPresets.ColumnNames.Remy:
                return "Remy";
            case FillPresets.ColumnNames.Megan:
                return "Megan";
            case FillPresets.ColumnNames.Mousey:
                return "Mousey";
            case FillPresets.ColumnNames.AutoMode:
                return "AutoMode";
            case FillPresets.ColumnNames.SemiAutoMode:
                return "SemiAutoMode";
            case FillPresets.ColumnNames.ManualMode:
                return "ManualMode";
            case FillPresets.ColumnNames.MusicVolume:
                return "MusicVolume";
            case FillPresets.ColumnNames.SfxVolume:
                return "SfxVolume";
            case FillPresets.ColumnNames.CrosshairColorized:
                return "CrosshairColorized";
            default:
                return "";
        }
    }
    #endregion
}
