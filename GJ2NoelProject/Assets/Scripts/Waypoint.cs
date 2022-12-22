using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private GameManager _gameManager;

    public int NextWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        KartController kart = other.gameObject.GetComponentInParent<KartController>();

        if (kart)
        {
            if (kart.CurrentWaypointTargetting == NextWaypointIndex - 1 || (kart.CurrentWaypointTargetting == 11 && NextWaypointIndex == 0))
            {
                kart.CurrentWaypointTargetting = NextWaypointIndex;
                if (NextWaypointIndex == 0)
                {
                    _gameManager.NewLap(kart);
                }
            }
        }
    }
}
