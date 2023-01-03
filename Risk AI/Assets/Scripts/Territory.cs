using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Territory
{

    public string name;

    public enum thePlayers
    {
        UNCLAIMED,
        PLAYER1,
        PLAYER2,
        PLAYER3,
        PLAYER4
    }

    public thePlayers player;

    public int troops;

}
