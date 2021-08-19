using System;
using System.IO;
using UnityAtoms.BaseAtoms;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace Data
{
    public class PlayerSaveData : MonoBehaviour
    {
        #region Attributs
        

        private string _startTime;
        private bool _endGame;
        private DirectoryInfo _directoryInfo;

        internal int _modelIndex
        {
            get
            {
                if (DataManager.instance.IsRemySelected.Value) return 0;
                if (DataManager.instance.IsMeganSelected.Value) return 1;
                if (DataManager.instance.IsDogSelected.Value) return 2;
                else
                {
                    Debug.LogError("Player not found !");
                    return -1;
                }
            }
        }

        #endregion

        #region Events
        private void Awake()
        {
            //Define where we will save our file
            _directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
            DataManager.instance.TargetList.Clear();
        }
        private void Start()
        {
            _startTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        #endregion

        #region Methods
        public void EndGame()
        {
            _endGame = true;

            //Save infos in data files
            FillCsv(); //Csv with all last players
            CreateNewPlayerFile(); //Specific player file
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
            text += "Type Camera;" + (DataManager.instance.FpsCamera.Value ? "FPS" : "TPS") + "\n";
            text += "Identifiant;" + DataManager.instance.IdPlayer.Value + "\n";
            text += "Temps de jeu (s);" + UIManager.instance.TimerUI.GetComponent<PlayerTimer>().GetPlayerTimer() + "\n";
            text += "Nombres d'images;" + DataManager.instance.TargetCount.Value + "\n";
            text += "Cibles manquees;" + (DataManager.instance.TargetCount.Value - DataManager.instance.TargetHit.Value) + "\n";
            text += "Longueur;" + DataManager.instance.CorridorLength.Value + "\n";
            text += "Nombre  Virages;" + DataManager.instance.TurnNumber.Value + "\n";
            text += "Images Random;" + (DataManager.instance.RandomizeImage.Value ? "Oui" : "Non") + "\n";
            text += "Taille images;" + DataManager.instance.ImageSize.Value + "\n";
            text += "Temps images;" + DataManager.instance.ImageTime.Value + "\n";
            text += "Seed;" + DataManager.instance.Seed.Value + "\n";
            text += "Preset;" + DataManager.instance.PresetName.Value + "\n";
            text += "Score;" + (DataManager.instance.Score.Value * 100) + "\n";
            text += "Tir Actif;" + (DataManager.instance.IsShootActivated.Value ? "Oui" : "Non") + "\n";
            text += "Score Affiche;" + (DataManager.instance.DisplayScore.Value ? "Oui" : "Non") + "\n";
            text += "Vitesse;" + DataManager.instance.Speed.Value + "\n";
            text += "Force frein;" + DataManager.instance.BreakForce.Value + "\n";
            text += "Viseur colorise;" + (DataManager.instance.IsCrosshairColorized.Value ? "Oui" : "Non") + "\n";
            text += "Mode deplacement;" + (DataManager.instance.IsAutoMode.Value ? "Auto" : (DataManager.instance.IsManualMode.Value ? "Manuel" : "Semi-Auto")) +
                    "\n";
            text += "Personnage;" + (DataManager.instance.IsRemySelected.Value ? "Remy" : DataManager.instance.IsMeganSelected.Value ? "Megan" : "Mousey") + "\n";
            text +=
                "\nNom image;Position image;Temps affichage;Cible a toucher ?;Cible effectivement touchee ?;Succes ?\n";

            //For each target in the level, add data
            foreach (GameObject o in DataManager.instance.TargetList.List)
            {
                text += o.GetComponent<Target>().Sprite.name + ";" +
                        o.GetComponent<Target>().TargetPosition + ";" +
                        (o.GetComponent<Target>().GetTimeToShoot()) + ";" +
                        (o.GetComponent<Target>().HasToBeShot ? "Oui" : "Non") + ";" +
                        (o.GetComponent<Target>().GetIsHit() ? "Oui" : "Non") + ";" +
                        ((o.GetComponent<Target>().HasToBeShot && o.GetComponent<Target>().GetIsHit()) ||
                         (!o.GetComponent<Target>().HasToBeShot && !o.GetComponent<Target>().GetIsHit())
                            ? "Oui"
                            : "Non") + ";" +
                        "\n";
            }

            //Create a file in the good path
            string path = "";

            if (DataManager.instance.PresetName.Value.Length > 0 && DataManager.instance.PresetName.Value!="[TempSave]")
            {
                path = DataManager.instance.PresetName.Value + "/";
                Directory.CreateDirectory(_directoryInfo + "/Players/" + DataManager.instance.PresetName.Value);
            }

            int index = Directory.GetFiles(_directoryInfo + "/Players/" + path).Length;

            string playerUniqueId = "";
            if (index > 0)
                playerUniqueId = "_" + index;

            string fullPath = _directoryInfo + "/Players/" + path + DataManager.instance.IdPlayer.Value +
                              playerUniqueId /*+ "(" + DateTime.Now.ToString("hhmm_ss") */ + ".csv";

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

            foreach (float f in DataManager.instance.TimeToShootList.List)
            {
                average += f;
            }

            average /= DataManager.instance.TimeToShootList.Count;

            text += _startTime + ";";
            text += (DataManager.instance.FpsCamera.Value ? "FPS" : "TPS") + ";";
            text += average + ";";
            text += UIManager.instance.TimerUI.GetComponent<PlayerTimer>().GetPlayerTimer() + ";";
            text += DataManager.instance.TargetCount.Value + ";";
            text += DataManager.instance.TargetCount.Value - DataManager.instance.TargetHit.Value + ";";
            text += DataManager.instance.CorridorLength.Value + ";";
            text += DataManager.instance.TurnNumber.Value + ";";
            text += DataManager.instance.ImageTime.Value + ";";
            text += (DataManager.instance.RandomizeImage.Value ? "Oui" : "Non") + ";";
            text += DataManager.instance.ImageSize.Value + ";";
            text += DataManager.instance.Seed.Value + ";";
            text += gameObject.name + ";";
            text += DataManager.instance.PresetName.Value + ";\n";
            return text;
        }
        #endregion
    }
}