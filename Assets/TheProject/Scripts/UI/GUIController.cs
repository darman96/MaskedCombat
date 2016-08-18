/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Changes the interface of the in game menu
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIController : MonoBehaviour
{
    public MaskCycle player1Cycle;
    public MaskCycle player2Cycle;
    public MaskCycle player3Cycle;
    public MaskCycle player4Cycle;

    public GameObject RoundPausePanel;
    public Text WinningPlayer;
    public Text GreenWins;
    public Text BlueWins;
    public Text YellowWins;
    public Text PurpleWins;
    public Text RoundCounter;

    private PlayerController pPlayer1;
    private PlayerController pPlayer2;
    private PlayerController pPlayer3;
    private PlayerController pPlayer4;

    void Start()
    {
        pPlayer1 = GameManager.instance.Player1;
        pPlayer2 = GameManager.instance.Player2;
        pPlayer3 = GameManager.instance.Player3;
        pPlayer4 = GameManager.instance.Player4;
    }

    void Update()
    {
        player1Cycle.SetMaskStatus(pPlayer1);
        player2Cycle.SetMaskStatus(pPlayer2);
        player3Cycle.SetMaskStatus(pPlayer3);
        player4Cycle.SetMaskStatus(pPlayer4);

        if (GameManager.instance.GameState == GameState.RoundPause)
        {
            RoundPausePanel.SetActive(true);

            switch (GameManager.instance.Winner)
            {
                case 0:
                    WinningPlayer.text = "No one. Cowards!";
                    break;
                case 1:
                    WinningPlayer.text = "Green";
                    break;
                case 2:
                    WinningPlayer.text = "Blue";
                    break;
                case 3:
                    WinningPlayer.text = "Purple";
                    break;
                case 4:
                    WinningPlayer.text = "Yellow";
                    break;
            }

            GreenWins.text = pPlayer1.Score.ToString();
            BlueWins.text = pPlayer2.Score.ToString();
            PurpleWins.text = pPlayer3.Score.ToString();
            YellowWins.text = pPlayer4.Score.ToString();
        }
        else
            RoundPausePanel.SetActive(false);

        RoundCounter.text = GameManager.instance.Round.ToString();
    }
}
