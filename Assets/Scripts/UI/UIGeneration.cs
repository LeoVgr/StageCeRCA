﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/**
 * @author : Samuel BUSSON
 * @brief : UIGeneration controls the Generate Button which start the game and generate the corridor
 * @date : 07/2020
 */
public class UIGeneration : MonoBehaviour
{

    public GameObject mazeUI;
    public GameObject inputIdentfiant;
    public SavePreset SavePreset;

    
    private Vector3 idBaseScale;
    
    // Start is called before the first frame update
    void Start()
    {
        idBaseScale = inputIdentfiant.transform.localScale;
    }


    public void Play()
    {
        if (DataManager.instance.IdPlayer.Value.Length < 1)
        {
            inputIdentfiant.transform.DOScale(inputIdentfiant.transform.localScale * Random.Range(1.1f, 1.5f), 0.2f)
                .OnComplete(() => inputIdentfiant.transform.DOScale(idBaseScale, 0.2f));
        }
        else
        {
            SavePreset.AddItemThenSave(true);
            DataManager.instance.IsTutorial.Value = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void PlayTutorial()
    {
        if (DataManager.instance.IdPlayer.Value.Length < 1)
        {
            inputIdentfiant.transform.DOScale(inputIdentfiant.transform.localScale * Random.Range(1.1f, 1.5f), 0.2f)
                .OnComplete(() => inputIdentfiant.transform.DOScale(idBaseScale, 0.2f));
        }
        else
        {
            SavePreset.AddItemThenSave(true);
            DataManager.instance.IsTutorial.Value = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
