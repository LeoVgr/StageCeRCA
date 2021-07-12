using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;
// ReSharper disable StringLiteralTypo

public class PlayerSaveData : MonoBehaviour
{
    #region "Attributs"
    public BoolVariable IsPlayerLock;
    public IntVariable TargetCount;
    public IntVariable TargetHit;
    public IntVariable CorridorLength;
    public IntVariable TurnCount;
    public FloatVariable ImageTime;
    public FloatVariable ImageSize;
    public BoolVariable ImageRandom;
    public BoolVariable IsFPSBool;
    public IntVariable SeedNumber;
    public StringVariable PresetName;
    public FloatValueList TimeToShootList;
    public GameObjectValueList TargetList;
    public StringVariable IdPlayer;
    public IntVariable Score;
    public BoolVariable ScoreDisplay;
    public BoolVariable EnableFire;
    public FloatVariable TotalTimer;

    private string _startTime;   
    private bool _endGame;   
    private DirectoryInfo _directoryInfo;
    #endregion "Attributs"

    #region "Events"
    private void Awake()
    {
        //Define where we will save our file
        _directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        TargetList.Clear();
    }
    private void Start()
    {
        _startTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
    private void Update()
    {
        //Increment timer value while the game is running
        if (!IsPlayerLock.Value && !_endGame)
        {
            TotalTimer.Value += Time.deltaTime;        
        }
    }
    #endregion

    #region "Methods"
    public void EndGame()
    {
        _endGame = true;

        //Save infos in data files
        FillCsv(); //Csv with all last players
        CreateNewPlayerFile(); //Specific player file

        //Reset the timer
        TotalTimer.SetValue(.0f);
    }
    private void FillCsv()
    {
        //the line that we want to add to the csv file
        string text = "";

        //Looking for the existing file (if there is one)
        FileInfo[] allFiles = _directoryInfo.GetFiles("*.*");
        bool fileFind = false;

        foreach (FileInfo file in allFiles)
        {
            if (file.Name.Contains("PlayersData.csv") && !file.Name.Contains("meta"))
            {
                StreamReader fileText = file.OpenText();
                text = fileText.ReadToEnd();
                text = AppendString(text);
                fileText.Close();
                fileFind = true;
            }
        }

        //If not, just create one
        if (!fileFind)
        {
            text = "Date et heure;" +
                   "Type Camera;" +
                   " Temps de reaction moyen (s);" +
                   " Temps de jeu (s); " +
                   "Nombres d'images;" +
                   "Cibles manquees;" +
                   "Longueur;" +
                   "Nombre Virages;" +
                   "Temps images;" +
                   " Images Random;" +
                   " Taille images;" +
                   "seed;" +
                   "personnage;" +
                   "preset\n";

            text = AppendString(text);          
        }
        
        File.WriteAllText(_directoryInfo + "/PlayersData.csv", text);
    }
    private void CreateNewPlayerFile()
    {
        // Add data to our specific player file
        string text = "";
        text += "Date et heure;" + _startTime + "\n";
        text += "Type Camera;" + (IsFPSBool.Value ? "FPS" : "TPS") + "\n";
        text += "Identifiant;" + IdPlayer.Value + "\n";
        text += "Temps de jeu (s);" + TotalTimer.Value + "\n";
        text += "Nombres d'images;" + TargetCount.Value + "\n";
        text += "Cibles manquees;" + (TargetCount.Value - TargetHit.Value) + "\n";
        text += "Longueur;" + CorridorLength.Value + "\n";
        text += "Nombre  Virages;" + TurnCount.Value + "\n";
        text += "Images Random;" + (ImageRandom.Value ? "Oui" : "Non") + "\n";
        text += "Taille images;" + ImageSize.Value + "\n";
        text += "Temps images;" + ImageTime.Value + "\n";
        text += "seed;" + SeedNumber.Value  + "\n";
        text += "preset;" + PresetName.Value  + "\n";
        text += "score;" + (Score.Value * 100)  + "\n";
        text += "Tir Actif;" + (EnableFire.Value ? "Oui" : "Non") + "\n";
        text += "Score Affiche;" + (ScoreDisplay.Value ? "Oui" : "Non") + "\n";
        text += "Personnage;" + gameObject.name + "\n";
        text += "\nNom image;Temps affichage;Cible a toucher ?;Cible effectivement touchee ?;Succes ?\n";
        
        //For each target in the level, add data
        foreach (GameObject o in TargetList.List)
        {
            text += o.GetComponent<Target>().Sprite.name + ";" 
                + (o.GetComponent<Target>().ShowTime.Value - o.GetComponent<Target>().GetTimeToShoot()) + ";" +
                (o.GetComponent<Target>().HasToBeShot ? "Oui" : "Non") + ";" +
                (o.GetComponent<Target>().GetIsHit() ? "Oui" : "Non") + ";" +
                ((o.GetComponent<Target>().HasToBeShot && o.GetComponent<Target>().GetIsHit()) || (!o.GetComponent<Target>().HasToBeShot && !o.GetComponent<Target>().GetIsHit()) ? "Oui" : "Non") + ";" +
                "\n";
        }

        //Create a file in the good path
        string path = "";

        if (PresetName.Value.Length > 0)
        {
            path = PresetName.Value + "/";
            Directory.CreateDirectory(_directoryInfo + "/Players/" + PresetName.Value);
        }

        int index = Directory.GetFiles(_directoryInfo + "/Players/" + path).Length;

        string playerUniqueId = "";
        if (index > 0)
            playerUniqueId = "_" + index;

        string fullPath = _directoryInfo + "/Players/" + path + IdPlayer.Value + playerUniqueId /*+ "(" + DateTime.Now.ToString("hhmm_ss") */+ ".csv";

       FileStream fs = File.Create(fullPath);
       var sr = new StreamWriter(fs);
       sr.Write(text);
       sr.Close();

       _endGame = false;

    }    
    private string AppendString(string text)
    {
        //Use to add data into the string in parameter

        float average = 0.0f;
        
        foreach (float f in TimeToShootList.List)
        {
            average += f;
        }

        average /= TimeToShootList.Count;

        text += _startTime + ";";
        text += (IsFPSBool.Value ? "FPS" : "TPS") + ";";
        text += average + ";";
        text += TotalTimer.Value + ";";
        text += TargetCount.Value + ";";
        text += TargetCount.Value - TargetHit.Value + ";";
        text += CorridorLength.Value + ";";
        text += TurnCount.Value + ";";
        text += ImageTime.Value + ";";
        text += (ImageRandom.Value ? "Oui" : "Non") + ";";
        text += ImageSize.Value + ";";
        text += SeedNumber.Value + ";";
        text += gameObject.name + ";";
        text += PresetName.Value + ";\n";
        return text;
    }
    #endregion
}


