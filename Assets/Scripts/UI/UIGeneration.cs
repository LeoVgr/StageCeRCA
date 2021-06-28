using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
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

    public StringVariable atomIdentifiant;
    public BoolVariable isGameStart;
    public BoolVariable a_isPlayerLock;
    public bool hideOnGenerate;

    private Vector3 idBaseScale;
    private MazeGenerator _mazeGenerator;
    private PlayerMovement _playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        a_isPlayerLock.SetValue(true);
        _mazeGenerator = FindObjectOfType<MazeGenerator>();
        idBaseScale = inputIdentfiant.transform.localScale;
    }


    public void GenerateMaze()
    {
        if (atomIdentifiant.Value.Length < 1)
        {
            inputIdentfiant.transform.DOScale(inputIdentfiant.transform.localScale * Random.Range(1.1f, 1.5f), 0.2f)
                .OnComplete(() => inputIdentfiant.transform.DOScale(idBaseScale, 0.2f));
        }
        else
        {
            if (_mazeGenerator.GenerateMaze())
            {
                isGameStart.SetValue(true);
                if(hideOnGenerate)
                    mazeUI.SetActive(false);

                a_isPlayerLock.SetValue(false);
            }
        }
    }

    private void OnDestroy()
    {
        isGameStart.SetValue(false);
    }
}
