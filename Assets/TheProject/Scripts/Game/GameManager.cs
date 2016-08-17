/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// GameManager changes the game flow
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;
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

    [HideInInspector]
    public int Round;

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

            Player1.ResetAll();
            Player2.ResetAll();
            Player3.ResetAll();
            Player4.ResetAll();

            SetGameState(GameState.Playing);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Time.time - startedRoundTimer > MaxRoundTime)
        {
            SetGameState(GameState.GameOver);
        }

        switch (GameState)
        {
            case GameState.GameOver:

                if (Input.GetMouseButtonDown(0))
                {
                    SetGameState(GameState.Menu);
                }
                break;

            case GameState.Playing:

                break;

            case GameState.RoundPause:

                if (Input.GetMouseButtonDown(0))
                {
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
}
