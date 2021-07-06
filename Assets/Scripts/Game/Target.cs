using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;


/**
 * @author : Samuel BUSSON
 * @brief : Class Target is a target which player can shoot, target shows up if player see it and if he's within the range
 * @date : 07/2020
 */

public class Target : MonoBehaviour
{
    #region "Attributs"
    [HideInInspector()]
    public Direction Direction;
    [HideInInspector()]
    public Sprite Sprite;

    [Header("FX")] 
    public Text ScoreUpText;
    public Sprite SplashImage;

    [Header("Atoms Variables")]
    public BoolVariable HasToBeShot;
    public FloatVariable ShowTime;
    public FloatVariable DistanceToShow;
    public FloatValueList TimeToShootList;
    public IntVariable Score;
    public IntVariable TargetCount;
    public IntVariable TargetHit;
    public GameObjectValueList TargetList;
    public BoolVariable DisplayScore;
    public GameObjectVariable Player;
    
    [Header("Atoms Events")]
    public IntEvent PlayerWaypointChange;
    
    [Header("Atoms Constant")]
    public Vector2Constant WidthHeight;
    
    [Header("Sound Effect")]
    [FMODUnity.EventRef][SerializeField]
    private string _hit;
    [FMODUnity.EventRef][SerializeField]
    private string _miss;
    
    [HideInInspector()]
    public int WayPointIndex;
    [HideInInspector()]
    public bool IsNegateImage;

    private List<Collider> _colliders;
    private SpriteRenderer _spriteRenderer;
    private float _bounceDuration = 0.5f;
    private bool _isHit;

    private bool _isShown;
    private bool _canShow;
    private MeshRenderer _meshRenderer;

    private float _time;
    private float _endTimeValue;
    private bool _loose = false;
    private static readonly int ColorMat = Shader.PropertyToID("_Color");
    #endregion

    #region "Events"
    void Start()
    {
        switch (Direction)
        {
            case Direction.North:
                transform.eulerAngles =  Vector3.zero;
                break;
            case Direction.East:
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case Direction.South:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case Direction.West:
                transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case Direction.Undefined:
                transform.eulerAngles = Vector3.zero;
                break;
        }
        
        TargetList.Add(gameObject);

        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        TargetCount?.SetValue(TargetCount.Value + 1);
        
        TimeToShootList.Clear();

        _colliders = GetComponentsInChildren<Collider>().ToList();
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer)
        {
            _spriteRenderer.sprite = Sprite;
            GetComponentInChildren<SpriteRenderer>().size = WidthHeight.Value;
            _spriteRenderer.transform.localPosition = -_spriteRenderer.transform.localPosition;
        }
        
        PlayerWaypointChange.Register(CheckDistanceToPlayer);
        
        ScoreUpText.material = Instantiate(ScoreUpText.material);

