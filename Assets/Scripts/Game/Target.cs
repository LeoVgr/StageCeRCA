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

    [Header("Sound Effect")] 
    public AudioSource AudioValidate;
    public AudioSource AudioMiss;
    public AudioSource AudioSpawn;
    public AudioSource AudioDespawn;
    
    
    [HideInInspector()]
    public int WayPointIndex;
    [HideInInspector()]
    public bool IsNegateImage;
    public bool HasToBeShot;
    public ImageLoc TargetLocation;

    private List<Collider> _colliders;
    private SpriteRenderer _spriteRenderer;
    private float _bounceDuration = 0.5f;
    private bool _hasBeenShown = false;
    private bool _isHit = false;

    private bool _isShown;
    private bool _canShow;
    private MeshRenderer _meshRenderer;

    private float _time;
    private float _endTimeValue;
    private static readonly int ColorMat = Shader.PropertyToID("_Color");
    #endregion

    #region "Events"
    protected virtual void Start()
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
        
        DataManager.instance.TargetList.Add(gameObject);

        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        DataManager.instance.TargetCount?.SetValue(DataManager.instance.TargetCount.Value + 1);

        DataManager.instance.TimeToShootList.Clear();

        _colliders = GetComponentsInChildren<Collider>().ToList();
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer)
        {
            _spriteRenderer.sprite = Sprite;
            GetComponentInChildren<SpriteRenderer>().size = DataManager.instance.WidthHeight.Value;
            _spriteRenderer.transform.localPosition = -_spriteRenderer.transform.localPosition;
        }

        DataManager.instance.PlayerWaypointChange.Register(CheckDistanceToPlayer);
        
        ScoreUpText.material = Instantiate(ScoreUpText.material);

        CheckDistanceToPlayer(0);

        if (this.GetComponent<Target>())
        {
            //Setup up if image has to be shot or not depending their name (should contain N)
            HasToBeShot = true;
            if (Sprite.name.Split('_').Length >= 2 && Sprite.name.Split('_')[1].Contains("N"))
            {
                HasToBeShot = false;
            }
        }
        else
        {
            HasToBeShot = true;
        }
        

    }
    protected virtual void Update()
    {
        //If the target hasn't been shown yet
        if (!_hasBeenShown)
        {
            if (_canShow && !_isShown && _meshRenderer.isVisible)
            {
                Vector3 right = transform.right;
                Vector3 position = transform.position;
                
                Vector3 origin = position + (IsNegateImage ? -right : right);
                Vector3 direction = DataManager.instance.Player.Value.transform.position + Vector3.up - position +
                                    (IsNegateImage ? right : -right);

                if (Physics.Raycast(origin, direction, out RaycastHit hitinfo))
                {
                    if (hitinfo.collider.gameObject == DataManager.instance.Player.Value)
                    {
                        Show();
                    }
                }
            }

            //Update the time that the target is shown
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
        if (!_hasBeenShown)
        {
            if (!_isShown && Mathf.Abs(WayPointIndex - playerPosition) < DataManager.instance.DistanceToShow.Value)
            {
                _canShow = true;
            }
        }
    }
    private void Show()
    {
        if (!_hasBeenShown)
        {
            _isShown = true;
            _spriteRenderer.transform.DOLocalMove(-_spriteRenderer.transform.localPosition, 0.5f)
                .SetEase(Ease.InCubic);
        
            foreach (Collider collider1 in _colliders)
            {
                collider1.enabled = true;
            }
        
            if (DataManager.instance.ImageTime.Value > 0.1f)
                DOVirtual.DelayedCall(DataManager.instance.ImageTime.Value, Hide);
            
            AudioSpawn.Play();
        }
    }
    private void Hide()
    {
        //When a target hasn't be shot in time, check if it was a good or bad move
        if (!_hasBeenShown)
        {
            if (HasToBeShot)
            {
                NoHitBad();
            }
            else
            {
                NoHitGood();
            }

            //Set some variables
            _hasBeenShown = true;
            
            AudioDespawn.Play();
        }  
    }
    public void Hit()
    {
        //When a target is hit, check if it hasn't be already hit 
        if (!_isHit && _isShown)
        {
            //Check if this target has to be shot
            if (HasToBeShot)
            {
                HitGood();
            }
            else
            {
                HitBad();
            }

            //Set some variables
            _hasBeenShown = true;
        }
    }
    public void NoHitGood()
    {
        //Disable colliders
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        //Update target's status
        _isShown = false;
        _isHit = false;
        _endTimeValue = _time;

        //Register some stats for further purpose (save data of player)
        DataManager.instance.TimeToShootList.Add(_time);
        DataManager.instance.TargetHit?.SetValue(DataManager.instance.TargetHit.Value + 1);

        //Play sound
        AudioValidate.Play();

        //Play animation
        Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
        var transform1 = parent.transform;
        Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
        parent.transform.SetParent(null);
        parent.transform.position = worldCanvasPosition;      
       
        if(_spriteRenderer)
            _spriteRenderer.transform.DOLocalMove(-_spriteRenderer.transform.localPosition, 0.3f)
                .SetEase(Ease.InCubic);

        GetComponentInChildren<MeshRenderer>().material.DOColor(Color.green * 2.0f, "_Color", 1f)
            .SetEase(Ease.OutBounce);

        if (DataManager.instance.DisplayScore.Value)
        {
            DataManager.instance.Score.SetValue(DataManager.instance.Score.Value + 1);
            ScoreUpText.text = "+100";
            ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0));
            Sequence scoreUpSequence = DOTween.Sequence();
            scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.green * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
            scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 0.5f, 0.5f));
            scoreUpSequence.Join(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.2f, 0.5f).SetEase(Ease.OutBounce));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce));
            scoreUpSequence.AppendCallback(() =>
                ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0)));
            scoreUpSequence.AppendCallback(() => Destroy(parent.gameObject));
        }
    }
    public void NoHitBad()
    {
        //Disable colliders
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        //Update target's status
        _isShown = false;
        _endTimeValue = _time;

        //Register some stats for further purpose (save data of player)
        DataManager.instance.TimeToShootList.Add(_time);

        //Play sound
        AudioMiss.Play();

        //Play animation
        Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
        var transform1 = parent.transform;
        Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
        parent.transform.SetParent(null);
        parent.transform.position = worldCanvasPosition;

        if (DataManager.instance.Score.Value > 0 && DataManager.instance.DisplayScore.Value)
        {
            DataManager.instance.Score.SetValue(DataManager.instance.Score.Value - 1);
            ScoreUpText.text = "-100";
            ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0));
            Sequence scoreUpSequence = DOTween.Sequence();
            scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.red * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
            scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 0.5f, 0.2f));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.5f, 0.5f).SetEase(Ease.OutBounce));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBounce));
            scoreUpSequence.AppendCallback(() => Destroy(parent.gameObject));
        }
        
        if(_spriteRenderer)
            _spriteRenderer.transform.DOLocalMove(-_spriteRenderer.transform.localPosition, 0.3f)
            .SetEase(Ease.InCubic);

        GetComponentInChildren<MeshRenderer>().material.DOColor(Color.red * 2.0f, "_Color", 1f)
            .SetEase(Ease.OutBounce);       
    }
    public void HitGood()
    {
        //Disable colliders
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        //Update target's status
        _isHit = true;
        _isShown = false;
        _endTimeValue = _time;

        //Register some stats for further purpose (save data of player)
        DataManager.instance.TimeToShootList.Add(_time);
        DataManager.instance.TargetHit?.SetValue(DataManager.instance.TargetHit.Value + 1);

        //Play sound
        AudioValidate.Play();

        //Play animation
        Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
        var transform1 = parent.transform;
        Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
        parent.transform.SetParent(null);
        parent.transform.position = worldCanvasPosition;
        
        GetComponentInChildren<MeshRenderer>().material.DOColor(Color.green * 2.0f, "_Color", 1f)
            .SetEase(Ease.OutBounce);

        switch (Direction)
        {
            case Direction.North:
                transform.DORotate(new Vector3(0.0f, IsNegateImage ? 85 : -85, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
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

        if (_spriteRenderer)
        {
            changeImage.Append(_spriteRenderer.DOColor(new Color(0, 0.25f, 0.25f, 1), 0.5f).SetEase(Ease.OutBounce));
            changeImage.AppendCallback(() => _spriteRenderer.sprite = SplashImage);
            changeImage.Append(_spriteRenderer.DOColor(Color.white, 0.5f).SetEase(Ease.OutBounce));
        }
        
        if (DataManager.instance.DisplayScore.Value)
        {
            DataManager.instance.Score.SetValue(DataManager.instance.Score.Value + 1);
            ScoreUpText.text = "+100";
            ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0));
            Sequence scoreUpSequence = DOTween.Sequence();
            scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.green * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
            scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 0.5f, 0.5f));
            scoreUpSequence.Join(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.2f, 0.5f).SetEase(Ease.OutBounce));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBounce));
            scoreUpSequence.AppendCallback(() =>
                ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0)));
            scoreUpSequence.AppendCallback(() => Destroy(parent.gameObject));
        }
        
    }
    public void HitBad()
    {
        //Disable colliders
        foreach (Collider collider1 in _colliders)
        {
            collider1.enabled = false;
        }

        //Update target's status
        _isHit = true;
        _isShown = false;
        _endTimeValue = _time;

        //Register some stats for further purpose (save data of player)
        DataManager.instance.TimeToShootList.Add(_time);

        //Play sound
        AudioMiss.Play();

        //Play animation
        Canvas parent = ScoreUpText.GetComponentInParent<Canvas>();
        var transform1 = parent.transform;
        Vector3 worldCanvasPosition = new Vector3(transform1.position.x, transform1.position.y, transform1.position.z);
        parent.transform.SetParent(null);
        parent.transform.position = worldCanvasPosition;

        if (DataManager.instance.Score.Value > 0 && DataManager.instance.DisplayScore.Value)
        {
            DataManager.instance.Score.SetValue(DataManager.instance.Score.Value - 1);
            ScoreUpText.text = "-100";
            ScoreUpText.material.SetColor(ColorMat, new Color(0, 0, 0, 0));
            Sequence scoreUpSequence = DOTween.Sequence();
            scoreUpSequence.Append(ScoreUpText.material.DOColor(Color.red * 3.0f, "_Color", 0.5f).SetEase(Ease.InBounce));
            scoreUpSequence.Join(ScoreUpText.transform.DOMove(transform1.position + Vector3.up * 0.5f, 0.2f));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(ScoreUpText.transform.localScale + ScoreUpText.transform.localScale * 1.5f, 0.5f).SetEase(Ease.OutBounce));
            scoreUpSequence.Append(ScoreUpText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBounce));
            scoreUpSequence.AppendCallback(() => Destroy(parent.gameObject));
        }

        
        GetComponentInChildren<MeshRenderer>().material.DOColor(Color.red * 2.0f, "_Color", 1f)
            .SetEase(Ease.OutBounce);

        switch (Direction)
        {
            case Direction.North:
                transform.DORotate(new Vector3(0.0f, IsNegateImage ? 85 : -85, 0.0f), _bounceDuration).SetEase(Ease.OutBounce);
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

        if (_spriteRenderer)
        {
            changeImage.Append(_spriteRenderer.DOColor(new Color(0.25f, 0, 0, 1), 0.5f).SetEase(Ease.OutBounce));
            changeImage.AppendCallback(() => _spriteRenderer.sprite = SplashImage);
            changeImage.Append(_spriteRenderer.DOColor(Color.white, 0.5f).SetEase(Ease.OutBounce));
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
    public bool GetIsHit()
    {
        return _isHit;
    }
    public bool IsShown()
    {
        return _isShown;
    }
    #endregion

}
