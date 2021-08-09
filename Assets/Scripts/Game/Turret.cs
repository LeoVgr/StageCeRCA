using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Target
{
    #region "Attributs"
    public GameObject TurretBody;
    public Transform OriginBullet;
    public float TimeBetweenShoot;

    private float _shootTimer = 0;
    #endregion

    #region "Events"
    protected override void Update()
    {
        base.Update();

        

        if (this.IsShown())
        {
            OrientTurret();

            //Add the time to cd 
            _shootTimer += Time.deltaTime;

            if (_shootTimer >= TimeBetweenShoot)
            {
                ShootAtPlayer();

                //Reset the time
                _shootTimer = 0;
            }
        }

    }
    #endregion

    #region "Methods"
    public void ShootAtPlayer()
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
    public void OrientTurret()
    {
        TurretBody.transform.rotation = Quaternion.Euler(0,Vector3.Angle(-this.transform.forward, DataManager.instance.Player.Value.transform.position - transform.position),0);
    }
}       
    #endregion

