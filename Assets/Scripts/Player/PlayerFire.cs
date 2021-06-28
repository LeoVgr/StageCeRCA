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

    [Header("Atom variables")]
    public GameObjectVariable cameraAtomVariable;
    public GameObject fireStartPositonFPS;
    public GameObject fireStartPositonTPS;
    public BoolVariable a_isPlayerLock;
    public BoolVariable playerCanFire;
    public BoolVariable cameraInfps;


    [Header("FX")]
    //public VisualEffect gunShootFX;
    public GameObject gunObjectFirstPerson;
    public GameObject gunObjectThirdPerson;

    
    [Header("Sound Effect")]
    [FMODUnity.EventRef][SerializeField]
    private string fireEvent;
    
    [Header("Pool")]
    public GameObject objectToPool;
    private int _amountToPool;

    private bool _isFiring;
    private List<GameObject> _pooledObjects;
    private InputActionMap _inputActions;
    private PlayerInput _input;
    private Camera _camera;

    private GameObject _gunObject;
    private GameObject _fireStartPositon;

    private float _timer;
    private float _timeToFire = 0.5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _amountToPool = 3;
        
        _input = GetComponent<PlayerInput>();
        _inputActions = _input.actions.actionMaps[0];
        _inputActions["Fire"].performed += Fire;
        _inputActions["Fire"].canceled += CancelFire;

        _camera = cameraAtomVariable.Value.GetComponent<Camera>();
        cameraInfps.Changed.Register(UpdateGun);
        UpdateGun(cameraInfps.Value);

        CreatePooledObjectList();
    }


    private void Update()
    {
        _timer += Time.deltaTime;
        if (_isFiring)
        {
            if (_timer > _timeToFire)
            {
                _timer = 0f;
                RapidFire();
            }
        }
    }

    private void UpdateGun(bool b)
    {
        _gunObject = b ? gunObjectFirstPerson : gunObjectThirdPerson;
        _fireStartPositon = b ? fireStartPositonFPS : fireStartPositonTPS;
    }

    private void OnDestroy()
    {
        _inputActions["Fire"].performed -= Fire;
        _inputActions["Fire"].canceled -= CancelFire;
    }
    
    private void CreatePooledObjectList()
    {
        _pooledObjects = new List<GameObject>();
        for (int i = 0; i < _amountToPool; ++i)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }

    private GameObject GetPooledObject()
    {
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
        if(!a_isPlayerLock.Value && playerCanFire.Value)
        {
            FMODUnity.RuntimeManager.PlayOneShot(fireEvent, transform.position);

            if (_gunObject)
                _gunObject.transform.DOPunchScale(_gunObject.transform.localScale * 0.1f, _timeToFire * 0.75f);
            
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(new Vector3(_camera.pixelWidth / 2.0f, _camera.pixelHeight / 2.0f, 0));

            Vector3 normal = Vector3.zero;;

            GameObject go = GetPooledObject();
            if (go)
                go.SetActive(true);

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
                endPoint = ray.GetPoint(100.0f);
            }

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

}
