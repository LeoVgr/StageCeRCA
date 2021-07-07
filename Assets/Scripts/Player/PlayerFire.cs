using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

/**
 * @author : Samuel BUSSON
 * @brief : PlayerFire get input from the inputActions map and fires a laser if the player presses the fire button.
 * @date : 07/2020
 */
public class PlayerFire : MonoBehaviour
{
    #region "Attributs"
    [Header("Atom variables")]
    public GameObject FireStartPositonFPS;
    public GameObject FireStartPositonTPS;
    public BoolVariable IsPlayerLock;
    public BoolVariable PlayerCanFire;
    public BoolVariable CameraInfps;

    [Header("FX")]
    public GameObject GunObjectFirstPerson;
    public GameObject GunObjectThirdPerson;
    
    [Header("Sound Effect")]
    [FMODUnity.EventRef][SerializeField]
    private string _fireEvent;
    
    [Header("Pool")]
    public GameObject ObjectToPool;
    private int _amountToPool;

    public GameObject Crosshair;
    private bool _isFiring;
    private List<GameObject> _pooledObjects;
    private InputActionMap _inputActions;
    private PlayerInput _input;
    private Camera _camera;
    private GameObject _gunObject;
    private GameObject _fireStartPositon;
    private float _timer;
    private float _cooldownFire = 0.5f;
    #endregion


    #region "Events"
    void Start()
    {
        //Find references 
        _camera = Camera.main;

        //Create bullet pool
        _amountToPool = 3;
        CreatePooledObjectList();

        //Bind player inputs on methods
        _input = GetComponent<PlayerInput>();
        _inputActions = _input.actions.actionMaps[0];
        _inputActions["Fire"].performed += Fire;
        _inputActions["Fire"].canceled += CancelFire;
        
        //Update the origin of the shot depending if we are in fps or tps
        CameraInfps.Changed.Register(UpdateGun);
        UpdateGun(CameraInfps.Value);      
    }
    private void Update()
    {
        //We need to update the reference (don't delete this line, it wreates a bug)
        _camera = Camera.main;

        //Update the timer
        _timer += Time.deltaTime;

        //Check if the fire action has been done by the player
        if (_isFiring)
        {
            if (_timer > _cooldownFire)
            {
                _timer = 0f;
                RapidFire();
            }
        }
        
    }
    private void OnDestroy()
    {
        //Unregister methods when object is destroyed
        _inputActions["Fire"].performed -= Fire;
        _inputActions["Fire"].canceled -= CancelFire;
    }
    #endregion

    #region "Methods"
    private void UpdateGun(bool b)
    {
        _gunObject = b ? GunObjectFirstPerson : GunObjectThirdPerson;
        _fireStartPositon = b ? FireStartPositonFPS : FireStartPositonTPS;
    }      
    private void CreatePooledObjectList()
    {
        _pooledObjects = new List<GameObject>();

        for (int i = 0; i < _amountToPool; ++i)
        {
            GameObject obj = Instantiate(ObjectToPool);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }
    private GameObject GetFirstAvailablePooledObject()
    {
        //Looking for the first bullet not active in hierarchy (aka not used)
        for (int i = 0; i < _pooledObjects.Count; ++i)
        {
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }

        return null;
    }
    private void RapidFire()
    {
        if(!IsPlayerLock.Value && PlayerCanFire.Value)
        {
            //Play Sound
            FMODUnity.RuntimeManager.PlayOneShot(_fireEvent, transform.position);

            //Shot animation
            if (_gunObject)
                _gunObject.transform.DOPunchScale(_gunObject.transform.localScale * 0.1f, _cooldownFire * 0.75f);

            //Prepare the bullet
            GameObject go = GetFirstAvailablePooledObject();
            if (go)
                go.SetActive(true);

            //Looking for target
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Crosshair.transform.position);

            Vector3 normal = Vector3.zero;;
            Vector3 endPoint =  Vector3.zero;
            bool hitSomething = false;
            
            if (Physics.Raycast(ray, out hit, 100.0f, ~LayerMask.GetMask("Player")))
            {              
                Target t = hit.collider.GetComponentInChildren<Target>();
                t?.Hit();
                endPoint =  hit.point;
                hitSomething = true;
                normal = hit.normal;
            }
            else
            {
                //If there is no target, just makes the bullet go far away
                endPoint = ray.GetPoint(100.0f);
            }

            //Fire the bullet
            if (go)
                go.GetComponentInChildren<Bullet>().Fire(_fireStartPositon.transform.position,  endPoint, hitSomething, normal);

        }
    }
    private void Fire(InputAction.CallbackContext obj)
    {
        _isFiring = true;
    }
    private void CancelFire(InputAction.CallbackContext obj)
    {
        _isFiring = false;
    }
    #endregion
}
