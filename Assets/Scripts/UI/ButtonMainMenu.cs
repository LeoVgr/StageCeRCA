using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMainMenu : MonoBehaviour
{
    public void HighLight()
    {
        this.GetComponent<Animator>().SetTrigger("HighLight");
        this.GetComponent<AudioSource>().Play();
    }

    public void Normal()
    {
        this.GetComponent<Animator>().SetTrigger("Normal");
    }
}
