using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : GenericSingleton<LobbyManager>
{
    private Lobby _lobby;
    private Coroutine _heartbeatCoroutine;
    private Coroutine _refreshCoroutine;

    public string GetLobbyCode()
    {
        return _lobby?.LobbyCode;
    }
    
    public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
    {
        var playerData = SerializePlayerData(data);
        var player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

        var options = new CreateLobbyOptions()
        {
            Data = SerializeLobbyData(lobbyData),
            IsPrivate = isPrivate,
            Player = player
        };

        try
        {
            _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
        }
        catch (SystemException)
        {
            return false;
        }

        Debug.Log("Lobby successfuly create with id: " + _lobby.Id);

        _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6f));
        _refreshCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
        
        return true;
    }
    

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            Debug.Log("Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }
    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            
            yield return new WaitUntil(() => task.IsCompleted);
            
            Lobby newLobby = task.Result;
            if (newLobby.LastUpdated > _lobby.LastUpdated)
            {
                _lobby = newLobby;
                LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
            }
            
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }
    
    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
        var playerData = new Dictionary<string, PlayerDataObject>();
        foreach (var (key,value) in data)
        {
            playerData.Add(key, new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Member,
                value: value));
        }

        return playerData;
    }
    
    private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
    {
        var lobbyData = new Dictionary<string, DataObject>();
        foreach (var (key,value) in data)
        {
            lobbyData.Add(key, new DataObject(
                visibility: DataObject.VisibilityOptions.Member,
                value: value));
        }

        return lobbyData;
    }

    public void OnApplicationQuit()
    {
        if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
        }
    }

    public async Task<bool> JoinLobby(string lobbyCode, Dictionary<string, string> playerData)
    {
        var options = new JoinLobbyByCodeOptions();
        var player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));

        options.Player = player;
        
        try
        {
            _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
        }
        catch (SystemException)
        {
            return false;
        }

        _refreshCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
        return true;
    }

    public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
    {
        var data = new List<Dictionary<string, PlayerDataObject>>();

        foreach (var player in _lobby.Players)
        {
            data.Add(player.Data);
        }

        return data;
    }

    public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
    {
        var playerData = SerializePlayerData(data);
        var options = new UpdatePlayerOptions()
        {
            Data = playerData,
            AllocationId = allocationId,
            ConnectionInfo = connectionData
        };

        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
        }
        catch (Exception)
        {
            return false;
        }

        LobbyEvents.OnLobbyUpdated(_lobby);
        
        return true;
    }

    public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
    {
        var lobbyData = SerializeLobbyData(data);

        var options = new UpdateLobbyOptions()
        {
            Data = lobbyData
        };

        try
        {
            _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
        }
        catch (SystemException)
        {
            return false;
        }

        LobbyEvents.OnLobbyUpdated(_lobby);

        return true;
    }

    public string GetHostId()
    {
        return _lobby.HostId;
    }
}
