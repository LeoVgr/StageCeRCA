using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    #region "Attributs"
    public FloatVariable LifeAmount;
    #endregion

    #region "Events"
    private void Start()
    {
        LifeAmount.Reset();
    }
    private void Update()
    {
        LifeAmount.Value += Time.deltaTime * 3;   
    }
    #endregion

    #region "Methods"
    public void GetHurt()
    {
        LifeAmount.Value = Mathf.Max(0, LifeAmount.Value - 10);

        //Find random pos
        int x,y;
        do
        {    
            x = Random.Range(0, Screen.width);
        } while (x > Screen.width/4f && x< Screen.width * 3f/4f);

        do
        {
            y = Random.Range(0, Screen.height);
        } while (y > Screen.width / 4f && y < Screen.width * 3f / 4f);


        //Add image hurt
        UIManager.instance.AddImageToHurtScreen(x,y);
    }
    #endregion
}
