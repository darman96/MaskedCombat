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
    }
}
