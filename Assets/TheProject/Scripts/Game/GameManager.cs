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
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
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

    float playingTimer;
    float deadTimer;

    [HideInInspector]
    public float Highscore;

    public int InputPlayer1
    {
        get; set;
    }
    public int InputPlayer2
    {
        get; set;
    }
    public int InputPlayer3
    {
        get; set;
    }
    public int InputPlayer4
    {
        get; set;
    }

    void Awake()
    {
        base.Awake(this);
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);

        InputPlayer1 = PlayerPrefs.GetInt("P1", 0);
        InputPlayer2 = PlayerPrefs.GetInt("P2", 0);
        InputPlayer3 = PlayerPrefs.GetInt("P3", 0);
        InputPlayer4 = PlayerPrefs.GetInt("P4", 0);

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0) // Intro and menu
        {
            SetGameState(GameState.Menu);

            //SoundManager.instance.Stop("alieneating");

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
        }
        else if (level == 1)
        {
            if (EnemyManager.instance != null)
                EnemyManager.instance.Initialize();

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

            Player1.Respawn();
            Player1.Reset();
            Player2.Respawn();
            Player2.Reset();

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

                if (Time.time - deadTimer > 2)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetGameState(GameState.Menu);
                    }
                }
                break;

            case GameState.Playing:

                float DeltaGameTime = Time.time - playingTimer;

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
                        SceneManager.LoadScene(1);

                    playingTimer = Time.time;

                    break;

                case global::GameState.GameOver:

                    deadTimer = Time.time;

                    //PlayerPrefs.SetFloat("highscore", bestSurvivalTime);
                    //PlayerPrefs.Save();

                    break;
            }
        }
    }

    public float PlayingSince
    {
        get
        {
            return Time.time - playingTimer;
        }
    }

    public float DeadSince
    {
        get
        {
            return Time.time - deadTimer;
        }
    }
}
