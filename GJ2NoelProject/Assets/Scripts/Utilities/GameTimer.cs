using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using TMPro;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    public TMP_Text Text;
    
    public NetworkVariable<float> Timer = new(0.0f);
    
    // public GameEventSO TimerBossTrigger;
    public float TimerBossTriggerValue = 60.0f;
    private float _tempValue = 0.0f;

    private void Update()
    {
        Text.SetText(Timer.Value.ToString("F1"));
        
        if (!IsServer)
            return;

        Timer.Value += Time.deltaTime;
        _tempValue += Time.deltaTime;

        if (_tempValue >= TimerBossTriggerValue)
        {
            print("TIMERRRRRRR");
            _tempValue -= TimerBossTriggerValue;
            // TimerBossTrigger.Raise();
        }
    }
}
