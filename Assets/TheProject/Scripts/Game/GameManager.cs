/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// GameManager changes the game flow
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    RoundPause,
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
    public GameObject StartPoint1;
    public GameObject StartPoint2;
    public GameObject StartPoint3;
    public GameObject StartPoint4;
    public GameObject[] Masks;

    public float MaxRoundTime = 90f;

    [HideInInspector]
    public PlayerController Player1
    {
        get
        {
            if (_Player1 == null)
                _Player1 = GameObject.Find("Player1").GetComponent<PlayerController>();

            return _Player1;
        }
        private set { }
    }
    private PlayerController _Player1;
    [HideInInspector]
    public PlayerController Player2
    {
        get
        {
            if (_Player2 == null)
                _Player2 = GameObject.Find("Player2").GetComponent<PlayerController>();

            return _Player2;
        }
        private set { }
    }
    private PlayerController _Player2;
    [HideInInspector]
    public PlayerController Player3
    {
        get
        {
            if (_Player3 == null)
                _Player3 = GameObject.Find("Player3").GetComponent<PlayerController>();

            return _Player3;
        }
        private set { }
    }
    private PlayerController _Player3;
    [HideInInspector]
    public PlayerController Player4
    {
        get
        {
            if (_Player4 == null)
                _Player4 = GameObject.Find("Player4").GetComponent<PlayerController>();

            return _Player4;
        }
        private set { }
    }
    private PlayerController _Player4;

    [HideInInspector]
    public GameState GameState;

    float startedGame;
    float startedRoundTimer;
    float startedPause;
    int NumberOfPlayers;

    [HideInInspector]
    public int Round;
    [HideInInspector]
    public int Winner;

    [HideInInspector]
    public int InputPlayer1
    {
        get; set;
    }
    [HideInInspector]
    public int InputPlayer2
    {
        get; set;
    }
    [HideInInspector]
    public int InputPlayer3
    {
        get; set;
    }
    [HideInInspector]
    public int InputPlayer4
    {
        get; set;
    }

    void Awake()
    {
        base.Awake(this);

        InputPlayer1 = PlayerPrefs.GetInt("P1", 1);
        InputPlayer2 = PlayerPrefs.GetInt("P2", 2);
        InputPlayer3 = PlayerPrefs.GetInt("P3", 3);
        InputPlayer4 = PlayerPrefs.GetInt("P4", 4);
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0) // Intro and menu
        {
            SetGameState(GameState.Menu);

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
        }
        else if (level == 1)
        {
            if (WeaponManager.instance != null)
                WeaponManager.instance.Initialize();

            if (PickupManager.instance != null)
                PickupManager.instance.Initialize();

            if (SoundManager.instance != null)
                SoundManager.instance.Initialize();

            if (ParticleManager.instance != null)
                ParticleManager.instance.Initialize();

            if (LevelManager.instance != null)
                LevelManager.instance.Initialize();

            if (StartPoint1 == null)
                StartPoint1 = GameObject.Find("StartPoint1");

            if (StartPoint2 == null)
                StartPoint2 = GameObject.Find("StartPoint2");

            if (StartPoint3 == null)
                StartPoint3 = GameObject.Find("StartPoint3");

            if (StartPoint4 == null)
                StartPoint4 = GameObject.Find("StartPoint4");

            if (Masks.Length == 0)
            {
                Masks = new GameObject[8];
                Masks[0] = GameObject.Find("FireMask");
                Masks[1] = GameObject.Find("WaterMask");
                Masks[2] = GameObject.Find("IceMask");
                Masks[3] = GameObject.Find("WindMask");
                Masks[4] = GameObject.Find("MetalMask");
                Masks[5] = GameObject.Find("NatureMask");
                Masks[6] = GameObject.Find("EarthMask");
                Masks[7] = GameObject.Find("LightningMask");
            }

            NumberOfPlayers = 0;

            if (Player1.ResetAll())
                NumberOfPlayers++;
            if (Player2.ResetAll())
                NumberOfPlayers++;
            if (Player3.ResetAll())
                NumberOfPlayers++;
            if (Player4.ResetAll())
                NumberOfPlayers++;

            PrepareNewRound();

            SetGameState(GameState.Playing);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    void Update()
    {
        switch (GameState)
        {
            case GameState.GameOver:

                if (Input.GetMouseButtonDown(0))
                {
                    SetGameState(GameState.Menu);
                }
                break;

            case GameState.Playing:

                if (Time.time - startedRoundTimer > MaxRoundTime)
                {
                    Winner = 0;
                    SetGameState(GameState.RoundPause);
                }

                if (Player1.OwnedMasks.Count == NumberOfPlayers * 2)
                {
                    Winner = 1;
                    Player1.AddWin();
                    SetGameState(GameState.RoundPause);
                }
                else if (Player2.OwnedMasks.Count == NumberOfPlayers * 2)
                {
                    Winner = 2;
                    Player2.AddWin();
                    SetGameState(GameState.RoundPause);

                }
                else if (Player3.OwnedMasks.Count == NumberOfPlayers * 2)
                {
                    Winner = 3;
                    Player3.AddWin();
                    SetGameState(GameState.RoundPause);

                }
                else if (Player4.OwnedMasks.Count == NumberOfPlayers * 2)
                {
                    Winner = 4;
                    Player4.AddWin();
                    SetGameState(GameState.RoundPause);
                }

                break;

            case GameState.RoundPause:

                if (Time.time - startedPause > 2f && Input.GetButtonDown("Submit"))
                {
                    PrepareNewRound();
                    SetGameState(GameState.Playing);
                }
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void StopGame()
    {
        SceneManager.LoadScene(0);
    }
    public void CloseGame()
    {
        Application.Quit();
    }

    public void SetGameState(GameState _GameState)
    {
        if (_GameState != GameState)
        {
            GameState = _GameState;

            switch (GameState)
            {
                case global::GameState.Menu:

                    if (SceneManager.GetActiveScene().buildIndex != 0)
                        SceneManager.LoadScene(0);
                    break;

                case global::GameState.Playing:
                    if (SceneManager.GetActiveScene().buildIndex != 1)
                    {
                        SceneManager.LoadScene(1);
                        startedGame = Time.time;
                    }

                    startedRoundTimer = Time.time;

                    break;

                case global::GameState.RoundPause:

                    startedPause = Time.time;

                    break;

                case global::GameState.GameOver:
                    break;
            }
        }
    }

    public float PlayingSince
    {
        get
        {
            return Time.time - startedGame;
        }
    }
    public float RoundSince
    {
        get
        {
            return Time.time - startedRoundTimer;
        }
    }

    public void PrepareNewRound()
    {
        Player1.ResetStartGame();
        Player2.ResetStartGame();
        Player3.ResetStartGame();
        Player4.ResetStartGame();

        GiveOutMasks();
    }
    public void GiveOutMasks()
    {
        List<Mask> MasksLeftOff = new List<Mask>();
        List<Mask> MasksLeftDef = new List<Mask>();

        for (int i = 0; i < Masks.Length; i++)
        {
            Mask m = Masks[i].GetComponent<Mask>();

            if (m == null)
                continue;

            if (m.IsOffensive)
                MasksLeftOff.Add(m);
            else
                MasksLeftDef.Add(m);
        }

        for (int j = 0; j < 4; j++)
        {
            int rnd = Random.Range(0, MasksLeftOff.Count);

            Mask m = MasksLeftOff[rnd];
            m.Owner = j + 1;
            m.gameObject.SetActive(false);

            MasksLeftOff.RemoveAt(rnd);

            switch (m.Owner)
            {
                case 1:
                    Player1.OwnedMasks.Add(m.Type, m);
                    Player1.ActiveMask_Offensive = m.Type;
                    break;
                case 2:
                    Player2.OwnedMasks.Add(m.Type, m);
                    Player2.ActiveMask_Offensive = m.Type;
                    break;
                case 3:
                    Player3.OwnedMasks.Add(m.Type, m);
                    Player3.ActiveMask_Offensive = m.Type;
                    break;
                case 4:
                    Player4.OwnedMasks.Add(m.Type, m);
                    Player4.ActiveMask_Offensive = m.Type;
                    break;
            }
        }

        for (int j = 0; j < 4; j++)
        {
            int rnd = Random.Range(0, MasksLeftDef.Count);

            Mask m = MasksLeftDef[rnd];
            m.Owner = j + 1;
            m.gameObject.SetActive(false);

            MasksLeftDef.RemoveAt(rnd);

            switch (m.Owner)
            {
                case 1:
                    Player1.OwnedMasks.Add(m.Type, m);
                    Player1.ActiveMask_Defensive = m.Type;
                    break;
                case 2:
                    Player2.OwnedMasks.Add(m.Type, m);
                    Player2.ActiveMask_Defensive = m.Type;
                    break;
                case 3:
                    Player3.OwnedMasks.Add(m.Type, m);
                    Player3.ActiveMask_Defensive = m.Type;
                    break;
                case 4:
                    Player4.OwnedMasks.Add(m.Type, m);
                    Player4.ActiveMask_Defensive = m.Type;
                    break;
            }
        }
    }

    public void DropMask(Mask m, Vector3 position)
    {
        m.Owner = 0;
        m.gameObject.transform.position = position + Vector3.up;
        m.gameObject.SetActive(true);
        m.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 3, 0), ForceMode.Force);

        SoundManager.instance.Play(position, Quaternion.identity, SoundType.dong);
    }

    public void PickupMask(Mask m, PlayerController pc)
    {
        m.Owner = pc.PlayerNumber;
        m.gameObject.SetActive(false);
        pc.OwnedMasks.Add(m.Type, m);

        SoundManager.instance.Play(pc.transform.position, Quaternion.identity, SoundType.ding);

        if (m.IsOffensive)
        {
            if (pc.ActiveMask_Offensive == MaskType.NONE)
            {
                pc.ActiveMask_Offensive = m.Type;
            }
        }
        else
        {
            if (pc.ActiveMask_Defensive == MaskType.NONE)
            {
                pc.ActiveMask_Defensive = m.Type;
            }
        }
    }
}
