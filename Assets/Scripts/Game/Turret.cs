using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Target
{
    #region "Attributs"
    public float TimeBetweenShoot;

    private float _shootTimer = 0;
    #endregion


    #region "Events"
    protected override void Update()
    {
        base.Update();

        if (this.IsShown())
        {
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
        transform.DOPunchScale(transform.localScale * 0.1f, 1f);

        //Prepare the bullet and fire it
        Bullet bullet = Player.Value.GetComponentInChildren<PlayerFire>().GetFirstAvailablePooledObject();

        if (bullet)
        {
            bullet.gameObject.SetActive(true);
            bullet.GetComponent<Bullet>().Fire(transform.position, Player.Value.transform.position, false, Vector3.zero);
        }
    }
}       
    #endregion

