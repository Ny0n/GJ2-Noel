using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KartController : NetworkBehaviour
{
    public int CurrentWaypointTargetting;

    // Start is called before the first frame update
    void Start()
    {
        CurrentWaypointTargetting = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Waypoint waypoint = other.gameObject.GetComponent<Waypoint>();

        if (waypoint)
        {
            CurrentWaypointTargetting = waypoint.NextWaypoint;
        }
    }
}
