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
    [HideInInspector()]
    public Direction direction;
    [HideInInspector()]
    public Sprite sprite;

    [Header("FX")] 
    public Text scoreUpText;
    public Sprite splashImage;
    
    [Header("Atoms Variables")]
    public FloatVariable showTime;
    public FloatVariable distanceToShow;
    public FloatValueList timeToShootList;
    public IntVariable score;
    public IntVariable targetCount;
    public IntVariable targetHit;
    public GameObjectValueList targetList;
    public BoolVariable displayScore;
    public GameObjectVariable player;
    
    [Header("Atoms Events")]
    public IntEvent playerWaypointChange;
    
    [Header("Atoms Constant")]
    public Vector2Constant widthHeight;
    
    [Header("Sound Effect")]
    [FMODUnity.EventRef][SerializeField]
    private string hit;
    [FMODUnity.EventRef][SerializeField]
    private string miss;
    
    [HideInInspector()]
    public int wayPointIndex;
    [HideInInspector()]
    public bool isNegateImage;

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

    // Start is called before the first frame update
    void Start()
    {
        switch (direction)
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
        
        targetList.Add(gameObject);

        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        targetCount?.SetValue(targetCount.Value + 1);
        
        timeToShootList.Clear();

        _colliders = GetComponentsInChildren<Collider>().ToList();
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer)
        {
            _spriteRenderer.sprite = sprite;
            GetComponentInChildren<SpriteRenderer>().size = widthHeight.Value;
            _spriteRenderer.transform.localPosition = -_spriteRenderer.transform.localPosition;
        }
        
        playerWaypointChange.Register(CheckDistanceToPlayer);
        
        scoreUpText.material = Instantiate(scoreUpText.material);

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
                
                Vector3 origin = position + (isNegateImage ? -right : right);
                Vector3 direction = player.Value.transform.position + Vector3.up - position +
                                    (isNegateImage ? right : -right);

                if (Physics.Raycast(origin, direction, out RaycastHit hitinfo))
                {
                    if (hitinfo.collider.gameObject == player.Value)
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

    private void CheckDistanceToPlayer(int playerPosition)
    {
        if (!_loose && !_isHit)
        {
            if (!_isShown && Mathf.Abs(wayPointIndex - playerPosition) < distanceToShow.Value)
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
        
            if (showTime.Value > 0.1f)
                DOVirtual.DelayedCall(showTime.Value, Hide);
        }
    }

    private void Hide()
    {
        if (!_loose && !_isHit)
        {
            FMODUnity.RuntimeManager.PlayOneShot(miss, transform.position);
            
            Canvas parent = scoreUpText.GetComponentInParent<Canvas>();
            var transform1 = parent.transform;
            Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
            parent.transform.SetParent(null);
            parent.transform.position = worldCanvasPosition;

            if (score.Value > 0 && displayScore.Value)
            {
                score.SetValue(score.Value - 1);
                scoreUpText.text = "-100";
                scoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0));
                Sequence scoreUpSequence = DOTween.Sequence();
                scoreUpSequence.Append(scoreUpText.material.DOColor(Color.red * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
                scoreUpSequence.Join(scoreUpText.transform.DOMove(transform1.position + Vector3.up * 1.5f, 0.2f));
                scoreUpSequence.Append(scoreUpText.transform.DOScale(scoreUpText.transform.localScale + scoreUpText.transform.localScale * 1.5f, 0.5f).SetEase(Ease.OutBounce));
                scoreUpSequence.Append(scoreUpText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBounce));
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
            timeToShootList.Add(_time);
            _loose = true;
        }  
    }

    public void Hit()
    {
        if (!_isHit && _isShown)
        {
            FMODUnity.RuntimeManager.PlayOneShot(hit, transform.position);
            
            Canvas parent = scoreUpText.GetComponentInParent<Canvas>();
            var transform1 = parent.transform;
            Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
            parent.transform.SetParent(null);
            parent.transform.position = worldCanvasPosition;

            _isHit = true;
            GetComponentInChildren<MeshRenderer>().material.DOColor(Color.green * 2.0f, "_Color", 1f)
                .SetEase(Ease.OutBounce);


            timeToShootList.Add(_time);

            _endTimeValue = _time;

            switch (direction)
            {
                case Direction.North:
                    transform.DORotate(new Vector3(0.0f,  isNegateImage ? 85 : -85, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.South:
                    transform.DORotate(new Vector3(0.0f, isNegateImage ? -95 : 95, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.East:
                    transform.DORotate(new Vector3(0.0f, isNegateImage ? 175 : 5, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
                case Direction.West:
                    transform.DORotate(new Vector3(0.0f, isNegateImage ? -5 : -175, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
                    break;
            }

            Sequence changeImage = DOTween.Sequence();
            
            changeImage.Append( _spriteRenderer.DOColor(new Color(0, 0.25f, 0.25f, 1), 0.5f).SetEase(Ease.OutBounce));
            changeImage.AppendCallback(() => _spriteRenderer.sprite = splashImage);
            changeImage.Append( _spriteRenderer.DOColor(Color.white, 0.5f).SetEase(Ease.OutBounce));

            targetHit?.SetValue(targetHit.Value + 1);

            if (displayScore.Value)
            {
                score.SetValue(score.Value + 1);
                scoreUpText.text = "+100";
                scoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0));
                Sequence scoreUpSequence = DOTween.Sequence();
                scoreUpSequence.Append(scoreUpText.material.DOColor(Color.green * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
                scoreUpSequence.Join(scoreUpText.transform.DOMove(transform1.position + Vector3.up * 1.5f, 0.5f));
                scoreUpSequence.Join(scoreUpText.transform.DOScale(scoreUpText.transform.localScale + scoreUpText.transform.localScale * 1.2f, 0.5f).SetEase(Ease.OutBounce));
                scoreUpSequence.Append(scoreUpText.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce));
                scoreUpSequence.AppendCallback(() => 
                    scoreUpText.material.SetColor(ColorMat, new Color(0,0,0,0)));
                scoreUpSequence.AppendCallback(() =>  Destroy(parent.gameObject));
            }

            foreach (Collider collider1 in _colliders)
            {
                collider1.enabled = false;
            }
            
            
        }
    }

    /**
     * @brief : Debug fonction to see a the ray between target and player
     */
    private void OnDrawGizmosSelected()
    {
      /*  Gizmos.color = Color.red;  
        Vector3 origin = transform.position + (isNegateImage ? -transform.right : transform.right);
        Vector3 direction = _player.transform.position + Vector3.up - transform.position + (isNegateImage ? transform.right : -transform.right);;
        Gizmos.DrawRay(origin, direction * 3.0f);*/
    }

    public bool IsHit()
    {
        return _isHit;
    }

    public float GetTimeToShoot()
    {
        return _endTimeValue;
    }
    
    
}
