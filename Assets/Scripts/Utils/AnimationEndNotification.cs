using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndNotification : MonoBehaviour
{

    public void AnimationEnd()
    {
        GameManager.instance.Resume();
    }

    public void PlayEndAnimation()
    {
        GetComponent<Animator>().SetBool("IsOpen", false);
    }
}
