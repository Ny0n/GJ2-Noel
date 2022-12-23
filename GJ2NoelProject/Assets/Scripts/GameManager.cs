using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private KartController[] _classification;
    private List<KartController> _karts = new List<KartController>();

    [SerializeField] private RectTransform _classificationPanel;
    [SerializeField] private RectTransform _finalPanel;

    public Waypoint[] Waypoints;

    // Start is called before the first frame update
    void Start()
    {
        _classification = new KartController[4];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _karts.Count; i++)
        {
            foreach (KartController kart in _karts)
            {
                if (_classification[i] && !_classification.Contains(kart))
                {
                    float posKartDistanceToTarget = Vector3.Distance(_classification[i].transform.position, Waypoints[_classification[i].CurrentWaypointTargetting].transform.position);
                    float kartDistanceToTarget = Vector3.Distance(kart.transform.position, Waypoints[kart.CurrentWaypointTargetting].transform.position);
                    if (_classification[i].CurrentLap < kart.CurrentLap
                        || (_classification[i].CurrentLap == kart.CurrentLap && _classification[i].CurrentWaypointTargetting < kart.CurrentWaypointTargetting)
                        || (_classification[i].CurrentLap == kart.CurrentLap && _classification[i].CurrentWaypointTargetting == kart.CurrentWaypointTargetting && posKartDistanceToTarget > kartDistanceToTarget))
                        _classification[i] = kart;
                }
                else if (!_classification.Contains(kart))
                    _classification[i] = kart;
            }

            PrintClassification(_classificationPanel.GetChild(i).GetComponent<TextMeshProUGUI>(), i);
            PrintClassification(_finalPanel.GetChild(i).GetComponent<TextMeshProUGUI>(), i);
        }

        for (int i = 0; i < _karts.Count; i++)
        {
            _classification[i] = null;
        }
    }

    public void AddKart(KartController kart)
    {
        _karts.Add(kart);
    }

    private void PrintClassification(TextMeshProUGUI textHolder, int place)
    {
        string name;


        NameOfPlayer playerName = _classification[place].GetComponent<NameOfPlayer>();

        if (playerName)
            name = playerName.NameSync.Value.ToString();
        else
            name = _classification[place].name;

        if (place == 0)
            textHolder.text = "1st lutin - " + name;
        else if (place == 1)
            textHolder.text = "2nd lutin - " + name;
        else if (place == 2)
            textHolder.text = "3rd lutin - " + name;
        else if (place == 3)
            textHolder.text = "4th lutin - " + name;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[(int)Mathf.Repeat(i + 1, Waypoints.Length)].transform.position);
        }
    }

    public void NewLap(KartController kart)
    {
        kart.CurrentLap++;
        if (kart.CurrentLap >= 0)
        {
            if (kart.GetComponent<ArcadeKart>())
            {
                _finalPanel.gameObject.SetActive(true);
            }
        }
    }
}
