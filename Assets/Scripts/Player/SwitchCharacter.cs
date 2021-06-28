using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{

    public GameObject remyCharacter;
    public GameObject meganCharacter;
    public GameObject mouseyCharacter;
    
    
    public GameObject remyCharacterPreview;
    public GameObject meganCharacterPreview;
    public GameObject mouseyCharacterPreview;

    public BoolEvent isCharacterRemyEvent;
    public BoolEvent isCharacterMeganEvent;
    public BoolEvent isCharacterMouseyEvent;

    public GameObjectVariable playerGameObject;
    
    
    // Start is called before the first frame update
    void Start()
    {
        isCharacterMeganEvent.Register(setMeganAsCharacter);
        isCharacterRemyEvent.Register(setRemyAsCharacter);
        isCharacterMouseyEvent.Register(setMouseyAsCharacter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setMeganAsCharacter(bool isMeganCharacter)
    {
        meganCharacter.SetActive(isMeganCharacter);
        meganCharacterPreview.SetActive(isMeganCharacter);

        if (isMeganCharacter)
            SetPlayerVariable(meganCharacter);
    }
    
    void setRemyAsCharacter(bool isRemyCharacter)
    {
        remyCharacter.SetActive(isRemyCharacter);
        remyCharacterPreview.SetActive(isRemyCharacter);
        
        if (isRemyCharacter)
            SetPlayerVariable(remyCharacter);
    }
    
    void setMouseyAsCharacter(bool isMouseyCharacter)
    {
        mouseyCharacter.SetActive(isMouseyCharacter);
        mouseyCharacterPreview.SetActive(isMouseyCharacter);
        
        if (isMouseyCharacter)
            SetPlayerVariable(mouseyCharacter);
    }

    private void SetPlayerVariable(GameObject go)
    {
        playerGameObject.SetValue(go.GetComponentInChildren<PlayerMovement>().gameObject);
    }
}
