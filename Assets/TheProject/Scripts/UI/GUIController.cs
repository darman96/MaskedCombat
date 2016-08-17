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

        if (player1Cycle.ActiveOffensive != pPlayer1.ActiveMask_Offensive)
            player1Cycle.ActiveOffensive = pPlayer1.ActiveMask_Offensive;
        if (player1Cycle.ActiveDefensive != pPlayer1.ActiveMask_Defensive)
            player1Cycle.ActiveDefensive = pPlayer1.ActiveMask_Defensive;

        if (player2Cycle.ActiveOffensive != pPlayer2.ActiveMask_Offensive)
            player2Cycle.ActiveOffensive = pPlayer2.ActiveMask_Offensive;
        if (player2Cycle.ActiveDefensive != pPlayer2.ActiveMask_Defensive)
            player2Cycle.ActiveDefensive = pPlayer2.ActiveMask_Defensive;

        if (player3Cycle.ActiveOffensive != pPlayer3.ActiveMask_Offensive)
            player3Cycle.ActiveOffensive = pPlayer3.ActiveMask_Offensive;
        if (player3Cycle.ActiveDefensive != pPlayer3.ActiveMask_Defensive)
            player3Cycle.ActiveDefensive = pPlayer3.ActiveMask_Defensive;

        if (player4Cycle.ActiveOffensive != pPlayer4.ActiveMask_Offensive)
            player4Cycle.ActiveOffensive = pPlayer4.ActiveMask_Offensive;
        if (player4Cycle.ActiveDefensive != pPlayer4.ActiveMask_Defensive)
            player4Cycle.ActiveDefensive = pPlayer4.ActiveMask_Defensive;

    }
}
