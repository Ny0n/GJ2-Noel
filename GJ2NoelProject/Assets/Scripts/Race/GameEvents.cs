using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    /*public delegate void StartRace();
    public static StartRace OnStartRace;

    public delegate void WaitForPlayers();
    public static WaitForPlayers OnWaitForPlayers;*/

    public delegate void GameEvent();

    public static GameEvent OnStartRace;
    public static GameEvent OnWaitForPlayers;
}
