using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Target
{
    #region "Attributs"
    public GameObject TurretBody;
    public GameObject TurretArm;
    public LineRenderer LineRenderer;
    public Transform EyeTransform;
    public Transform OriginBullet;
    public float TimeBetweenShoot;

    private float _shootTimer = 0;
    #endregion

    #region "Events"
    protected override void Start()
    {
        base.Start();

        //Put the turret arm in the right place
        if (!IsNegateImage)
        {
            TurretArm.transform.rotation *= Quaternion.Euler(0,180,0);
            TurretArm.transform.localPosition = new Vector3(-7, 0, 0);
        }
      
    }
    protected override void Update()
    {
        base.Update();       

        if (this.IsShown())
        {
            LineRenderer.enabled = true;

            OrientTurret();

            DisplayLineRenderer();

            //Add the time to cd 
            _shootTimer += Time.deltaTime;

            if (_shootTimer >= TimeBetweenShoot)
            {
                ShootAtPlayer();

                //Reset the time
                _shootTimer = 0;
            }
        }
        else
        {
            LineRenderer.enabled = false;
        }
        

    }
    #endregion

    #region "Methods"
    private void ShootAtPlayer()
    {
        //TODO : put shot sound
        //FireAudio.Play();

        //Shot animation
        TurretBody.transform.DOPunchScale(TurretBody.transform.localScale * 0.1f, 1f);

        //Prepare the bullet and fire it
        Bullet bullet = DataManager.instance.Player.Value.GetComponentInChildren<PlayerFire>().GetFirstAvailablePooledObject();

        if (bullet)
        {
            bullet.gameObject.SetActive(true);
            bullet.GetComponent<Bullet>().Fire(OriginBullet.position, DataManager.instance.Player.Value.transform.position, false, Vector3.zero);
        }

        //Do damage effect on the player
        DataManager.instance.Player.Value.transform.DOPunchScale(transform.localScale * 0.1f, 0.5f);

        DataManager.instance.Player.Value.GetComponentInChildren<PlayerLife>().GetHurt();
    }
    private void OrientTurret()
    {
        Vector3 lookPos = DataManager.instance.Player.Value.transform.position - TurretBody.transform.position;
        lookPos.y = 0;

        Quaternion rotation = Quaternion.Euler(0,180,0) * Quaternion.LookRotation(lookPos);
        TurretBody.transform.rotation = rotation;

    }
    private void DisplayLineRenderer()
    {
        //Starting position
        LineRenderer.SetPosition(0, EyeTransform.position);

        //End position
        LineRenderer.SetPosition(1, DataManager.instance.Player.Value.transform.position + new Vector3(0,1f,0));

        LineRenderer.widthMultiplier = 0.05f;
    }
}       
    #endregion

