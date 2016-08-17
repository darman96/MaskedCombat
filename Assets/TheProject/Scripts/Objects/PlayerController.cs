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

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField]
    [Range (1, 4)]
    public int PlayerNumber = 1;

    [SerializeField]
    public float AttackDelayOff { get; set; }
    [SerializeField]
    public float AttackDelayDef { get; set; }
    [SerializeField]
    public float AttackDelayHit { get; set; }
    [SerializeField]
    public float RunSpeed { get; set; }
    [SerializeField]
    public float JumpHeight { get; set; }
    [SerializeField]
    public float StunnedTime { get; set; }
    [SerializeField]
    public float InvulnerableTime { get; set; }

    [HideInInspector]
    public Dictionary<MaskType, Mask> OwnedMasks;
    [HideInInspector]
    public MaskType ActiveMask_Offensive;
    [HideInInspector]
    public MaskType ActiveMask_Defensive;

    private float LastOffensive;
    private float LastDefensive;
    private float LastHit;
    private float LastJump;
    private float DeadTime;

    private bool _IsDead;
    public bool IsDead
    {
        get { return _IsDead; }
        private set { _IsDead = value; }
    }

    private int _Score;
    public int Score
    {
        get { return _Score; }
        private set { _Score = value; }
    }
    #endregion

    public virtual void ResetStartGame()
    {
        IsDead = false;
        OwnedMasks.Clear();
        transform.rotation = Quaternion.identity;

        switch (PlayerNumber)
        {
            case 1:
                transform.position = GameManager.instance.StartPoint1.transform.position;
                break;
            case 2:
                transform.position = GameManager.instance.StartPoint2.transform.position;
                break;
            case 3:
                transform.position = GameManager.instance.StartPoint3.transform.position;
                break;
            case 4:
                transform.position = GameManager.instance.StartPoint4.transform.position;
                break;
        }
    }

    public virtual void ResetAll()
    {
        Score = 0;

        ResetStartGame();
    }

    public virtual void AddWin ()
    {
        _Score++;
    }

    void Start()
    {
        ResetAll();
    }

    private void Update()
    {
        if (Time.time - DeadTime < StunnedTime)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time - LastHit > AttackDelayHit)
            MeleeAttack();
    }

    private void FixedUpdate()
    {
        if (Time.time - DeadTime < StunnedTime)
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
        if (Time.time - DeadTime < InvulnerableTime)
        {
            return;
        }

        if (col.gameObject.tag != "EnemyWeapon")
            return;

        OnWasHit();
    }

    public void OnWasHit()
    {
        SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.death);
    }

    private void ShootOffensive()
    {
        WeaponManager.instance.CreateWeapon(transform.position + transform.right * -0.4f, transform.rotation, transform.forward * 15, WeaponType.LaserWeapon);
        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
    }

    private void MeleeAttack()
    {
        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.meleeswing);
    }

    private void ActivateDefensive()
    {
        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.alienroar);
    }
}
