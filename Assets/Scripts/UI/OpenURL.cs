using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenURL : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public string url;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(url);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //gameObject.transform.DOScale(transform.localScale * 1.1f, 0.3f).SetEase(Ease.OutBounce);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce);
    }
}
