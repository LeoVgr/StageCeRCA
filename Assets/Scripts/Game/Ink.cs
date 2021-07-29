using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ink : MonoBehaviour
{
    public float TimeOnScreen = 5f;
    public float MaxYScale = 1.5f;

    private float _timer = 0;
    private bool _end = false;
    private float _initialYScale;

    private void Start()
    {
        _initialYScale = transform.localScale.y;
        transform.DOPunchScale(transform.localScale * 0.1f, 1f);
    }
    private void Update()
    {
        //Update timer
        _timer += Time.deltaTime;

        //Percent time
        float percent = ( _timer / TimeOnScreen);

        //Change scale
        this.transform.localScale = new Vector3(transform.localScale.x, _initialYScale + MaxYScale*percent,transform.localScale.z);

        //Change position
        this.transform.position += new Vector3(0,Time.deltaTime * -20f,0);

        if (_end)
        {
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, this.GetComponent<Image>().color.a - Time.deltaTime);

            if(this.GetComponent<Image>().color.a <= 0)
            {
                Destroy(gameObject);
            }
        }


        if (_timer >= TimeOnScreen)
        {
            _end = true;
        }
       

    }
}
