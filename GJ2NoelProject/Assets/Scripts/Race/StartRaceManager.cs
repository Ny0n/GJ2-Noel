using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class StartRaceManager : NetworkBehaviour
{
    [Tooltip("In second")]
    [SerializeField] private float _timeBeforeCountdown;
    [Tooltip("Countdown timer max")]
    [SerializeField] private int _countDownMax;

    [SerializeField] private TextMeshProUGUI _countDownText;

    private void Start()
    {
        _countDownText.text = "Waiting for all player to be connected ...";

        if (!IsServer) return;
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        CountDownClientRpc("Waiting for all player to be connected ...");
        InvokeWaitForPlayerEventsClientRpc();

        // Should wait for all to be connected, but for now it's in time
        yield return new WaitForSeconds(_timeBeforeCountdown);

        for (int i = _countDownMax; i > 0; i--)
        {
            CountDownClientRpc(i.ToString());
            yield return new WaitForSeconds(1);
        }

        CountDownClientRpc("Start !!!");
        InvokeStartRaceEventsClientRpc();

        yield return new WaitForSeconds(1);
        CountDownClientRpc("");
    }

    [ClientRpc]
    private void InvokeWaitForPlayerEventsClientRpc()
    {
        GameEvents.OnWaitForPlayers?.Invoke();
    }

    [ClientRpc]
    private void InvokeStartRaceEventsClientRpc()
    {
        GameEvents.OnStartRace?.Invoke();
    }

    [ClientRpc]
    private void CountDownClientRpc(string value)
    {
        // Change the countdown text
        _countDownText.text = value;
    }
}
