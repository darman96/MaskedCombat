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
    public float AttackDelayOff = 0.5f;
    public float AttackDelayDef = 0.5f;
    public float AttackDelayHit = 0.5f;
    public float WindActivationPeriod = 2f;
    public float IceActivationPeriod = 2f;
    public float NatureActivationPeriod = 2f;
    public float MetalActivationPeriod = 2f;
    public float JumpDelay = 1.0f;
    public float RunSpeed = 6f;
    public float TurnSpeed = 5f;
    public float JumpHeight = 5f;
    public float StunnedTime = 2.0f;
    public float DizzyTime = 0.5f;
    public float InvulnerableTime = 5.0f;
    public float deadzone = 0.5f;
    public GameObject StunnedEffect;
    public GameObject InvulnerableEffect;
    public GameObject PlayerMesh;

    [HideInInspector]
    public Dictionary<MaskType, Mask> OwnedMasks = new Dictionary<MaskType, Mask>();
    [HideInInspector]
    public MaskType ActiveMask_Offensive = MaskType.NONE;
    [HideInInspector]
    public MaskType ActiveMask_Defensive = MaskType.NONE;

    private float LastOffensive;
    private float LastDefensive;
    private float LastHit;
    private float LastJump;
    private float DeadTime = -10;
    private float EnteredDizzyTime = -10;
    private int Joystick;
    private Vector2 leftstick;
    private Vector2 rightstick;
    private Animator pAnimator;
    private Rigidbody pRigidbody;
    private Vector3 ResultingSpeed;
    private bool IsInvulnerable;
    private bool IsShieldedFromRanged;
    private bool IsProtectedFromMelee;
    private bool IsStunned;
    private bool IsJumping;
    private float DefensiveUntilTime;
    private MaskType DefensiveActivated;

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

    public virtual bool ResetAll()
    {
        Score = 0;

        ResetStartGame();

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

        if (Joystick < 0)
        {
            gameObject.SetActive(false);
            return false;
        }

        gameObject.SetActive(true);
        return true;
    }

    public virtual void AddWin ()
    {
        _Score++;
    }

    void Start()
    {
        pAnimator = GetComponent<Animator>();
        pRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.instance == null || GameManager.instance.GameState != GameState.Playing)
            return;

        if (Time.time > DefensiveUntilTime)
        {
            DefensiveActivated = MaskType.NONE;
            PlayerMesh.SetActive(true);
            IsShieldedFromRanged = false;
            IsProtectedFromMelee = false;
        }
        else
        {
            if (DefensiveActivated == MaskType.Nature)
            {
                PlayerMesh.SetActive(false);
            }
            else if (DefensiveActivated == MaskType.Ice)
            {
                IsShieldedFromRanged = true;
            }
            else if (DefensiveActivated == MaskType.Metal)
            {
                IsProtectedFromMelee = true;
            }
        }

        // scale character while jumping
            if (transform.position.y > 0.2f)
        {
            float scale = Mathf.Clamp(1 + transform.position.y, 1, 1.5f);
            transform.localScale = new Vector3(scale, scale, scale);
            IsJumping = true;
        }
        else
        { 
            // reset scale when not in air
            transform.localScale = Vector3.one;
            IsJumping = false;
        }

        if (Time.time - DeadTime < StunnedTime)
        {
            IsStunned = true;
            IsInvulnerable = true;
            pRigidbody.velocity = new Vector3(0, pRigidbody.velocity.y, 0);
            StunnedEffect.SetActive(true);

            return;
        }

        if (Time.time - EnteredDizzyTime < DizzyTime)
        {
            IsStunned = true;
            pRigidbody.velocity = new Vector3(0, pRigidbody.velocity.y, 0);
            StunnedEffect.SetActive(true);
            return;
        }

        IsStunned = false;
        StunnedEffect.SetActive(false);

        if (Time.time - DeadTime < InvulnerableTime)
        {
            IsInvulnerable = true;
            InvulnerableEffect.SetActive(true);
        }
        else
        {
            IsInvulnerable = false;
            InvulnerableEffect.SetActive(false);
        }

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

            if (Input.GetButtonDown("ButtonB_P" + Joystick) && deltaAttack > AttackDelayHit && deltaJump > JumpDelay)
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

            if (Input.GetButtonDown("ButtonL_P" + Joystick))
            {
                NextOffensiveMask();
            }

            if (Input.GetButtonDown("ButtonR_P" + Joystick))
            {
                NextDefensiveMask();
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
        if (GameManager.instance == null || GameManager.instance.GameState != GameState.Playing)
            return;

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
        pAnimator.SetFloat("Forward", Mathf.Abs(leftstick.y), 0.01f, Time.deltaTime);
        pAnimator.SetBool("IsStunned", IsStunned);
        pAnimator.SetBool("IsInvulnerable", IsInvulnerable);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mask")
        {
            Mask m = other.gameObject.GetComponent<Mask>();

            if (m && m.CanBePickedUp)
            {
                GameManager.instance.PickupMask(m, this);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (IsInvulnerable)
        {
            return;
        }

        if (col.gameObject.tag != "Weapon")
            return;

        WeaponController wpn = col.gameObject.GetComponent<WeaponController>();

        if (wpn && wpn.Owner != PlayerNumber)
            OnWasHitOff();
    }

    public void OnWasHitMelee()
    {
        if (IsInvulnerable || IsProtectedFromMelee)
            return;

        switch (Random.Range(0, 3))
        {
            case 0:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hit1);
                break;
            case 1:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hit2);
                break;
            case 2:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hit3);
                break;
        }

        ParticleManager.instance.CreateEffect(transform.position + transform.up + transform.forward, Quaternion.identity, Vector2.zero, EffectType.Explosion);

        if (OwnedMasks.Count > 0)
        {
            KeyValuePair<MaskType, Mask> m = OwnedMasks.ElementAt(Random.Range(0, OwnedMasks.Count));
            OwnedMasks.Remove(m.Key);

            if (m.Value.IsOffensive)
            {
                NextOffensiveMask();
            }
            else
            {
                NextDefensiveMask();
            }

            GameManager.instance.DropMask(m.Value, transform.position);

            DeadTime = Time.time;
        }
        else
        {
            ActiveMask_Offensive = MaskType.NONE;
            ActiveMask_Defensive = MaskType.NONE;
        }
    }

    public void OnWasHitOff()
    {
        if (IsInvulnerable || IsShieldedFromRanged)
            return;

        switch (Random.Range(0, 3))
        {
            case 0:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hitoff1);
                break;
            case 1:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hitoff2);
                break;
            case 2:
                SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.hitoff3);
                break;
        }

        ParticleManager.instance.CreateEffect(transform.position + transform.up + transform.forward, Quaternion.identity, Vector2.zero, EffectType.Lightning);

        EnteredDizzyTime = Time.time;
    }

    public void NextOffensiveMask()
    {
        IEnumerable<KeyValuePair<MaskType, Mask>> OffMasks = OwnedMasks.Where(x => x.Value.IsOffensive);

        if (OffMasks.Count() == 0)
        {
            ActiveMask_Offensive = MaskType.NONE;
            return;
        }

        if (ActiveMask_Offensive == MaskType.NONE)
        {
            ActiveMask_Offensive = OffMasks.First().Key;
            return;
        }

        bool Found = false;
        MaskType ToSearch = ActiveMask_Offensive;
        ActiveMask_Offensive = MaskType.NONE;

        foreach (KeyValuePair<MaskType, Mask> m in OffMasks)
        {
            if (Found == true)
            {
                ActiveMask_Offensive = m.Key;
                break;
            }

            if (m.Key == ToSearch)
                Found = true;
        }

        if (ActiveMask_Offensive == MaskType.NONE)
        {
            ActiveMask_Offensive = OffMasks.First().Key;
        }
    }

    public void NextDefensiveMask()
    {
        IEnumerable<KeyValuePair<MaskType, Mask>> DefMasks = OwnedMasks.Where(x => !x.Value.IsOffensive);

        if (DefMasks.Count() == 0)
        {
            ActiveMask_Defensive = MaskType.NONE;
            return;
        }

        if (ActiveMask_Defensive == MaskType.NONE)
        {
            ActiveMask_Defensive = DefMasks.First().Key;
            return;
        }

        bool Found = false;
        MaskType ToSearch = ActiveMask_Defensive;
        ActiveMask_Defensive = MaskType.NONE;

        foreach (KeyValuePair<MaskType, Mask> m in DefMasks)
        {
            if (Found == true)
            {
                ActiveMask_Defensive = m.Key;
                break;
            }

            if (m.Key == ToSearch)
                Found = true;
        }

        if (ActiveMask_Defensive == MaskType.NONE)
        {
            ActiveMask_Defensive = DefMasks.First().Key;
        }
    }

    void Move()
    {
        ResultingSpeed = new Vector3 (leftstick.x, 0, leftstick.y).normalized * RunSpeed * (DefensiveActivated == MaskType.Wind ? 3f : 1.0f);

        //Debug.Log("Stick: " + leftstick);
        //Debug.Log("Speed: " + ResultingSpeed);

        //if (Movement == MovementType.Walking && ResultingSpeed > 0.5f && Grounded)
        //    SoundManager.instance.Loop("P1Walk", transform.position, transform.rotation, SoundType.footstep);
        //else
        //    SoundManager.instance.Stop("P1Walk");

        ResultingSpeed.y = pRigidbody.velocity.y;
        pRigidbody.velocity = ResultingSpeed;

        ResultingSpeed.y = 0;
        if (ResultingSpeed.sqrMagnitude > 1f)
            transform.rotation = Quaternion.LookRotation(ResultingSpeed, Vector3.up);

        pRigidbody.angularVelocity = Vector2.zero;

        //Debug.Log("Curr: " + RotY.ToString("0.0") + " Tgt: " + TgtRot.ToString("0.0") + " Delta: " + delta.ToString("0.0") + "Req: " + ReqTorque.ToString("0.0"));
    }

    void Jump()
    {
        pRigidbody.AddForce(Vector3.up * JumpHeight * 40, ForceMode.Impulse);
        //Debug.Log("Jump!");
        
        LastJump = Time.time;
    }

    private void ShootOffensive()
    {
        LastOffensive = Time.time;

        pAnimator.SetTrigger("DefAttack");

        float roty = transform.rotation.eulerAngles.y % 360;
        float targetroty = 0;

        if (roty > 22.5f && roty < 67.5f)
            targetroty = 45f;
        else if (roty >= 67.5f && roty < 112.5f)
            targetroty = 90f;
        else if (roty >= 112.5f && roty < 157.5f)
            targetroty = 135f;
        else if (roty >= 157.5f && roty < 202.5f)
            targetroty = 180f;
        else if (roty >= 202.5f && roty < 247.5f)
            targetroty = 225f;
        else if (roty >= 247.5f && roty < 292.5f)
            targetroty = 270f;
        else if (roty >= 292.5f && roty < 337.5f)
            targetroty = 315f;
        else
            targetroty = 0f;

        Quaternion CardinalRotation = Quaternion.Euler (0, targetroty, 0);

        //Debug.Log(targetroty);

        switch (ActiveMask_Offensive)
        {
            // Cone forward 4 units = short stun + short slow
            case MaskType.Fire:

                GameObject wpn = WeaponManager.instance.CreateWeapon(transform.position + transform.up + transform.forward, CardinalRotation, transform.forward * 15, WeaponType.Fireball);
                wpn.GetComponent<WeaponController>().Owner = PlayerNumber;

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.fire);
                break;

            // SphereRaycast pull towards 5 units
            case MaskType.Earth:

                GameObject wpn2 = WeaponManager.instance.CreateWeapon(transform.position + transform.up + transform.forward, CardinalRotation, transform.forward * 15, WeaponType.Earth);
                wpn2.GetComponent<WeaponController>().Owner = PlayerNumber;

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.earth);
                break;

            // Line Raycast hit = long stun
            case MaskType.Lightning:

                GameObject wpn3 = WeaponManager.instance.CreateWeapon(transform.position + transform.up + transform.forward, CardinalRotation, transform.forward * 15, WeaponType.Lightning);
                wpn3.GetComponent<WeaponController>().Owner = PlayerNumber;
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lightning);
                break;

            // SphereRaycast pushback 3 units + long slow
            case MaskType.Water:

                GameObject wpn4 = WeaponManager.instance.CreateWeapon(transform.position + transform.up + transform.forward, CardinalRotation, transform.forward * 15, WeaponType.Water);
                wpn4.GetComponent<WeaponController>().Owner = PlayerNumber;
                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.water);
                break;
        }
    }

    private void MeleeAttack()
    {
        LastHit = Time.time;

        pAnimator.SetTrigger("MeleeAttack");

        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.meleeswing);

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.up + transform.forward, 1.5f);

        foreach (Collider col in hits)
        {
            if (col.tag == "Player" && col.gameObject != this.gameObject)
            {
                PlayerController pc = col.gameObject.GetComponent<PlayerController>();

                if (pc != null)
                {
                    pc.OnWasHitMelee();
                }
            }
        }
    }

    private void ActivateDefensive()
    {
        LastDefensive = Time.time;

        pAnimator.SetTrigger("DefAttack");

        DefensiveActivated = ActiveMask_Defensive;

        switch (ActiveMask_Defensive)
        {
            // Blocking Object behind
            case MaskType.Ice:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.ice);
                DefensiveUntilTime = Time.time + IceActivationPeriod;

                break;
            
            // Invulnerability
            case MaskType.Metal:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.metal);
                DefensiveUntilTime = Time.time + MetalActivationPeriod;

                break;

            // Invisible
            case MaskType.Nature:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.invisible);
                DefensiveUntilTime = Time.time + NatureActivationPeriod;

                break;

            // Speed
            case MaskType.Wind:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.wind);
                DefensiveUntilTime = Time.time + WindActivationPeriod;

                break;
        }
    }
}
