/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// This handles the player interaction
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using UnityEngine.VR;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : GameAttributes
{
    [SerializeField]
    [Range (1, 4)]
    int PlayerNumber = 1;

    private float LastRespawn;

    void Start()
    {
        Reset();
    }

    private void Update()
    {
        if (IsDead || Time.time - LastRespawn < 0.5f)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && Health > 5)
            ShootBullet();
    }

    private void FixedUpdate()
    {
        if (IsDead || Time.time - LastRespawn < 0.5f)
        {
            return;
        }

        //Debug.Log(AxisX.ToString("0.0") + " : " + AxisY.ToString("0.0") + (Jumping ? "JUMP " : " ") + (Crouching ? "CROUCH " : " ") + (IsGrounded ? "GROUNDED" : ""));
    }

    void OnTriggerStay(Collider other)
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if (IsDead || Time.time - LastRespawn < 0.5f)
        {
            return;
        }

        if (col.gameObject.tag == "Deadly")
            HitForDamage(100, true);

        if (col.gameObject.tag != "EnemyWeapon")
            return;

        HitForDamage(10, false);
    }

    public new void Reset()
    {
        transform.position = Vector3.zero;
        base.Reset();
    }
    public void Respawn()
    {
        LastRespawn = Time.time;
        ResetHealth();
        this.gameObject.SetActive(true);       
    }
    public void HitForDamage(int amount, bool cankill)
    {
        SubtractHealth(amount/10, cankill);

        SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.death);

        if (IsDead)
        {
            //this.gameObject.SetActive(false);

            SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.gameover, true);
            GameManager.instance.SetGameState(GameState.GameOver);
        }
    }

    private void ShootBullet()
    {
        //WeaponManager.instance.CreateWeapon(EyeCam.transform.position + EyeCam.transform.right * -0.4f, EyeCam.transform.rotation, EyeCam.transform.forward * 15, WeaponType.LaserWeapon);
        //SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
    }
}
