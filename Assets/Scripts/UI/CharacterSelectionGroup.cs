using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionGroup : MonoBehaviour
{    
    public GameObject RemyCharacterPreview;
    public GameObject MeganCharacterPreview;
    public GameObject MouseyCharacterPreview;

    public Toggle ToggleRemy;
    public Toggle ToggleMegan;
    public Toggle ToggleMousey;

    private void Awake()
    {
        //Add event to change preview when a toggle is clicked
        ToggleRemy.onValueChanged.AddListener(delegate { SetRemyAsCharacter(ToggleRemy.isOn); });
        ToggleMegan.onValueChanged.AddListener(delegate { SetMeganAsCharacter(ToggleMegan.isOn); });
        ToggleMousey.onValueChanged.AddListener(delegate { SetMouseyAsCharacter(ToggleMousey.isOn); });
    }
    private void OnDestroy()
    {
        ToggleRemy.onValueChanged.RemoveListener(delegate { SetRemyAsCharacter(ToggleRemy.isOn); });
        ToggleMegan.onValueChanged.RemoveListener(delegate { SetMeganAsCharacter(ToggleMegan.isOn); });
        ToggleMousey.onValueChanged.RemoveListener(delegate { SetMouseyAsCharacter(ToggleMousey.isOn); });
    }
    private void Start()
    {
        //Enable or disable previews at the launch with value of atom's variables
        SetMouseyAsCharacter(DataManager.instance.IsMouseySelected.Value);
        SetMeganAsCharacter(DataManager.instance.IsMeganSelected.Value);
        SetRemyAsCharacter(DataManager.instance.IsRemySelected.Value);
    }

    private void SetMeganAsCharacter(bool isMeganCharacter)
    {
        MeganCharacterPreview.SetActive(isMeganCharacter);     
    }
    private void SetRemyAsCharacter(bool isRemyCharacter)
    {
        RemyCharacterPreview.SetActive(isRemyCharacter);       
    }
    private void SetMouseyAsCharacter(bool isMouseyCharacter)
    {
        MouseyCharacterPreview.SetActive(isMouseyCharacter);        
    }
}
