using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

/**
 * @author : Samuel BUSSON
 * @brief : Bullet is the ammo fired from the player's gun
 * @date : 07/2020
 */
public class Bullet : MonoBehaviour
{
    #region "Attribut"
    public LineRenderer LineRendererComponent;
    public float Time = 0.3f;
    public float IntervalTime = 0.5f;   
    public GameObject BulletImpact;
    public float ImpactLife = 10f;
    public GameObject BulletSphere;
    
    [Header("Audio")] public AudioSource ImpactSource;
    
    private CinemachineImpulseSource _impulseSource;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _hitNormal;
    private Vector3 _baseBulletScale;
    private WhiteBalance _whiteBalance;
    private Color _baseLineRendererColor;   
    private float _laserLength = 1.5f;
    private static readonly int _color = Shader.PropertyToID("_Color");
    private static readonly int _intensity = Shader.PropertyToID("_Intensity");
    #endregion

    #region "Events"
    // Start is called before the first frame update
    void Awake()
    {
        //Get references
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        _baseLineRendererColor = LineRendererComponent.material.GetColor(_color);

        _baseBulletScale = BulletSphere.transform.localScale;
    }
    #endregion

    #region "Methods"
    public void Fire(Vector3 pos1, Vector3 pos2, bool hit, Vector3 normal)
    {
        SetPostProcess();
        GenerateScreenShake();

        _startPosition = pos1;
        _endPosition = pos2;
        _hitNormal = normal;
        BulletSphere.transform.localScale = _baseBulletScale;

        Sequence fireSequence = DOTween.Sequence();
        fireSequence.AppendCallback(() => LineRendererComponent.material.SetColor(_color, _baseLineRendererColor));
        fireSequence.Append(LineRendererComponent.material.DOColor( new Color(0,0,0,0), "_Color", Time));
        fireSequence.Join(DOVirtual.Float(0.0f, 1.0f, Time, DoMoveVector));
        fireSequence.Join(DOVirtual.Float(0.0f, 2.0f, Time, SetWhiteBalance));
        if (hit)
        {  
            fireSequence.AppendCallback(HitEvent);
            fireSequence.Append(BulletSphere.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBounce));
        }
        fireSequence.AppendInterval(IntervalTime);
        fireSequence.AppendCallback(() => gameObject.SetActive(false));
    }
    private void GenerateScreenShake()
    {
        _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = DataManager.instance.ScreenShakesValues.Value.x;
        _impulseSource.m_ImpulseDefinition.m_FrequencyGain = DataManager.instance.ScreenShakesValues.Value.x;
        _impulseSource.GenerateImpulse();
        DOVirtual.DelayedCall(DataManager.instance.ScreenShakesValues.Value.y, ResetCiemachine);
    }      
    private void HitEvent()
    {
        //Impact Effect
        var bulletImpact = Instantiate(BulletImpact, _endPosition, Quaternion.LookRotation(_hitNormal));
        //Life time of impact;
        Destroy(bulletImpact,ImpactLife);
        //AUDIO
        ImpactSource.Play();
    }
    private void ResetCiemachine()
    {
        _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = .0f;
        _impulseSource.m_ImpulseDefinition.m_FrequencyGain = .0f;
    }
    private void DoMoveVector(float f)
    {
        Vector3 heading = (_endPosition - _startPosition);
        Vector3 startPos = _startPosition + heading * f;
        Vector3 endPos = _startPosition + heading.normalized * _laserLength + heading * f;
        BulletSphere.transform.position = endPos;
        LineRendererComponent.SetPositions( new []{startPos, endPos});
    }
    private void SetWhiteBalance(float f)
    {
        float val = f;
        if (f > 1.0f)
            val = 2.0f - f;
        

        if (_whiteBalance)
            _whiteBalance.temperature.value = val * 15.0f;
    }   
    private void SetPostProcess()
    {
        if (_whiteBalance == null)
        {
            FindObjectOfType<Volume>()?.profile.TryGet(out _whiteBalance);
        }
    }
    #endregion
}
