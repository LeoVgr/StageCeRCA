using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;
// ReSharper disable StringLiteralTypo

public class PlayerSaveData : MonoBehaviour
{
    public BoolVariable a_IsPlayerLock;
    public IntVariable targetCount;
    public IntVariable targetHit;
    public IntVariable corridorLength;
    public IntVariable turnCount;
    public FloatVariable imageTime;
    public FloatVariable imageSize;
    public BoolVariable imageRandom;
    public BoolVariable isFPSBool;
    public IntVariable seedNumber;
    public StringVariable presetName;
    public FloatValueList timeToShootList;
    public GameObjectValueList targetList;
    public StringVariable idPlayer;
    public IntVariable score;
    public BoolVariable scoreDisplay;
    public BoolVariable enableFire;
    public FloatVariable totalTimer;

    private string _startTime;
    
    private bool _endGame;
    
    private DirectoryInfo _directoryInfo;


    private void Awake()
    {
        _directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        targetList.Clear();
    }

    private void Start()
    {
        _startTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void Update()
    {
        if (!a_IsPlayerLock.Value && !_endGame)
        {
            totalTimer.Value += Time.deltaTime;
        }
    }

    public void EndGame()
    {
        _endGame = true;
        FillCsv();
        CreateNewPlayerFile();
        totalTimer.SetValue(.0f);
    }

    private void FillCsv()
    {
        FileInfo[] allFiles = _directoryInfo.GetFiles("*.*");

        string text = "";

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
        string text = "";
        text += "Date et heure;" + _startTime + "\n";
        text += "Type Camera;" + (isFPSBool.Value ? "FPS" : "TPS") + "\n";
        text += "Identifiant;" + idPlayer.Value + "\n";
        text += "Temps de jeu (s);" + totalTimer.Value + "\n";
        text += "Nombres d'images;" + targetCount.Value + "\n";
        text += "Cibles manquees;" + (targetCount.Value - targetHit.Value) + "\n";
        text += "Longueur;" + corridorLength.Value + "\n";
        text += "Nombre  Virages;" + turnCount.Value + "\n";
        text += "Images Random;" + (imageRandom.Value ? "Oui" : "Non") + "\n";
        text += "Taille images;" + imageSize.Value + "\n";
        text += "Temps images;" + imageTime.Value + "\n";
        text += "seed;" + seedNumber.Value  + "\n";
        text += "preset;" + presetName.Value  + "\n";
        text += "score;" + (score.Value * 100)  + "\n";
        text += "Tir Actif;" + (enableFire.Value ? "Oui" : "Non") + "\n";
        text += "Score Affiche;" + (scoreDisplay.Value ? "Oui" : "Non") + "\n";
        text += "Personnage;" + gameObject.name + "\n";
        
        foreach (GameObject o in targetList.List)
        {
            text += o.GetComponent<Target>().sprite.name + ";" + o.GetComponent<Target>().GetTimeToShoot() + "\n";
        }

        string path = "";

        if (presetName.Value.Length > 0)
        {
            path = presetName.Value + "/";
            Directory.CreateDirectory(_directoryInfo + "/Players/" + presetName.Value);
        }

        int index = Directory.GetFiles(_directoryInfo + "/Players/" + path).Length;

        string playerUniqueId = "";
        if (index > 0)
            playerUniqueId = "_" + index;

        string fullPath = _directoryInfo + "/Players/" + path + idPlayer.Value + playerUniqueId /*+ "(" + DateTime.Now.ToString("hhmm_ss") */+ ".csv";

       FileStream fs = File.Create(fullPath);
       var sr = new StreamWriter(fs);
       sr.Write(text);
       sr.Close();

       _endGame = false;

    }
    
    private string AppendString(string text)
    {
        float average = 0.0f;
        
        foreach (float f in timeToShootList.List)
        {
            average += f;
        }

        average /= timeToShootList.Count;

        text += _startTime + ";";
        text += (isFPSBool.Value ? "FPS" : "TPS") + ";";
        text += average + ";";
        text += totalTimer.Value + ";";
        text += targetCount.Value + ";";
        text += targetCount.Value - targetHit.Value + ";";
        text += corridorLength.Value + ";";
        text += turnCount.Value + ";";
        text += imageTime.Value + ";";
        text += (imageRandom.Value ? "Oui" : "Non") + ";";
        text += imageSize.Value + ";";
        text += seedNumber.Value + ";";
        text += gameObject.name + ";";
        text += presetName.Value + ";\n";
        return text;
    }
}


