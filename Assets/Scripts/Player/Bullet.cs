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
    
    public LineRenderer lineRenderer;
    public float time = 0.3f;
    public float intervalTime = 0.5f;
    public VisualEffect sparkle;
    public Vector2Variable screenShakesValues;
    public GameObject bulletImpact;
    public GameObject bulletSphere;
    
    [Header("Audio")] public AudioSource ImpactSource;
    
    private CinemachineImpulseSource _impulseSource;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _hitNormal;
    private Vector3 baseBulletScale;

    private WhiteBalance _whiteBalance;

    private Color _baseLineRendererColor;
    private static readonly int Color = Shader.PropertyToID("_Color");

    private float laserLength = 1.5f;
    private static readonly int Intensity = Shader.PropertyToID("_Intensity");

    // Start is called before the first frame update
    void Awake()
    {
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        _baseLineRendererColor = lineRenderer.material.GetColor(Color);

        baseBulletScale = bulletSphere.transform.localScale;
    }

    public void Fire(Vector3 pos1, Vector3 pos2, bool hit, Vector3 normal)
    {
        SetPostProcess();
        GenerateScreenShake();

        _startPosition = pos1;
        _endPosition = pos2;
        sparkle.transform.position = _endPosition;
        _hitNormal = normal;
        bulletSphere.transform.localScale = baseBulletScale;

        Sequence fireSequence = DOTween.Sequence();
        fireSequence.AppendCallback(() => lineRenderer.material.SetColor(Color, _baseLineRendererColor));
        fireSequence.Append(lineRenderer.material.DOColor( new Color(0,0,0,0), "_Color", time));
        fireSequence.Join(DOVirtual.Float(0.0f, 1.0f, time, DoMoveVector));
        fireSequence.Join(DOVirtual.Float(0.0f, 2.0f, time, SetWhiteBalance));
        if (hit)
        {  
            fireSequence.AppendCallback(HitEvent);
            fireSequence.Append(bulletSphere.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBounce));
        }
        fireSequence.AppendInterval(intervalTime);
        fireSequence.AppendCallback(() => gameObject.SetActive(false));
    }

    private void GenerateScreenShake()
    {
        _impulseSource.m_ImpulseDefinition.m_AmplitudeGain = screenShakesValues.Value.x;
        _impulseSource.m_ImpulseDefinition.m_FrequencyGain = screenShakesValues.Value.x;
        _impulseSource.GenerateImpulse();
        DOVirtual.DelayedCall(screenShakesValues.Value.y, ResetCiemachine);
    }
    
    

    private void HitEvent()
    {
        //VFX
        sparkle.SendEvent("OnFire");
        sparkle.transform.position += _hitNormal * 0.1f;
        //Impact Effect
        MeshRenderer impactRenderer = bulletImpact.GetComponent<MeshRenderer>();
        impactRenderer.material.SetFloat(Intensity, 1.0f);
        impactRenderer.material.DOFloat(.0f, "_Intensity", intervalTime);
        bulletImpact.transform.position = _endPosition + _hitNormal * 0.05f;
        bulletImpact.transform.rotation = Quaternion.LookRotation(_hitNormal);
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
        Vector3 endPos = _startPosition + heading.normalized * laserLength + heading * f;
        bulletSphere.transform.position = endPos;
        lineRenderer.SetPositions( new []{startPos, endPos});
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
}
