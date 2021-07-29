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

        //Allow us to first choose a position and juste random the other (ink has to be on screen's side)
        int randomFirstChoice = Random.Range(0,2);

        if (randomFirstChoice ==0)
        {
            do
            {
                x = (int)Random.Range(-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f, UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f);
            } while (!((x > (-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f) && x < (-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f) * 3f / 4f) || (x > (UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f) * 3f / 4f) && x < (UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f)));
            
            y = (int)Random.Range(-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f, UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f);
            

        }
        else
        {
            
            x = (int)Random.Range(-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f, UIManager.instance.Canvas.GetComponent<RectTransform>().rect.width / 2f);
           
            do
            {
                y = (int)Random.Range(-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f, UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f);
            } while (!((y > (-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f) && y < (-UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f) * 3f / 4f) || (y > (UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f) * 3f / 4f) && y < (UIManager.instance.Canvas.GetComponent<RectTransform>().rect.height / 2f)));

        }    

        //Add image hurt
        UIManager.instance.AddImageToHurtScreen(x,y);
    }
    #endregion
}
