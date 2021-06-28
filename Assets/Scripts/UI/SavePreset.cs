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

    [Header("Data")]
    public IntVariable seed;
    public IntVariable longueur;
    public IntVariable nbrVirage;
    public FloatVariable largeur;
    public FloatVariable hauteur;
    public FloatVariable tailleDesImages;
    public FloatVariable tempsImage;
    public FloatVariable chronoImage;
    public BoolVariable  randomizeImage;

    public Image saveImage;
    public Image deleteImage;

    public GameObjectVariable fillPresetObject;
    
    public StringVariable presetName;
    
    private FillPresets fillPresets;
    private InputField _Input;

    private bool delete;

    private Dictionary<string, Dictionary<FillPresets.ColumnNames, float>> dico;

    private void Start()
    {
        _Input = GetComponentInChildren<InputField>();
        if(fillPresetObject.Value)
            fillPresets = fillPresetObject.Value.GetComponent<FillPresets>();
    }


    public void AddItemThenSave()
    {
        if (_Input.text != "")
        {
            _Input.text = _Input.text.Replace(";", ".");
            
            if(fillPresetObject.Value && !fillPresets)
                fillPresets = fillPresetObject.Value.GetComponent<FillPresets>();
            
            dico = fillPresets.GetPresets();
            
            string inputTextName = _Input.text;
            dico[inputTextName] = new Dictionary<FillPresets.ColumnNames, float>();
            dico[inputTextName][(FillPresets.ColumnNames.Seed)] = seed.Value;
            dico[inputTextName][(FillPresets.ColumnNames.Longueur)] = longueur.Value;
            dico[inputTextName][(FillPresets.ColumnNames.Largeur)] = largeur.Value;
            dico[inputTextName][(FillPresets.ColumnNames.Hauteur)] = hauteur.Value;
            dico[inputTextName][(FillPresets.ColumnNames.NbrVirage)] = nbrVirage.Value;
            dico[inputTextName][(FillPresets.ColumnNames.TempsImage)] = tempsImage.Value;
            dico[inputTextName][(FillPresets.ColumnNames.RandomizeImage)] = randomizeImage.Value ? 1 : 0;
            dico[inputTextName][(FillPresets.ColumnNames.TailleDesImages)] = tailleDesImages.Value;
            dico[inputTextName][(FillPresets.ColumnNames.Timer)] = chronoImage.Value;
            delete = false;
            
            DOVirtual.DelayedCall(Time.deltaTime, (() => presetName?.SetValue(inputTextName)));
            
            SaveCSV();
        }
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
            default:
                return "";
        }
    }
}
