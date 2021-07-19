using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Player
{
    /**
 * @author : Samuel BUSSON
 * @brief : PlayerFire get input from the inputActions map and fires a laser if the player presses the fire button.
 * @date : 07/2020
 */
    public class PlayerFire : MonoBehaviour
    {
        #region Attributs
        [Header("Atom variables")]
        public BoolVariable IsPlayerLock;
        public BoolVariable PlayerCanFire;
        public BoolVariable CameraInfps;

        private PlayerSaveData _playerData;
        private int _modelIndex => _playerData._modelIndex;

        [Header("FX")]
        public GameObject GunObjectFirstPerson;
        public GameObject[] GunObjectThirdPerson = new GameObject[3];
    
        [Header("Audio")] public AudioSource FireAudio;

        [Header("Pool")]
        public GameObject ObjectToPool;
        private int _amountToPool;

        public GameObject Crosshair;
        private List<Bullet> _pooledObjects;
        private Camera _camera;
        private GameObject _gunObject;
        private GameObject _fireStartPositon;
        private float _timer;
        private float _cooldownFire = 0.5f;
        #endregion


        #region Events
        void Start()
        {
            //Find references 
            _camera = Camera.main;
            _playerData = transform.parent.GetComponent<PlayerSaveData>();

            //Create bullet pool
            _amountToPool = 3;
            CreatePooledObjectList();
        
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
            if (InputManager.Instance.isFiring)
            {
                if (_timer > _cooldownFire)
                {
                    _timer = 0f;
                    RapidFire();
                }
            }
        
        }
        #endregion

        #region Methods
        private void UpdateGun(bool isFPS)
        {
            _gunObject = isFPS ? GunObjectFirstPerson : GunObjectThirdPerson[_modelIndex];
            _fireStartPositon = _gunObject;
        }
        
        private void CreatePooledObjectList()
        {
            _pooledObjects = new List<Bullet>();

            for (int i = 0; i < _amountToPool; ++i)
            {
                GameObject obj = Instantiate(ObjectToPool);
                obj.SetActive(false);
                _pooledObjects.Add(obj.GetComponent<Bullet>());
            }
        }
        private Bullet GetFirstAvailablePooledObject()
        {
            //Looking for the first bullet not active in hierarchy (aka not used)
            for (int i = 0; i < _pooledObjects.Count; ++i)
            {
                if (!_pooledObjects[i].gameObject.activeInHierarchy)
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
                FireAudio.Play();

                //Shot animation
                if (_gunObject)
                    _gunObject.transform.DOPunchScale(_gunObject.transform.localScale * 0.1f, _cooldownFire * 0.75f);

                //Prepare the bullet
                Bullet bullet = GetFirstAvailablePooledObject();
                if (bullet)
                    bullet.gameObject.SetActive(true);

                //Looking for target
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(Crosshair.transform.position);

                Vector3 normal = Vector3.zero;;
                Vector3 endPoint =  Vector3.zero;
                bool hitSomething = false;

                if (Physics.Raycast(ray, out hit, 100.0f, ~LayerMask.GetMask("Player")))
                {              
                    Target target = hit.collider.GetComponentInChildren<Target>();
                    if(target != null)
                        target.Hit();
                    endPoint = hit.point;
                    hitSomething = true;
                    normal = hit.normal;
                }
                else
                {
                    //If there is no target, just makes the bullet go far away
                    endPoint = ray.GetPoint(100.0f);
                }

                //Fire the bullet
                if (bullet)
                    bullet.Fire(_fireStartPositon.transform.position,  endPoint, hitSomething, normal);

            }
        }
        #endregion
    }
}