        CheckDistanceToPlayer(0);

    }
    private void Update()
    {
        if (!_loose && !_isHit)
        {
            if (_canShow && !_isShown && _meshRenderer.isVisible)
            {
                Vector3 right = transform.right;
                Vector3 position = transform.position;
                
                Vector3 origin = position + (IsNegateImage ? -right : right);
                Vector3 direction = Player.Value.transform.position + Vector3.up - position +
                                    (IsNegateImage ? right : -right);

                if (Physics.Raycast(origin, direction, out RaycastHit hitinfo))
                {
                    if (hitinfo.collider.gameObject == Player.Value)
                    {
                        Show();
                    }
                }
            }

            if (_isShown)
            {
                _time += Time.deltaTime;
            }
        }
    }
    #endregion

    #region "Methods"
    private void CheckDistanceToPlayer(int playerPosition)
    {
        if (!_loose && !_isHit)
        {
            if (!_isShown && Mathf.Abs(WayPointIndex - playerPosition) < DistanceToShow.Value)
            {
                _canShow = true;
            }
        }
    }
    private void Show()
    {
        if (!_loose && !_isHit)
        {
            _isShown = true;
            _spriteRenderer.transform.DOLocalMove(-_spriteRenderer.transform.localPosition, 0.5f)
                .SetEase(Ease.InCubic);
        
            foreach (Collider collider1 in _colliders)
            {
                collider1.enabled = true;
            }
        
            if (ShowTime.Value > 0.1f)
                DOVirtual.DelayedCall(ShowTime.Value, Hide);
        }
    }
    private void Hide()
    {
        if (!_loose && !_isHit)
        {
            FMODUnity.RuntimeManager.PlayOneShot(_miss, transform.position);
            
            Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
            var transform1 = parent.transform;
            Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
            parent.transform.SetParent(null);
            parent.transform.position = worldCanvasPosition;

            if (Score.Value > 0 && DisplayScore.Value)
            {
                Score.SetValue(Score.Value - 1);
                ScoreUpText.text = "-100";
                ScoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0));
                Sequence scoreUpSequence = DOTween.Sequence();
                scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.red * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
                scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 1.5f, 0.2f));
                scoreUpSequence.Append(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.5f, 0.5f).SetEase(Ease.OutBounce));
                scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBounce));
                scoreUpSequence.AppendCallback(() =>  Destroy(parent.gameObject));
            }

            _isShown = false;
            _spriteRenderer.transform.DOLocalMove(-_spriteRenderer.transform.localPosition, 0.3f)
                .SetEase(Ease.InCubic);
            
            GetComponentInChildren<MeshRenderer>().material.DOColor(Color.red * 2.0f, "_Color", 1f)
                .SetEase(Ease.OutBounce);
            
            foreach (Collider collider1 in _colliders)
            {
                collider1.enabled = false;
            }

            _time = 0.0f;
            _endTimeValue = 0.0f;
            TimeToShootList.Add(_time);
            _loose = true;
        }  
    }
    public void Hit()
    {
        if (!_isHit && _isShown)
        {
            FMODUnity.RuntimeManager.PlayOneShot(_hit, transform.position);
            
            Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
            var transform1 = parent.transform;
            Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
            parent.transform.SetParent(null);
            parent.transform.position = worldCanvasPosition;

            _isHit = true;
            GetComponentInChildren<MeshRenderer>().material.DOColor(Color.green * 2.0f, "_Color", 1f)
                .SetEase(Ease.OutBounce);


            TimeToShootList.Add(_time);

            _endTimeValue = _time;

            switch (Direction)
            {
                case Direction.North:
                    transform.DORotate(new Vector3(0.0f,  IsNegateImage ? 85 : -85, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.South:
                    transform.DORotate(new Vector3(0.0f, IsNegateImage ? -95 : 95, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.East:
                    transform.DORotate(new Vector3(0.0f, IsNegateImage ? 175 : 5, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.West:
                    transform.DORotate(new Vector3(0.0f, IsNegateImage ? -5 : -175, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
            }

            Sequence changeImage = DOTween.Sequence();
            
            changeImage.Append( _spriteRenderer.DOColor(new Color(0, 0.25f, 0.25f, 1), 0.5f).SetEase(Ease.OutBounce));
            changeImage.AppendCallback(() => _spriteRenderer.sprite = SplashImage);
            changeImage.Append( _spriteRenderer.DOColor(Color.white, 0.5f).SetEase(Ease.OutBounce));

            TargetHit?.SetValue(TargetHit.Value + 1);

            if (DisplayScore.Value)
            {
                Score.SetValue(Score.Value + 1);
                ScoreUpText.text = "+100";
                ScoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0));
                Sequence scoreUpSequence = DOTween.Sequence();
                scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.green * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
                scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 1.5f, 0.5f));
                scoreUpSequence.Join(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.2f, 0.5f).SetEase(Ease.OutBounce));
                scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce));
                scoreUpSequence.AppendCallback(() => 
                    ScoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0)));
                scoreUpSequence.AppendCallback(() =>  Destroy(parent.gameObject));
            }

            foreach (Collider collider1 in _colliders)
            {
                collider1.enabled = false;
            }
            
            
        }
    }
    public bool IsHit()
    {
        return _isHit;
    }
    public float GetTimeToShoot()
    {
        return _endTimeValue;
    }
    #endregion

}
