/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// GameAttributes store enemy and player variables
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class GameAttributes : MonoBehaviour
{
    public GameAttributes()
    {
        AttackDelay = 2f;
        Score = 0;
        IsDead = false;
    }

    [SerializeField]
    protected float AttackDelay { get; set; }

    protected float LastAttack;

    protected float DeadTime;
    protected float GameOverTime;

    protected bool _IsDead;
    public bool IsDead
    {
        get { return _IsDead; }
        private set { _IsDead = value; }
    }

    public bool IsGameOver
    {
        get { return _Health <= 0; }
        set { _Health = 0; }
    }

    protected int _Score;
    public int Score
    {
        get { return _Score; }
        private set { _Score = value; }
    }

    protected float _Health;
    public float Health
    {
        get { return _Health; }
        private set { _Health = value; }
    }

    public virtual void Reset()
    {
        AttackDelay = 2f;
        Health = 100f;
        Score = 0;
        IsDead = false;
        transform.rotation = Quaternion.identity;
    }

    public virtual void SubtractHealth(float amount, bool cankill)
    {
        _Health -= amount;

        if (_Health <= 0)
        {
            if (cankill)
                _IsDead = true;
            else
                _Health = 1;
        }
        else
            _IsDead = false;
    }
    public void ResetHealth()
    {
        Health = 100f;
        _IsDead = false;
    }

    public virtual void AddScore(int _AddScore)
    {
        _Score += _AddScore;
    }
}
