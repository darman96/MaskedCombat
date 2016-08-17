/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Changes the interface of the main menu
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using UnityEngine.UI;

public enum MenuSelection
{
    StartGame,
    Player1,
    Player2,
    Player3,
    Player4,
}

public class MenuManager : MonoBehaviour
{
    public GameObject StartGame;

    public GameObject ScrollTextLeft;
    public GameObject ScrollTextRight;
    public GameObject LoadScreen;

    public AudioSource TitleSound;

    public Camera MainCam;

    public TextMesh StartGameText;

    public TextMesh Player1Text;
    public TextMesh Player2Text;
    public TextMesh Player3Text;
    public TextMesh Player4Text;

    bool GoodInputJoy1;
    bool GoodInputJoy2;
    bool GoodInputJoy3;
    bool GoodInputJoy4;

    MenuSelection Choice = MenuSelection.StartGame;
    float LastSelection;
    string[] Joysticks;

    void Start ()
    {
        GameManager.instance.SetGameState(GameState.Menu);

        Invoke ("PlayIntro", 5);

        Joysticks = Input.GetJoystickNames();

        if (Joysticks.Length > 0 && Mathf.Abs(Input.GetAxis("AxisY_MenuJ1")) < 0.5f)
            GoodInputJoy1 = true;
        if (Joysticks.Length > 1 && Mathf.Abs(Input.GetAxis("AxisY_MenuJ2")) < 0.5f)
            GoodInputJoy2 = true;
        if (Joysticks.Length > 2 && Mathf.Abs(Input.GetAxis("AxisY_MenuJ3")) < 0.5f)
            GoodInputJoy3 = true;
        if (Joysticks.Length > 3 && Mathf.Abs(Input.GetAxis("AxisY_MenuJ4")) < 0.5f)
            GoodInputJoy4 = true;
    }

    void PlayIntro()
    {
        //SoundManager.instance.Play(transform.position, Quaternion.identity, SoundType.speech_intro, true);
    }

    void Update()
    {
        if (!TitleSound.isPlaying)
        {
            TitleSound.clip = (AudioClip)Resources.Load("Music/loop");
            TitleSound.Play();
        }

        UpdateMenu();

        if (Input.GetButtonDown("Submit"))
        {
            if (Choice == MenuSelection.StartGame)
                GameManager.instance.StartGame();
            else if (Choice == MenuSelection.Player1)
            {
                try
                {
                    do
                    {
                        GameManager.instance.InputPlayer1++;

                        if (GameManager.instance.InputPlayer1 > Mathf.Min(4, Joysticks.Length))
                            GameManager.instance.InputPlayer1 = -1;
                    }
                    while ((GameManager.instance.InputPlayer1 > 0 && string.IsNullOrEmpty(Joysticks[GameManager.instance.InputPlayer1 - 1])));
                }
                catch (System.Exception)
                {
                }
            }
            else if (Choice == MenuSelection.Player2)
            {
                try
                {
                    do
                    {
                        GameManager.instance.InputPlayer2++;

                        if (GameManager.instance.InputPlayer2 > Mathf.Min(4, Joysticks.Length))
                            GameManager.instance.InputPlayer2 = -1;
                    }
                    while ((GameManager.instance.InputPlayer2 > 0 && string.IsNullOrEmpty(Joysticks[GameManager.instance.InputPlayer2 - 1])));
                }
                catch (System.Exception)
                {
                }
            }
            else if (Choice == MenuSelection.Player3)
            {
                try
                {
                    do
                    {
                        GameManager.instance.InputPlayer3++;

                        if (GameManager.instance.InputPlayer3 > Mathf.Min(4, Joysticks.Length))
                            GameManager.instance.InputPlayer3 = -1;
                    }
                    while ((GameManager.instance.InputPlayer3 > 0 && string.IsNullOrEmpty(Joysticks[GameManager.instance.InputPlayer3 - 1])));
                }
                catch (System.Exception)
                {
                }
            }
            else if (Choice == MenuSelection.Player4)
            {
                try
                {
                    do
                    {
                        GameManager.instance.InputPlayer4++;

                        if (GameManager.instance.InputPlayer4 > Mathf.Min(4, Joysticks.Length))
                            GameManager.instance.InputPlayer4 = -1;
                    }
                    while ((GameManager.instance.InputPlayer4 > 0 && string.IsNullOrEmpty(Joysticks[GameManager.instance.InputPlayer4 - 1])));
                }
                catch (System.Exception)
                {
                }
            }

            PlayerPrefs.SetInt("P1", GameManager.instance.InputPlayer1);
            PlayerPrefs.SetInt("P2", GameManager.instance.InputPlayer2);
            PlayerPrefs.SetInt("P3", GameManager.instance.InputPlayer3);
            PlayerPrefs.SetInt("P4", GameManager.instance.InputPlayer4);
            PlayerPrefs.Save();
        }

        if (Input.GetButtonDown("Start"))
        {
            GameManager.instance.StartGame();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.instance.CloseGame();
        }

        if (Time.time - LastSelection < 0.25f)
            return;

        float updown = Input.GetAxis("AxisY_Menu") +
            (GoodInputJoy1 ? Input.GetAxis("AxisY_MenuJ1") : 0.0f) +
            (GoodInputJoy2 ? Input.GetAxis("AxisY_MenuJ2") : 0.0f) +
            (GoodInputJoy3 ? Input.GetAxis("AxisY_MenuJ3") : 0.0f) +
            (GoodInputJoy4 ? Input.GetAxis("AxisY_MenuJ4") : 0.0f);

        //Debug.Log("K_1: " + Input.GetAxis("AxisY_Menu") + " | 1_1: " + Input.GetAxis("AxisY_MenuJ1") + " | 2_1: " + Input.GetAxis("AxisY_MenuJ2"));
        //Debug.Log(updown);

        if (updown > 0.9f)
        {
            LastSelection = Time.time;

            switch (Choice)
            {
                case MenuSelection.Player1:
                    Choice = MenuSelection.StartGame;
                    break;
                case MenuSelection.Player2:
                    Choice = MenuSelection.Player1;
                    break;
                case MenuSelection.Player3:
                    Choice = MenuSelection.Player2;
                    break;
                case MenuSelection.Player4:
                    Choice = MenuSelection.Player3;
                    break;
            }
        }
        else if (updown < -0.9f)
        {
            LastSelection = Time.time;

            switch (Choice)
            {
                case MenuSelection.StartGame:
                    Choice = MenuSelection.Player1;
                    break;
                case MenuSelection.Player1:
                    Choice = MenuSelection.Player2;
                    break;
                case MenuSelection.Player2:
                    Choice = MenuSelection.Player3;
                    break;
                case MenuSelection.Player3:
                    Choice = MenuSelection.Player4;
                    break;
            }
        }
    }

