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
    public float JumpDelay = 2.0f;
    public float RunSpeed = 6f;
    public float TurnSpeed = 5f;
    public float JumpHeight = 12f;
    public float StunnedAfterMelee = 2.0f;
    public float InvulnerableAfterMelee = 5.0f;
    public float InvulnerableAfterOff = 1.0f;
    public float IceSlowsTime = 3;
    public float IceSlowsAmount = 0.3f;
    public float FireSlowsTime = 2.0f;
    public float FireSlowsAmount = 0.6f;
    public float WaterSlowsTime = 3;
    public float WaterSlowsAmount = 0.5f;
    public float FireStunsTime = 0.5f;
    public float LightningStunsTime = 1.0f;
    public float EarthStunsTime = 0.5f;
    public float PullForce = 40f;
    public float PushForce = 20f;

    public float deadzone = 0.8f;

    public GameObject StunnedEffect;
    public GameObject InvulnerableEffect;
    public GameObject PlayerMesh;

    public GameObject WindMask;
    public GameObject FireMask;
    public GameObject EarthMask;
    public GameObject LightningMask;
    public GameObject NatureMask;
    public GameObject MetalMask;
    public GameObject WaterMask;
    public GameObject IceMask;

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

    private int Joystick;
    private Vector2 leftstick;
    private Vector2 rightstick;

    private Animator pAnimator;
    private Rigidbody pRigidbody;
    private Vector3 ResultingSpeed;

    private bool IsInvulnerable;
    private bool IsStunned;
    private bool IsSlowed;
    private float SlowedAmount;
    private Vector3 ForcePushPull;

    private float InvulnerableUntilTime = -10;
    private float StunnedUntilTime = -10;
    private float SlowedUntilTime = -10;
    private float DefensiveUntilTime = -10;
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

        UpdateMasks();

        IsInvulnerable = false;
        IsStunned = false;
        IsSlowed = false;

        if (Time.time > DefensiveUntilTime)
        {
            switch (DefensiveActivated)
            {
                case MaskType.Metal:
                    StartCoroutine("ScaleDownMask", MetalMask);
                    break;

                case MaskType.Nature:
                    StartCoroutine("ScaleDownMask", NatureMask);
                    break;

                case MaskType.Wind:
                    StartCoroutine("ScaleDownMask", WindMask);
                    break;

                case MaskType.Ice:
                    StartCoroutine("ScaleDownMask", IceMask);
                    break;
            }

            DefensiveActivated = MaskType.NONE;
            PlayerMesh.SetActive(true);
        }
        else
        {
            if (DefensiveActivated == MaskType.Nature)
            {
                PlayerMesh.SetActive(false);
            }
            else if (DefensiveActivated == MaskType.Metal)
            {
                IsInvulnerable = true;
            }
        }

        // scale character while jumping
        if (transform.position.y > 0.2f)
        {
            float scale = Mathf.Clamp (1 + transform.position.y, 1, 2.0f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        { 
            transform.localScale = Vector3.one;
        }

        if (Time.time < StunnedUntilTime)
        {
            IsStunned = true;
            pRigidbody.velocity = new Vector3(0, pRigidbody.velocity.y, 0);
            StunnedEffect.SetActive(true);
        }

        if (Time.time < InvulnerableUntilTime)
        {
            IsInvulnerable = true;
            InvulnerableEffect.SetActive(true);
        }

        if (Time.time < SlowedUntilTime)
        {
            IsSlowed = true;
        }

        if (!IsStunned)
            StunnedEffect.SetActive(false);

        if (!IsInvulnerable)
            InvulnerableEffect.SetActive(false);

        if (Joystick == -1 || IsStunned)
            return;

        float deltaJump = Time.time - LastJump;
        float deltaAttack = Mathf.Min (Time.time - LastOffensive, Time.time - LastHit);

        if (Joystick == 0)
        {
            if (Input.GetButtonDown("KeyboardJump") && deltaJump > JumpDelay && transform.position.y <= 0.1f)
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
            if (Input.GetButtonDown("ButtonA_P" + Joystick) && deltaJump > JumpDelay && transform.position.y <= 0.1f)
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

        if (IsSlowed)
        {
            leftstick *= SlowedAmount;
            rightstick *= SlowedAmount;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance == null || GameManager.instance.GameState != GameState.Playing)
            return;

        UpdateAnimator();

        if (IsStunned)
            return;

        Move();
        //Debug.Log(AxisX.ToString("0.0") + " : " + AxisY.ToString("0.0") + (Jumping ? "JUMP " : " ") + (Crouching ? "CROUCH " : " ") + (IsGrounded ? "GROUNDED" : ""));
    }

    void UpdateAnimator()
    {
        pAnimator.SetFloat("Forward", (IsStunned ? 0.0f : Mathf.Abs(leftstick.y)), 0.01f, Time.deltaTime);
        pAnimator.SetBool("IsStunned", IsStunned);
        pAnimator.SetBool("IsInvulnerable", IsInvulnerable);
    }

    void UpdateMasks()
    {
        switch (ActiveMask_Defensive)
        {
            case MaskType.Metal:
                MetalMask.SetActive(true);
                IceMask.SetActive(false);
                NatureMask.SetActive(false);
                WindMask.SetActive(false);
                break;

            case MaskType.Ice:
                MetalMask.SetActive(false);
                IceMask.SetActive(true);
                NatureMask.SetActive(false);
                WindMask.SetActive(false);
                break;

            case MaskType.Nature:
                MetalMask.SetActive(false);
                IceMask.SetActive(false);
                NatureMask.SetActive(true);
                WindMask.SetActive(false);
                break;

            case MaskType.Wind:
                MetalMask.SetActive(false);
                IceMask.SetActive(false);
                NatureMask.SetActive(false);
                WindMask.SetActive(true);
                break;
        }

        switch (ActiveMask_Offensive)
        {
            case MaskType.Fire:
                FireMask.SetActive(true);
                EarthMask.SetActive(false);
                LightningMask.SetActive(false);
                WaterMask.SetActive(false);
                break;

            case MaskType.Earth:
                FireMask.SetActive(false);
                EarthMask.SetActive(true);
                LightningMask.SetActive(false);
                WaterMask.SetActive(false);
                break;

            case MaskType.Lightning:
                FireMask.SetActive(false);
                EarthMask.SetActive(false);
                LightningMask.SetActive(true);
                WaterMask.SetActive(false);
                break;

            case MaskType.Water:
                FireMask.SetActive(false);
                EarthMask.SetActive(false);
                LightningMask.SetActive(false);
                WaterMask.SetActive(true);
                break;
        }
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
        ForcePushPull = Vector3.zero;

        if (IsInvulnerable)
        {
            return;
        }

        if (col.gameObject.tag != "Weapon")
            return;

        WeaponController wpn = col.gameObject.GetComponent<WeaponController>();

        if (wpn && wpn.Owner != PlayerNumber)
            OnWasHitOff(MaskType.Fire, col.gameObject.transform.position);
    }

    public void OnWasHitMelee()
    {
        if (IsInvulnerable)
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
        }
        else
        {
            ActiveMask_Offensive = MaskType.NONE;
            ActiveMask_Defensive = MaskType.NONE;
        }

        StunnedUntilTime = Time.time + StunnedAfterMelee;
        InvulnerableUntilTime = Time.time + InvulnerableAfterMelee;
    }

    public void OnWasHitOff(MaskType weapon, Vector3 from)
    {
        if (IsInvulnerable)
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

        ParticleManager.instance.CreateEffect(transform.position + transform.up, Quaternion.identity, Vector2.zero, EffectType.OffHit);

        switch (weapon)
        {
            case MaskType.Ice:

                if (Time.time > SlowedUntilTime + 2.5f)
                {
                    SlowedUntilTime = Time.time + IceSlowsTime;
                    SlowedAmount = IceSlowsAmount;
                }
                break;

            case MaskType.Fire:

                if (Time.time > SlowedUntilTime + 2.5f)
                {
                    SlowedUntilTime = Time.time + FireSlowsTime;
                    SlowedAmount = FireSlowsAmount;
                }

                if (Time.time > StunnedUntilTime + 5f)
                    StunnedUntilTime = Time.time + FireStunsTime;

                InvulnerableUntilTime = Time.time + InvulnerableAfterOff;
                break;

            case MaskType.Earth:

                ForcePushPull = (from - transform.position).normalized * PullForce;

                if (Time.time > StunnedUntilTime + 5f)
                    StunnedUntilTime = Time.time + EarthStunsTime;

                InvulnerableUntilTime = Time.time + InvulnerableAfterOff;

                break;

            case MaskType.Water:

                ForcePushPull = (transform.position - from).normalized * PushForce;

                if (Time.time > SlowedUntilTime + 2.5f)
                {
                    SlowedUntilTime = Time.time + WaterSlowsTime;
                    SlowedAmount = WaterSlowsAmount;
                }
                break;

            case MaskType.Lightning:

                if (Time.time > StunnedUntilTime + 5f)
                    StunnedUntilTime = Time.time + LightningStunsTime;
                break;
        }
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

        ResultingSpeed += ForcePushPull;

        ForcePushPull *= 1.0f - (Time.deltaTime * 2f);

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

        pAnimator.SetTrigger("OffAttack");

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

                GameObject wpn = WeaponManager.instance.CreateWeapon(transform.position + transform.up, CardinalRotation, CardinalRotation * Vector3.forward * 15, WeaponType.Fire);
                wpn.GetComponent<WeaponController>().Owner = PlayerNumber;

                CardinalRotation = Quaternion.Euler(0, targetroty - 10, 0);
                wpn = WeaponManager.instance.CreateWeapon(transform.position + transform.up, CardinalRotation, CardinalRotation * Vector3.forward * 15, WeaponType.Fire);
                wpn.GetComponent<WeaponController>().Owner = PlayerNumber;

                CardinalRotation = Quaternion.Euler(0, targetroty + 10, 0);
                wpn = WeaponManager.instance.CreateWeapon(transform.position + transform.up, CardinalRotation, CardinalRotation * Vector3.forward * 15, WeaponType.Fire);
                wpn.GetComponent<WeaponController>().Owner = PlayerNumber;

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.fire);
                break;

            // SphereRaycast pull towards 5 units
            case MaskType.Earth:

               ParticleManager.instance.CreateEffect(transform.position + transform.up + transform.forward, CardinalRotation, Vector3.zero, EffectType.Earth);

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.earth);

                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 2.5f, CardinalRotation * Vector3.forward, 6, LayerMask.GetMask("Player1", "Player2", "Player3", "Player4"));

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        PlayerController pc = hit.collider.gameObject.GetComponent<PlayerController>();

                        if (pc != null)
                            pc.OnWasHitOff(ActiveMask_Offensive, transform.position);
                    }
                }

                break;

            // Line Raycast hit = long stun
            case MaskType.Lightning:

                ParticleManager.instance.CreateEffect(transform.position + transform.up + transform.forward, CardinalRotation, Vector3.zero, EffectType.Lightning);

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.lightning);

                RaycastHit[] hits2 = Physics.SphereCastAll(transform.position, 2.5f, CardinalRotation * Vector3.forward, 10f, LayerMask.GetMask("Player1", "Player2", "Player3", "Player4"));

                foreach (RaycastHit hit in hits2)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        PlayerController pc = hit.collider.gameObject.GetComponent<PlayerController>();

                        if (pc != null)
                            pc.OnWasHitOff(ActiveMask_Offensive, transform.position);
                    }
                }

                break;

            // SphereRaycast pushback 3 units + long slow
            case MaskType.Water:

                ParticleManager.instance.CreateEffect(transform.position + transform.up + transform.forward, CardinalRotation, Vector3.zero, EffectType.Water);

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.water);

                RaycastHit[] hits3 = Physics.SphereCastAll(transform.position, 2.5f, CardinalRotation * Vector3.forward, 6, LayerMask.GetMask("Player1", "Player2", "Player3", "Player4"));

                foreach (RaycastHit hit in hits3)
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        PlayerController pc = hit.collider.gameObject.GetComponent<PlayerController>();

                        if (pc != null)
                            pc.OnWasHitOff(ActiveMask_Offensive, transform.position);
                    }
                }

                break;
        }
    }

    private void MeleeAttack()
    {
        LastHit = Time.time;

        pAnimator.SetTrigger("MeleeAttack");

        SoundManager.instance.Play(transform.position, transform.rotation, SoundType.meleeswing);

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.up, 2.0f);

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

    private IEnumerator ScaleDownMask(GameObject Mask)
    {
        while (Mask.transform.localScale.x > 1.0f)
        {
            Mask.transform.localScale = new Vector3(Mask.transform.localScale.x - 0.2f, Mask.transform.localScale.x - 0.2f, Mask.transform.localScale.x - 0.2f);

            yield return new WaitForSeconds(0.02f);
        }

        Mask.transform.localScale = Vector3.one;
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
                IceMask.transform.localScale = new Vector3(3f, 3f, 3f);

                Collider[] hits = Physics.OverlapSphere(transform.position, 3, LayerMask.GetMask("Player1", "Player2", "Player3", "Player4"));

                foreach (Collider hit in hits)
                {
                    if (hit.gameObject != gameObject)
                    {
                        PlayerController pc = hit.gameObject.GetComponent<PlayerController>();

                        if (pc != null)
                            pc.OnWasHitOff(ActiveMask_Defensive, transform.position);
                    }
                }

                break;
            
            // Invulnerability
            case MaskType.Metal:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.metal);
                DefensiveUntilTime = Time.time + MetalActivationPeriod;
                MetalMask.transform.localScale = new Vector3(3f, 3f, 3f);

                break;

            // Invisible
            case MaskType.Nature:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.invisible);
                DefensiveUntilTime = Time.time + NatureActivationPeriod;
                NatureMask.transform.localScale = new Vector3(3f, 3f, 3f);

                break;

            // Speed
            case MaskType.Wind:

                SoundManager.instance.Play(transform.position, transform.rotation, SoundType.wind);
                DefensiveUntilTime = Time.time + WindActivationPeriod;
                WindMask.transform.localScale = new Vector3(3f, 3f, 3f);

                break;
        }
    }
}
