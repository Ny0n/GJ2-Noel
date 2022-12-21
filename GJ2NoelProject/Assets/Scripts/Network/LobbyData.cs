using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyData 
{
    private string _relayJoinJoinCode;
    private string _sceneName;

    public string RelayJoinCode
    {
        get => _relayJoinJoinCode; 
        set => _relayJoinJoinCode = value;
    }
    public string SceneName
    {
        get => _sceneName; 
        set => _sceneName = value;
    }

    public void Initialize(Dictionary<string, DataObject> lobbyData)
    {
        UpdateState(lobbyData);
    }

    public void UpdateState(Dictionary<string, DataObject> lobbyData)
    {
        if (lobbyData.ContainsKey("RelayJoinCode"))
        {
            _relayJoinJoinCode = lobbyData["RelayJoinCode"].Value;
        } 
        if (lobbyData.ContainsKey("SceneName"))
        {
            _sceneName = lobbyData["SceneName"].Value;
        } 
    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            {"RelayJoinCode", _relayJoinJoinCode},
            {"SceneName", _sceneName}
        };
    }
}
