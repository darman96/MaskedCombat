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
    [Range (1, 4)]
    public int PlayerNumber = 1;
    public float AttackDelayOff = 2f;
    public float AttackDelayDef = 2f;
    public float AttackDelayHit = 0.5f;
    public float JumpDelay = 1.0f;
    public float RunSpeed = 6f;
    public float TurnSpeed = 5f;
    public float JumpHeight = 5f;
    public float StunnedTime = 1.0f;
    public float InvulnerableTime = 2.0f;
    public float deadzone = 0.5f;

    [HideInInspector]
    public Dictionary<MaskType, Mask> OwnedMasks = new Dictionary<MaskType, Mask>();
    [HideInInspector]
    public MaskType ActiveMask_Offensive;
    [HideInInspector]
    public MaskType ActiveMask_Defensive;

    private float LastOffensive;
    private float LastDefensive;
    private float LastHit;
    private float LastJump;
    private float DeadTime;
    private int Joystick;
    private Vector2 leftstick;
    private Vector2 rightstick;
    private Animator pAnimator;
    private Rigidbody pRigidbody;
    private Vector3 ResultingSpeed;
    private float ResultingTorque;
    private bool IsInvulnerable;
    private bool IsStunned;

    private int _Score;
    public int Score
    {
        get { return _Score; }
        private set { _Score = value; }
    }
    #endregion

    public virtual void ResetStartGame()
    {
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
        pAnimator = GetComponent<Animator>();
        pRigidbody = GetComponent<Rigidbody>();

        ResetAll();

        switch (PlayerNumber)
        {
            case 1:
                Joystick = GameManager.instance.InputPlayer1;
                break;
            case 2:
                Joystick = GameManager.instance.InputPlayer2;
                break;
            case 3:
                Joystick = GameManager.instance.InputPlayer3;
                break;
            case 4:
                Joystick = GameManager.instance.InputPlayer4;
                break;
        }
    }

    private void Update()
    {
        if (Time.time - DeadTime < StunnedTime)
        {
            IsStunned = true;
            IsInvulnerable = true;

            return;
        }

        IsStunned = false;

        if (Time.time - DeadTime < InvulnerableTime)
        {
            IsInvulnerable = true;

            return;
        }

        IsInvulnerable = false;

        if (Joystick == -1)
            return;

        float deltaJump = Time.time - LastJump;
        float deltaAttack = Mathf.Min (Time.time - LastOffensive, Time.time - LastHit);

        if (Joystick == 0)
        {
            if (Input.GetButtonDown("KeyboardJump") && deltaJump > JumpDelay)
            {
                Jump();
            }

            if (Input.GetButtonDown("KeyboardHit") && deltaAttack > AttackDelayHit && deltaJump > JumpDelay)
            {
                MeleeAttack();
            }

            if (Input.GetButtonDown("KeyboardOff") && deltaAttack > AttackDelayOff && deltaJump > JumpDelay)
            {
                ShootOffensive();
            }

            if (Input.GetButtonDown("KeyboardDef") && Time.time - LastDefensive > AttackDelayDef)
            {
                ActivateDefensive();
            }

            // Specifies the amount of movement
            leftstick = new Vector2(Input.GetAxis("KeyboardAxisX"), Input.GetAxis("KeyboardAxisY"));
            rightstick = new Vector2(Input.GetAxis("MouseCamAxisX"), Input.GetAxis("MouseCamAxisY"));
        }
        else
        {
            if (Input.GetButtonDown("ButtonA_P" + Joystick) && deltaJump > JumpDelay)
            {
                Jump();
            }

            if (Input.GetButtonDown("KeyboardHit") && deltaAttack > AttackDelayHit && deltaJump > JumpDelay)
            {
                MeleeAttack();
            }

            if (Input.GetButtonDown("ButtonX_P" + Joystick) && deltaAttack > AttackDelayOff && deltaJump > JumpDelay)
            {
                ShootOffensive();
            }

            if (Input.GetButtonDown("ButtonY_P" + Joystick) && Time.time - LastDefensive > AttackDelayDef)
            {
                ActivateDefensive();
            }

            // Specifies the amount of movement
            leftstick = new Vector2(Mathf.Clamp(Input.GetAxis("AxisX_P" + Joystick) + Input.GetAxis("KeyboardAxisX"), -1.0f, 1.0f), Mathf.Clamp(Input.GetAxis("AxisY_P" + Joystick) + Input.GetAxis("KeyboardAxisY"), -1.0f, 1.0f));
            if (leftstick.magnitude < deadzone)
                leftstick = Vector2.zero;
            //else
            //    leftstick = leftstick.normalized * ((leftstick.magnitude - deadzone) / (1 - deadzone));

            rightstick = new Vector2(Mathf.Clamp(Input.GetAxis("CamAxisX_P" + Joystick) + Input.GetAxis("MouseCamAxisX"), -1.0f, 1.0f), Mathf.Clamp(Input.GetAxis("CamAxisY_P" + Joystick) + Input.GetAxis("MouseCamAxisY"), -1.0f, 1.0f));
            if (rightstick.magnitude < deadzone)
                rightstick = Vector2.zero;
            //else
            //    rightstick = rightstick.normalized * ((rightstick.magnitude - deadzone) / (1 - deadzone));
        }

    }

    private void FixedUpdate()
    {
        if (IsStunned)
        {
            return;
        }

        Move();
        UpdateAnimator();
        //Debug.Log(AxisX.ToString("0.0") + " : " + AxisY.ToString("0.0") + (Jumping ? "JUMP " : " ") + (Crouching ? "CROUCH " : " ") + (IsGrounded ? "GROUNDED" : ""));
    }

    void UpdateAnimator()
    {
        return;

        pAnimator.SetFloat("Forward", ResultingSpeed.magnitude, 0.01f, Time.deltaTime);
        pAnimator.SetFloat("Turn", ResultingTorque / TurnSpeed, 0.01f, Time.deltaTime);
        pAnimator.SetBool("IsStunned", IsStunned);
        pAnimator.SetBool("IsInvulnerable", IsInvulnerable);
        pAnimator.SetTrigger("MeleeAttack");
        pAnimator.SetTrigger("OffAttack");
        pAnimator.SetTrigger("DefAttack");
    }

    void OnTriggerStay(Collider other)
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if (IsInvulnerable)
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

    void Move()
    {
        ResultingSpeed = new Vector3 (leftstick.x, 0, leftstick.y).normalized * RunSpeed;

        //Debug.Log("Stick: " + leftstick);
        //Debug.Log("Speed: " + ResultingSpeed);

        //if (Movement == MovementType.Walking && ResultingSpeed > 0.5f && Grounded)
        //    SoundManager.instance.Loop("P1Walk", transform.position, transform.rotation, SoundType.footstep);
        //else
        //    SoundManager.instance.Stop("P1Walk");

        pRigidbody.velocity = ResultingSpeed;

        if (ResultingSpeed.sqrMagnitude > 1f)
            transform.rotation = Quaternion.LookRotation(ResultingSpeed, Vector3.up);

        pRigidbody.angularVelocity = Vector2.zero;

        //Debug.Log("Curr: " + RotY.ToString("0.0") + " Tgt: " + TgtRot.ToString("0.0") + " Delta: " + delta.ToString("0.0") + "Req: " + ReqTorque.ToString("0.0"));
    }

    void Jump()
    {
        LastJump = Time.time;
    }

    private void ShootOffensive()
    {
        LastOffensive = Time.time;

        switch (ActiveMask_Offensive)
        {
            case MaskType.Fire:

                WeaponManager.instance.CreateWeapon(transform.position + transform.right * -0.4f, transform.rotation, transform.forward * 15, WeaponType.Fireball);
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Earth:

                WeaponManager.instance.CreateWeapon(transform.position + transform.right * -0.4f, transform.rotation, transform.forward * 15, WeaponType.Earth);
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Lightning:

                WeaponManager.instance.CreateWeapon(transform.position + transform.right * -0.4f, transform.rotation, transform.forward * 15, WeaponType.Lightning);
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Water:

                WeaponManager.instance.CreateWeapon(transform.position + transform.right * -0.4f, transform.rotation, transform.forward * 15, WeaponType.Water);
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;
        }
    }

    private void MeleeAttack()
    {
        LastHit = Time.time;

        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.meleeswing);
    }

    private void ActivateDefensive()
    {
        LastDefensive = Time.time;

        switch (ActiveMask_Defensive)
        {
            case MaskType.Ice:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Metal:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Nature:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;

            case MaskType.Wind:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lasershot);
                break;
        }
    }
}
