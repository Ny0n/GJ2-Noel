using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private KartController[] _classification;
    private KartController[] _karts;

    [SerializeField] private RectTransform _classificationPanel;

    public Waypoint[] Waypoints;

    // Start is called before the first frame update
    void Start()
    {
        _karts = FindObjectsOfType<KartController>();
        _classification = new KartController[_karts.Length];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _karts.Length; i++)
        {
            foreach (KartController kart in _karts)
            {
                if (_classification[i] && !_classification.Contains(kart))
                {
                    float posKartDistanceToTarget = Vector3.Distance(_classification[i].transform.position, Waypoints[_classification[i].CurrentWaypointTargetting].transform.position);
                    float kartDistanceToTarget = Vector3.Distance(kart.transform.position, Waypoints[kart.CurrentWaypointTargetting].transform.position);
                    if (_classification[i].CurrentWaypointTargetting < kart.CurrentWaypointTargetting
                        || (_classification[i].CurrentWaypointTargetting == kart.CurrentWaypointTargetting && posKartDistanceToTarget > kartDistanceToTarget))
                        _classification[i] = kart;
                }
                else if (!_classification.Contains(kart))
                    _classification[i] = kart;
            }
            if (i == 0)
                _classificationPanel.GetChild(i).GetComponent<TextMeshProUGUI>().text = "1st lutin - " + _classification[i].name;
            else if (i == 1)
                _classificationPanel.GetChild(i).GetComponent<TextMeshProUGUI>().text = "2nd lutin - " + _classification[i].name;
            else if (i == 2)
                _classificationPanel.GetChild(i).GetComponent<TextMeshProUGUI>().text = "3rd lutin - " + _classification[i].name;
            else if (i == 3)
                _classificationPanel.GetChild(i).GetComponent<TextMeshProUGUI>().text = "4th lutin - " + _classification[i].name;
        }

        for (int i = 0; i < _classification.Length; i++)
        {
            _classification[i] = null;
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[(int)Mathf.Repeat(i + 1, Waypoints.Length)].transform.position);
        }
    }
}
