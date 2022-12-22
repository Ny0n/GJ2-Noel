using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KartController : NetworkBehaviour
{
    public int CurrentLap;
    public int CurrentWaypointTargetting;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLap = -1;
        CurrentWaypointTargetting = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
