using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KartController : NetworkBehaviour
{
    [NonSerialized] public bool Finished;
    [NonSerialized] public float CurrentLap;
    [NonSerialized] public int CurrentWaypointTargetting;

    // Start is called before the first frame update
    void Start()
    {
        Finished = false;
        CurrentLap = -1;
        CurrentWaypointTargetting = 11;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
