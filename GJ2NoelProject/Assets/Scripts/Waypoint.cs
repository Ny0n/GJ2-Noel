using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private GameManager _gameManager;

    public int NextWaypoint;

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
        KartController kartIA = other.gameObject.GetComponent<KartController>();
        KartController kartPlayer = other.gameObject.GetComponentInParent<KartController>();

        if (kartIA)
        {
            kartIA.CurrentWaypointTargetting = NextWaypoint;
            
            if (NextWaypoint == 0)
                _gameManager.NewLap(kartIA);
        }
        else if (kartPlayer)
        {
            kartPlayer.CurrentWaypointTargetting = NextWaypoint;

            if (NextWaypoint == 0)
                _gameManager.NewLap(kartPlayer);
        }
    }
}