    void UpdateMenu()
    {
        if ((Time.time + 2) % 4.0f > 3.5f && Random.Range (0, 3) == 0)
            ScrollTextLeft.SetActive(false);
        else
            ScrollTextLeft.SetActive(true);

        if (Time.time % 4.0f > 3.5f && Random.Range(0, 3) == 0)
            ScrollTextRight.SetActive(false);
        else
            ScrollTextRight.SetActive(true);

        switch (Choice)
        {
            case MenuSelection.StartGame:
                StartGameText.color = Color.blue;
                Player1Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player2Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player3Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player4Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                break;

            case MenuSelection.Player1:
                StartGameText.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player1Text.color = Color.blue;
                Player2Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player3Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player4Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                break;

            case MenuSelection.Player2:
                StartGameText.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player2Text.color = Color.blue;
                Player1Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player3Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player4Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                break;

            case MenuSelection.Player3:
                StartGameText.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player3Text.color = Color.blue;
                Player2Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player1Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player4Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                break;

            case MenuSelection.Player4:
                StartGameText.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player4Text.color = Color.blue;
                Player2Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player3Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                Player1Text.color = new Color(75f / 255f, 213f / 255f, 238f / 255f);
                break;
        }

        if (GameManager.instance != null)
        {
            if (GameManager.instance.InputPlayer1 == 0)
                Player1Text.text = "Player 1\r\nM/KB";
            else if (GoodInputJoy1)
                Player1Text.text = "Player 1\r\nJoy " + GameManager.instance.InputPlayer1 + " " + Joysticks[GameManager.instance.InputPlayer1 - 1];
            else
                Player1Text.text = "Player1: N/A";

            if (GameManager.instance.InputPlayer2 == 0)
                Player2Text.text = "Player 2\r\nM/KB";
            else if (GoodInputJoy2)
                Player2Text.text = "Player 2\r\nJoy " + GameManager.instance.InputPlayer2 + " " + Joysticks[GameManager.instance.InputPlayer2 - 1];
            else
                Player2Text.text = "Player2: N/A";

            if (GameManager.instance.InputPlayer3 == 0)
                Player3Text.text = "Player 3\r\nM/KB";
            else if (GoodInputJoy3)
                Player3Text.text = "Player 3\r\nJoy " + GameManager.instance.InputPlayer3 + " " + Joysticks[GameManager.instance.InputPlayer3 - 1];
            else
                Player3Text.text = "Player3: N/A";

            if (GameManager.instance.InputPlayer4 == 0)
                Player4Text.text = "Player 4\r\nM/KB";
            else if (GoodInputJoy4)
                Player4Text.text = "Player 4\r\nJoy " + GameManager.instance.InputPlayer4 + " " + Joysticks[GameManager.instance.InputPlayer4 - 1];
            else
                Player4Text.text = "Player4: N/A";
        }
    }
}
